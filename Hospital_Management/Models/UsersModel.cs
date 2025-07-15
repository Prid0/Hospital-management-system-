using System.ComponentModel.DataAnnotations;

namespace Hospital_Management.Models
{
    public class UsersModel
    {
        [Key]
        public int UserId { get; set; }
        public string Role { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpadatedDate { get; set; }
        public int? UpdatedBy { get; set; }

        //[InverseProperty("Users")]
        //public ICollection<DoctorModel> Doctors { get; set; }

        //[InverseProperty("Users")]
        //public ICollection<PatientModel> Patients { get; set; }
    }
}
