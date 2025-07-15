using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital_Management.Models
{
    public class AdminModel
    {
        [Key]
        public int AdminId { get; set; }
        public string Role { get; set; } = "Admin";
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public UsersModel Users { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpadatedDate { get; set; }

        public int? UpdatedBy { get; set; }
    }
}
