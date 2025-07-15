namespace Hospital_Management.Dto
{
    public class DoctorLeaveDto
    {
        public int DoctorId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; }
    }
}
