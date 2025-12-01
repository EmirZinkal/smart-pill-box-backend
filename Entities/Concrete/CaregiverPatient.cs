using System.Text.Json.Serialization;
using Core.Entities;

namespace Entities.Concrete
{
    /// Hasta Yakını (Caregiver) ile Hasta (Patient) arasındaki ilişkiyi kuran
    /// çoka-çok (many-to-many) bağlantı tablosu.
    public class CaregiverPatient : IEntity
    {
        public int Id { get; set; }

        // Hasta Yakını (Takip Eden)
        public int CaregiverId { get; set; }

        // Hasta (Takip Edilen)
        public int PatientId { get; set; }

        // --- Navigation Properties ---

        [JsonIgnore]
        public virtual User Caregiver { get; set; } = null!;
        [JsonIgnore]
        public virtual User Patient { get; set; } = null!;
    }
}