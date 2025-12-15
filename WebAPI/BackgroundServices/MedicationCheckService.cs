using Business.Abstract;
using Entities.Concrete;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebAPI.BackgroundServices
{
    public class MedicationCheckService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<MedicationCheckService> _logger;

        public MedicationCheckService(IServiceScopeFactory scopeFactory, ILogger<MedicationCheckService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("💊 İlaç Takip Dedektifi Başlatıldı...");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Veritabanı işlemleri için Scope oluşturuyoruz
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var medicationService = scope.ServiceProvider.GetRequiredService<IMedicationService>();
                        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                        // 1. Tüm İlaçları Çek
                        var allMedications = medicationService.GetAll().Data;
                        var now = DateTime.Now; // Türkiye saati (Sunucu saatine dikkat!)

                        foreach (var med in allMedications)
                        {
                            // "09:00,21:00" gibi gelen veriyi parçalıyoruz
                            var doseTimes = med.Dose.Split(',');

                            foreach (var timeStr in doseTimes)
                            {
                                if (TimeSpan.TryParse(timeStr.Trim(), out TimeSpan scheduledTime))
                                {
                                    // Bugünün o saati (Örn: Bugün 09:00)
                                    DateTime scheduleDateTime = DateTime.Today.Add(scheduledTime);

                                    // KONTROL MANTIĞI:
                                    // 1. Şu anki saat, ilaç saatini geçmiş mi? (En az 15 dk geçmiş olsun ki hemen alarm çalmasın)
                                    // 2. İlaç saati ile şu an arasındaki fark çok mu? (Örn: 2 saat geçtiyse artık kontrol etme)
                                    if (now > scheduleDateTime.AddMinutes(15) && now < scheduleDateTime.AddHours(2))
                                    {
                                        // 3. KRİTİK NOKTA: Bu ilaç için BUGÜN, BU SAATTE bir bildirim zaten oluşturulmuş mu?
                                        // (Bunu kontrol etmezsek her saniye bildirim atar!)
                                        var existingNotifications = notificationService.GetByPatient(med.UserId).Data;

                                        // Bu slot (saat) için bugün kayıt var mı?
                                        bool alreadyNotified = existingNotifications.Any(n =>
                                            n.Slot == int.Parse(med.Notes) && // Slot numarası (Kutu no)
                                            n.CreatedAt.Date == DateTime.Today && // Bugün mü?
                                            n.Message.Contains(timeStr.Trim()) // O saat için mi?
                                        );

                                        if (!alreadyNotified)
                                        {
                                            // DEMEK Kİ İLAÇ ALINMAMIŞ (veya işaretlenmemiş)!
                                            // Veritabanına "MISSED" (Atlandı) olarak yazıyoruz.
                                            var newNotification = new Notification
                                            {
                                                PatientId = med.UserId,
                                                Slot = int.Parse(med.Notes), // Kutu Numarası
                                                Status = "Missed",
                                                Message = $"DİKKAT: {med.Name} ilacı ({timeStr}) henüz alınmadı!",
                                                IsRead = false,
                                                CreatedAt = DateTime.Now // Log zamanı
                                            };

                                            notificationService.Add(newNotification);
                                            _logger.LogWarning($"⚠️ UYARI: Kullanıcı {med.UserId}, {med.Name} ilacını saat {timeStr}'de almadı. Kayıt açıldı.");

                                            // BURAYA İLERİDE FIREBASE (PUSH NOTIFICATION) KODU GELECEK
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "İlaç kontrol döngüsünde hata oluştu.");
                }

                // Dedektif 1 dakika uyusun, sonra tekrar kontrol etsin
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}