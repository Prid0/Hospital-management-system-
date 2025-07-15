using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Hospital_Management.Models
{
    public class DoctorModel
    {
        [Key]
        public int DoctorId { get; set; }
        public string Role { get; set; } = "Doctor";
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public int DepartmentId { get; set; }
        public string Password { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpadatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        [ForeignKey("Users")]
        public int UserId { get; set; }

        public UsersModel Users { get; set; }

        [JsonIgnore]
        public DepartmentModel Department { get; set; }

        [JsonIgnore]
        public ICollection<AppointmentModel> Appointments { get; set; }

        [JsonIgnore]
        public ICollection<DoctorLeaveModel> LeaveRecords { get; set; }

        [JsonIgnore]
        public ICollection<PrescriptionModel> Prescriptions { get; set; }
    }
}
