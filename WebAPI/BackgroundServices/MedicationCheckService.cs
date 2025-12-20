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
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var medicationService = scope.ServiceProvider.GetRequiredService<IMedicationService>();
                        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                        // Veritabanındaki tüm ilaçları çek
                        var allMedications = medicationService.GetAll().Data;
                        var now = DateTime.Now;

                        foreach (var med in allMedications)
                        {
                            // "09:00, 21:00" gibi gelen saatleri ayır
                            var doseTimes = med.Dose.Split(',');

                            foreach (var timeStr in doseTimes)
                            {
                                if (TimeSpan.TryParse(timeStr.Trim(), out TimeSpan scheduledTime))
                                {
                                    DateTime scheduleDateTime = DateTime.Today.Add(scheduledTime);

                                    // Kontrol Mantığı: İlaç saati geçti mi? (15 dk tolerans)
                                    if (now > scheduleDateTime.AddMinutes(15) && now < scheduleDateTime.AddHours(2))
                                    {
                                        var existingNotifications = notificationService.GetByPatient(med.UserId).Data;

                                        // Bugün bu saat için bir kayıt var mı?
                                        bool alreadyNotified = existingNotifications.Any(n =>
                                            n.Slot == int.Parse(med.Notes) &&
                                            n.CreatedAt.Date == DateTime.Today &&
                                            n.Message.Contains(timeStr.Trim())
                                        );

                                        if (!alreadyNotified)
                                        {
                                            // 🚨 DÜZELTME BURADA: DateTime.Now yerine DateTime.UtcNow kullandık!
                                            var newNotification = new Notification
                                            {
                                                PatientId = med.UserId,
                                                Slot = int.Parse(med.Notes),
                                                Status = "Missed",
                                                Message = $"DİKKAT: {med.Name} ilacı ({timeStr.Trim()}) alınmadı!",
                                                IsRead = false,
                                                CreatedAt = DateTime.UtcNow // <-- PostgreSQL Bunu İstiyor!
                                            };

                                            notificationService.Add(newNotification);
                                            _logger.LogWarning($"⚠️ Kullanıcı {med.UserId} için atlanan ilaç eklendi.");
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

                // 1 Dakika bekle, sonra tekrar kontrol et
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}