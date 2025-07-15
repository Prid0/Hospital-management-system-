using System.ComponentModel.DataAnnotations;

namespace Hospital_Management.Models
{
    public class PrescribedMedicineModel
    {
        [Key]
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string MedicineName { get; set; }
        public int TimesPerDay { get; set; }
        //   public int PrescriptionId { get; set; }
        //public PrescriptionModel Prescription { get; set; }
    }
}
