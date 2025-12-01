using System.Text.Json.Serialization;
using Core.Entities;

namespace Entities.Concrete
{
    /// Bir zamanlamaya (Schedule) bağlı planlanmış tek bir ilaç alımını
    /// ve durumunu (alındı, kaçırıldı vb.) temsil eder.
    public class Intake : IEntity
    {
        public int Id { get; set; }
        public int ScheduleId { get; set; }

        /// İlacın alınması PLANLANAN zaman.
        public DateTime PlannedTime { get; set; }

        /// İlacın fiilen ALINDIĞI zaman (alındıysa).
        public DateTime? TakenTime { get; set; }

        /// Planlanan bu alımın mevcut durumu.
        public IntakeStatus Status { get; set; } = IntakeStatus.Planned;

        // --- Navigation Properties ---
        [JsonIgnore]
        public virtual Schedule Schedule { get; set; } = null!;
    }

    /// İlaç alım durumlarını belirten enum.
    public enum IntakeStatus
    {
        Planned = 1, // Planlandı (Hasta/Hasta Yakını onayı bekleniyor)
        Taken = 2,   // Alındı (Onaylandı)
        Missed = 3,  // Kaçırıldı (Zamanı geçti ve onaylanmadı)
        Skipped = 4  // Atlandı (Hasta/Hasta Yakını bilinçli olarak 'atla' dedi)
    }
}