namespace Hospital_Management.Services.Iservice
{
    public interface IEmailService
    {
        Task SendAppointmentConfirmationAsync(string toEmail, string patientName, string doctorName, DateTime appointmentDate, string slotTime);
    }
}
