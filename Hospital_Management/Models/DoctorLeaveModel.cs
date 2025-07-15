using System.ComponentModel.DataAnnotations;

namespace Hospital_Management.Models
{
    public class DoctorLeaveModel
    {
        [Key]
        public int LeaveId { get; set; }

        public int DoctorId { get; set; }
        public DoctorModel Doctor { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpadatedDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string Reason { get; set; }

        public bool OnLeave { get; set; } = true;
    }
}
