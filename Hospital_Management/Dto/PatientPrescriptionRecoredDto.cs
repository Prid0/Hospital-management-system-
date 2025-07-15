namespace Hospital_Management.Dto
{
    public class PatientPrescriptionRecoredDto
    {
        // Doctor info
        public int PrescriptionId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string DoctorEmail { get; set; }
        public string? Decease { get; set; }

        // Medicine list
        public List<PrescribedMedicineDto> Medicines { get; set; }
    }
}
