namespace Hospital_Management.Dto
{
    public class ResponseDoctorDto
    {
        public int? DoctorId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public string Specilization { get; set; }
        public int DepartmentId { get; set; }
        public bool Onleave { get; set; }
        public DateTime? LeaveEndDate { get; set; }
    }
}
