namespace Hospital_Management.Dto
{
    public class UpdateAppointment
    {
        public int? PatientId { get; set; }

        public int DoctorId { get; set; }
        //public DateTime AppointmentDate { get; set; }
        public DateTime RescheduledAt { get; set; }

        // Slot can be "Morning", "Afternoon", or "Evening"
        public string SlotTime { get; set; }
    }
}
