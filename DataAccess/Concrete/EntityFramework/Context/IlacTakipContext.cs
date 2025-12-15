using Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Concrete.EntityFramework.Context
{
    /// Projenin veritabanı bağlantı sınıfı (Database Context).
    public class IlacTakipContext : DbContext
    {
        // Constructor: Veritabanı bağlantı ayarlarını (connection string) 
        // dışarıdan (genellikle Program.cs) almayı sağlar.
        public IlacTakipContext(DbContextOptions<IlacTakipContext> options) : base(options)
        {
        }

        // --- Veritabanı Tabloları (DbSets) ---
        // Entity katmanında oluşturduğumuz her bir sınıf için bir DbSet.

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Medication> Medications { get; set; } = null!;
        public DbSet<Schedule> Schedules { get; set; } = null!;
        public DbSet<Intake> Intakes { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; }

        /// Kullanıcılar arası (Hasta-Hasta Yakını) takip ilişkisi tablosu.
        public DbSet<CaregiverPatient> CaregiverPatients { get; set; } = null!;

        /// Veritabanı modeli oluşturulurken çalışır. 
        /// İlişkileri, kısıtlamaları ve kuralları burada tanımlarız (Fluent API).
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // --- User (Kullanıcı) Yapılandırması ---
            modelBuilder.Entity<User>(user =>
            {
                // Email adresini benzersiz (unique) yap.
                // Aynı email ile ikinci bir kullanıcı oluşturulamasın.
                user.HasIndex(u => u.Email).IsUnique();

                // Gerekli alanları belirt (Fluent API vs. Data Annotations)
                user.Property(u => u.FullName).IsRequired().HasMaxLength(100);
                user.Property(u => u.Email).IsRequired().HasMaxLength(100);
                user.Property(u => u.PasswordHash).IsRequired();
            });

            // --- CaregiverPatient (Hasta-Hasta Yakını İlişkisi) Yapılandırması ---
            // Bu, projemizdeki en kritik yapılandırmadır.
            modelBuilder.Entity<CaregiverPatient>(relation =>
            {
                // (CaregiverId, PatientId) ikilisini benzersiz yap.
                // Bir hasta yakını bir hastayı sadece bir kez takip edebilir.
                relation.HasIndex(cp => new { cp.CaregiverId, cp.PatientId }).IsUnique();

                // 1. İlişki: Caregiver (Hasta Yakını) tarafı
                relation.HasOne(cp => cp.Caregiver)       // CaregiverPatient'ın 'Caregiver' nesnesi...
                        .WithMany(u => u.Following)       // ...User'ın 'Following' koleksiyonu ile eşleşir...
                        .HasForeignKey(cp => cp.CaregiverId) // ...'CaregiverId' yabancı anahtarı üzerinden...
                        .OnDelete(DeleteBehavior.Restrict); // ...ve bir User silinirse bu ilişkiyi silme (Restrict).

                // 2. İlişki: Patient (Hasta) tarafı
                relation.HasOne(cp => cp.Patient)         // CaregiverPatient'ın 'Patient' nesnesi...
                        .WithMany(u => u.Followers)       // ...User'ın 'Followers' koleksiyonu ile eşleşir...
                        .HasForeignKey(cp => cp.PatientId)  // ...'PatientId' yabancı anahtarı üzerinden...
                        .OnDelete(DeleteBehavior.Restrict); // ...ve bir User silinirse bu ilişkiyi silme (Restrict).
            });

            // --- Medication (İlaç) Yapılandırması ---
            modelBuilder.Entity<Medication>(med =>
            {
                // Bir ilacın bir sahibi (User) vardır.
                // Bir kullanıcının birden çok ilacı olabilir.
                med.HasOne(m => m.User)
                   .WithMany(u => u.Medications)
                   .HasForeignKey(m => m.UserId)
                   .OnDelete(DeleteBehavior.Cascade); // Kullanıcı silinirse, ilaçları da silinsin.
                                                      // Not: Eğer bir kullanıcıyı silmek yerine "pasif" yapacaksanız, 
                                                      // burayı .OnDelete(DeleteBehavior.Restrict) yapmak daha güvenli olabilir.
                                                      // Şimdilik Cascade varsayalım.
            });

            // --- Schedule (Zamanlama) Yapılandırması ---
            modelBuilder.Entity<Schedule>(schedule =>
            {
                // Bir zamanlamanın bir ilacı (Medication) vardır.
                // Bir ilacın birden çok zamanlaması (örn: sabah, öğle, akşam) olabilir.
                schedule.HasOne(s => s.Medication)
                        .WithMany(m => m.Schedules)
                        .HasForeignKey(s => s.MedicationId)
                        .OnDelete(DeleteBehavior.Cascade); // İlaç silinirse, zamanlamaları da silinsin.
            });

            // --- Intake (İlaç Alım Kaydı) Yapılandırması ---
            modelBuilder.Entity<Intake>(intake =>
            {
                // Bir kaydın bir zamanlaması (Schedule) vardır.
                // Bir zamanlamanın birden çok kaydı (örn: bugünkü, yarınki) olabilir.
                intake.HasOne(i => i.Schedule)
                      .WithMany(s => s.Intakes)
                      .HasForeignKey(i => i.ScheduleId)
                      .OnDelete(DeleteBehavior.Cascade); // Zamanlama silinirse, kayıtları da silinsin.

                // IntakeStatus enum'ını veritabanında string olarak (Planned, Taken, Missed)
                // saklamak daha okunabilir ve yönetilebilirdir.
                intake.Property(i => i.Status)
                      .HasConversion<string>()
                      .HasMaxLength(50);
            });
            // Tablo adını açıkça belirtiyoruz.
            // Eğer veritabanında tablo adın küçük harfse ("notifications") burayı değiştir!
            modelBuilder.Entity<Notification>().ToTable("notifications");
        }
    }
}