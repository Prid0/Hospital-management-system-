using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital_Management.Models
{
    public class PatientModel
    {
        [Key]
        public int PatientId { get; set; }
        public string Role { get; set; } = "Patient";
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public bool IsDeleted { get; set; } = false;

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public UsersModel Users { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpadatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        public ICollection<AppointmentModel> Appointments { get; set; }
        public ICollection<PrescriptionModel> Prescriptions { get; set; }
    }
}
