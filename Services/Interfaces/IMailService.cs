namespace Lemoo_pos.Services.Interfaces
{
    public interface IMailService
    {
        Task SendMailAsync(string email, string subject, string mailTemplate);

        Task SendAccountCreationOtp(string email, string otpCode);

    }
}
