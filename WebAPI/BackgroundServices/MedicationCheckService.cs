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
            _logger.LogInformation("💊 İlaç Takip ve Doktor Bildirim Sistemi (TR Saati) Başlatıldı...");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var medicationService = scope.ServiceProvider.GetRequiredService<IMedicationService>();
                        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                        var caregiverService = scope.ServiceProvider.GetRequiredService<ICaregiverPatientService>();
                        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

                        var allMedications = medicationService.GetAll().Data;

                        // 🔴 DÜZELTME: Sunucu saati (UTC) yerine TÜRKİYE saati (UTC+3) baz alınıyor.
                        var nowTr = DateTime.UtcNow.AddHours(3);

                        if (allMedications != null)
                        {
                            foreach (var med in allMedications)
                            {
                                var doseTimes = med.Dose.Split(',');

                                foreach (var timeStr in doseTimes)
                                {
                                    if (TimeSpan.TryParse(timeStr.Trim(), out TimeSpan scheduledTime))
                                    {
                                        // Hesaplamaları bugünün TÜRKİYE tarihine göre yapıyoruz
                                        DateTime todayTr = nowTr.Date;
                                        DateTime scheduleDateTime = todayTr.Add(scheduledTime);

                                        // KONTROL: Türkiye saatiyle 15 dk geçti mi?
                                        if (nowTr > scheduleDateTime.AddMinutes(15) && nowTr < scheduleDateTime.AddHours(2))
                                        {
                                            int.TryParse(med.Notes, out int slotNumber);
                                            var existingNotifications = notificationService.GetByPatient(med.UserId).Data;

                                            // Kontrol ederken veritabanındaki UTC kayıtlarına bakmaya devam ediyoruz
                                            bool isProcessed = existingNotifications.Any(n =>
                                                n.CreatedAt.Date == DateTime.UtcNow.Date && // Veritabanı UTC tutar
                                                n.Message.Contains(timeStr.Trim()) &&
                                                (n.Slot == slotNumber || n.Message.Contains(med.Name))
                                            );

                                            if (!isProcessed)
                                            {
                                                // 1. HASTAYA BİLDİRİM
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
                                                _logger.LogWarning($"⚠️ Hasta {med.UserId} için atlanan ilaç eklendi: {med.Name} (Saat: {timeStr})");

                                                // 2. DOKTORA BİLDİRİM
                                                var relationResult = caregiverService.GetCaregiverByPatientId(med.UserId);

                                                if (relationResult.Success && relationResult.Data != null)
                                                {
                                                    var doctorId = relationResult.Data.CaregiverId;
                                                    var patientUser = userService.GetById(med.UserId);
                                                    string patientName = patientUser != null ? patientUser.Data.FullName : $"ID:{med.UserId}";

                                                    var doctorNotif = new Notification
                                                    {
                                                        PatientId = doctorId,
                                                        Slot = 0,
                                                        Status = "Alert",
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

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}