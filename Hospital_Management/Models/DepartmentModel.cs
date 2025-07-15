using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Hospital_Management.Models
{
    public class DepartmentModel
    {
        [Key]
        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; }

        [JsonIgnore]
        public ICollection<DoctorModel> Doctors { get; set; }
    }
}
