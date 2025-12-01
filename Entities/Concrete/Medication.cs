using System.Text.Json.Serialization;
using Core.Entities;

namespace Entities.Concrete
{
    public class Medication : IEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; } = null!;
        public string Dose { get; set; } = null!;
        public string? Notes { get; set; }

        // --- Navigation Properties ---

        [JsonIgnore]
        // 👇 DEĞİŞİKLİK BURADA: 'User?' yaptık ve '= null!' sildik.
        public virtual User? User { get; set; }

        [JsonIgnore]
        public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    }
}