using Core.Entities;
using System;

namespace Entities.Concrete
{
    public class Notification : IEntity
    {
        public int Id { get; set; }
        public int PatientId { get; set; }      // hasta
        public int Slot { get; set; }          // bolum no
        public string Status { get; set; } = string.Empty;   // taken / missed
        public string Message { get; set; } = string.Empty; // alarm
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
