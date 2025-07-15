namespace Hospital_Management.Dto
{
    public class PrescribedMedicineDto
    {
        public string MedicineName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TimesPerDay { get; set; }

    }

}
