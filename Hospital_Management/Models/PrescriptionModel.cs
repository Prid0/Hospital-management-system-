using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital_Management.Models
{
    public class PrescriptionModel
    {
        [Key]
        public int PrescriptionId { get; set; }

        public int PatientId { get; set; }
        [ForeignKey("PatientId")]
        public PatientModel Patient { get; set; }

        public int DoctorId { get; set; }
        [ForeignKey("DoctorId")]
        public DoctorModel Doctor { get; set; }

        public string? Decease { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
        public ICollection<PrescribedMedicineModel> Medicines { get; set; }
    }
}
