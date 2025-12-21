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
            _logger.LogInformation("💊 İlaç Takip ve Doktor Bildirim Sistemi Başlatıldı...");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        // Gerekli servisleri çağırıyoruz
                        var medicationService = scope.ServiceProvider.GetRequiredService<IMedicationService>();
                        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                        var caregiverService = scope.ServiceProvider.GetRequiredService<ICaregiverPatientService>();
                        var userService = scope.ServiceProvider.GetRequiredService<IUserService>(); // Hasta ismini bulmak için

                        var allMedications = medicationService.GetAll().Data;
                        var now = DateTime.UtcNow; // UTC Zamanı

                        if (allMedications != null)
                        {
                            foreach (var med in allMedications)
                            {
                                // İlaç saatlerini ayır (Örn: "09:00, 21:00")
                                var doseTimes = med.Dose.Split(',');

                                foreach (var timeStr in doseTimes)
                                {
                                    if (TimeSpan.TryParse(timeStr.Trim(), out TimeSpan scheduledTime))
                                    {
                                        DateTime todayUtc = DateTime.UtcNow.Date;
                                        DateTime scheduleDateTime = todayUtc.Add(scheduledTime);

                                        // KONTROL ZAMANI:
                                        // İlaç saati 15 dakika geçtiyse VE 2 saat dolmadıysa kontrol et.
                                        if (now > scheduleDateTime.AddMinutes(15) && now < scheduleDateTime.AddHours(2))
                                        {
                                            // Slot numarasını güvenli çevir (Hata önleyici)
                                            int.TryParse(med.Notes, out int slotNumber);

                                            // Bu hasta için bugünkü bildirimleri çek
                                            var existingNotifications = notificationService.GetByPatient(med.UserId).Data;

                                            // Bu ilaç, bu saat için DAHA ÖNCE İŞLEM GÖRDÜ MÜ?
                                            // Hem "Taken" (Alındı) hem "Missed" (Atlandı) kayıtlarına bakıyoruz.
                                            bool isProcessed = existingNotifications.Any(n =>
                                                n.CreatedAt.Date == todayUtc && // Bugün mü?
                                                n.Message.Contains(timeStr.Trim()) && // Bu saat için mi?
                                                (n.Slot == slotNumber || n.Message.Contains(med.Name)) // Doğru ilaç mı?
                                            );

                                            // Eğer ne alındı ne de atlandı kaydı yoksa -> DEMEK Kİ UNUTULDU!
                                            if (!isProcessed)
                                            {
                                                // 1. HASTAYA BİLDİRİM GÖNDER
                                                var patientNotif = new Notification
                                                {
                                                    PatientId = med.UserId,
                                                    Slot = slotNumber,
                                                    Status = "Missed",
                                                    Message = $"DİKKAT: {med.Name} ilacı ({timeStr.Trim()}) alınmadı!",
                                                    IsRead = false,
                                                    CreatedAt = DateTime.UtcNow
                                                };
                                                notificationService.Add(patientNotif);
                                                _logger.LogWarning($"⚠️ Hasta {med.UserId} için atlanan ilaç eklendi: {med.Name}");

                                                // 2. DOKTORA (HASTA YAKININA) BİLDİRİM GÖNDER
                                                var relationResult = caregiverService.GetCaregiverByPatientId(med.UserId);

                                                if (relationResult.Success && relationResult.Data != null)
                                                {
                                                    var doctorId = relationResult.Data.CaregiverId;

                                                    // Hastanın ismini bulalım ki doktor kimin unuttuğunu anlasın
                                                    var patientUser = userService.GetById(med.UserId);
                                                    string patientName = patientUser != null ? patientUser.Data.FullName : $"ID:{med.UserId}";

                                                    var doctorNotif = new Notification
                                                    {
                                                        PatientId = doctorId, // Doktora gidiyor
                                                        Slot = 0,
                                                        Status = "Alert", // Acil Uyarı
                                                        Message = $"UYARI: Hastanız {patientName}, {med.Name} ilacını saat {timeStr.Trim()}'de almadı!",
                                                        IsRead = false,
                                                        CreatedAt = DateTime.UtcNow
                                                    };
                                                    notificationService.Add(doctorNotif);
                                                    _logger.LogWarning($"👨‍⚕️ Doktora ({doctorId}) uyarı gönderildi.");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "İlaç kontrol döngüsünde kritik hata.");
                }

                // Her 1 dakikada bir kontrol et
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}