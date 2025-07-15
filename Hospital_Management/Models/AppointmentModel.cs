using System.ComponentModel.DataAnnotations;

namespace Hospital_Management.Models
{
    public enum AppointmentStatus
    {
        Booked,
        Cancelled,
        Rescheduled
    }

    public class AppointmentModel
    {
        [Key]
        public int AppointmentId { get; set; }
        public int? PatientId { get; set; }
        public PatientModel Patient { get; set; }

        public int DoctorId { get; set; }
        public DoctorModel Doctor { get; set; }

        public DateTime AppointmentDate { get; set; }

        public TimeSpan SlotStartTime { get; set; }
        public TimeSpan SlotEndTime { get; set; }

        public AppointmentStatus Status { get; set; } = AppointmentStatus.Booked;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? RescheduledAt { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpadatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        //public int? PrescriptionId { get; set; }
        //public PrescriptionModel Prescription { get; set; }
    }
}
