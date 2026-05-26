namespace EMSAuthApi.Services.Abstraction
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
