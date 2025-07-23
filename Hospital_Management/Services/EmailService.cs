using Hospital_Management.Models; // Your model namespace
using Hospital_Management.Services.Iservice; // Your interface
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

public class EmailService : IEmailService
{
    private readonly SmtpSetting _smtpSetting;

    public EmailService(IOptions<SmtpSetting> smtpSetting)
    {
        _smtpSetting = smtpSetting.Value;
    }
    string senderEmail = Environment.GetEnvironmentVariable("EMAIL_SENDER");
    string appPassword = Environment.GetEnvironmentVariable("EMAIL_PASSWORD");

    // Use these to authenticate


    public async Task SendAppointmentConfirmationAsync(string toEmail, string patientName, string doctorName, DateTime appointmentDate, string slotTime)
    {
        var email = new MimeMessage();
        email.Sender = new MailboxAddress(_smtpSetting.DisplayName ?? "", senderEmail);
        email.From.Add(new MailboxAddress(_smtpSetting.DisplayName ?? "", senderEmail));
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = "Appointment Confirmation";

        var builder = new BodyBuilder
        {
            HtmlBody = $@"
                <p>Dear {patientName},</p>
                <p>Your appointment with <strong>{doctorName}</strong> has been confirmed.</p>
                <p><strong>Date:</strong> {appointmentDate:dddd, dd MMM yyyy}<br/>
                   <strong>Slot:</strong> {slotTime}</p>
                <p>Thank you for choosing us.</p>"
        };

        email.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_smtpSetting.Host, _smtpSetting.Port, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(senderEmail, appPassword);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}
