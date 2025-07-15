using System.ComponentModel.DataAnnotations;

namespace Hospital_Management.Models
{
    public class AppointmentModel
    {
        [Key]
        public int AppointmentId { get; set; }

        public int? PatientId { get; set; }
        public PatientModel Patient { get; set; }

        public int DoctorId { get; set; }
        public DoctorModel Doctor { get; set; }

        public DateTime AppointmentDate { get; set; }
        public DateTime? RescheduledAt { get; set; }

        // Slot can be "Morning", "Afternoon", or "Evening"
        public string SlotTime { get; set; }

        public bool AppointmentBooked { get; set; } = true;
        public bool AppointmentRescheduled { get; set; } = false;
        public bool AppointmentCancled { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
