namespace Hospital_Management.Dto
{
    public class PrescriptionResponseDto
    {
        // Prescription info
        public int PrescriptionId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        // Patient info
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public string PatientEmail { get; set; }
        public string PatientPhone { get; set; }

        // Doctor info
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string DoctorEmail { get; set; }
        public string? Decease { get; set; }

        // Medicine list
        public List<PrescribedMedicineDto> Medicines { get; set; }
    }

}
