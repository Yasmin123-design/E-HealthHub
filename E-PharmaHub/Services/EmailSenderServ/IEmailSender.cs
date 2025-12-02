namespace E_PharmaHub.Services.EmailSenderServ
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string toEmail, string subject, string message);

    }
}
