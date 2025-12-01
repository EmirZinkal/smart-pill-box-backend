using System.Text.Json.Serialization;
using Core.Entities;

namespace Entities.Concrete
{
    /// Bir ilacın hangi kurala göre (örn: "Günde 3 kez")
    /// ve hangi saat aralığında alınacağını belirler.
    public class Schedule : IEntity
    {
        public int Id { get; set; }
        public int MedicationId { get; set; }

        /// Kullanım kuralı. "Günde 3 kez" gibi basit bir metin 
        /// veya ["08:00", "14:00", "22:00"] gibi bir JSON olabilir.
        public string Rule { get; set; } = null!;

        /// İlacın alınması için hatırlatma penceresinin başlangıcı (Örn: 08:00).
        public TimeOnly? WindowStart { get; set; }

        /// İlacın alınması için hatırlatma penceresinin sonu (Örn: 09:00).
        public TimeOnly? WindowEnd { get; set; }

        // --- Navigation Properties ---

        [JsonIgnore]
        public virtual Medication Medication { get; set; } = null!;

        /// Bu zamanlamaya göre oluşturulmuş planlı ilaç alım kayıtları.
        [JsonIgnore]
        public virtual ICollection<Intake> Intakes { get; set; } = new List<Intake>();
    }
}