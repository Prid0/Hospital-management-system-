namespace Hospital_Management.Dto
{
    public class AddPrescriptionDto
    {
        public int DoctorId { get; set; }
        public int PatientId { get; set; }
        public string? Decease { get; set; }

        public List<PrescribedMedicineDto>? Medicines { get; set; }
    }
}
