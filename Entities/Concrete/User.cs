using System.Text.Json.Serialization;
using Core.Entities;

namespace Entities.Concrete
{
    public class User : IEntity
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;

        // YENİ EKLENEN: Kullanıcının tipi ("Patient" veya "Relative")
        public string Role { get; set; } = null!;
        public DateTime? LastLoginAt { get; set; }

        // --- Navigation Properties (İlişkisel Özellikler) ---

        /// Bu kullanıcının sahip olduğu KENDİ ilaçları.
        /// (Bir hasta yakını da kendi ilaçlarını takip edebilir)
        [JsonIgnore]
        public virtual ICollection<Medication> Medications { get; set; } = new List<Medication>();

        /// Bu kullanıcının TAKİP ETTİĞİ kişiler (Hastalar).
        /// (Bu kullanıcı "Hasta Yakını" rolündedir)
        [JsonIgnore]
        public virtual ICollection<CaregiverPatient> Following { get; set; } = new List<CaregiverPatient>();

        /// Bu kullanıcıyı TAKİP EDEN kişiler (Hasta Yakınları).
        /// (Bu kullanıcı "Hasta" rolündedir)
        [JsonIgnore]
        public virtual ICollection<CaregiverPatient> Followers { get; set; } = new List<CaregiverPatient>();
    }
}