namespace Hospital_Management.Dto
{
    public class PatientMedicalRecordDto
    {
        // Patient info
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public string PatientEmail { get; set; }
        public string PatientPhone { get; set; }

        // Prescription list
        public List<PatientPrescriptionRecoredDto> PrescriptionRecord { get; set; }

    }
}
