namespace Lemoo_pos.Services.Interfaces
{
    public interface IMailService
    {
        Task SendMailAsync(string email, string subject, string mailTemplate);

        Task SendAccountCreationOtp(string email, string otpCode);

        Task SendAccountActivationEmail(
            string email,
            string activationLink,
            string storeName,
            string staffName,
            string staffEmail,
            string supportStorePhone,
            string supportStoreEmail,
            string storeOwnerName
        );

        Task SendResetPasswordEmail(string username, string email, string resetPasswordLink);
    }
}
