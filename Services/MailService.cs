using Lemoo_pos.Services.Interfaces;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.Text;
using System.Web;
using Microsoft.Extensions.Hosting;
using Lemoo_pos.Models.Entities;
using System.Security.Cryptography.X509Certificates;

namespace Lemoo_pos.Services
{
    public class MailService : IMailService
    {
        private readonly string MAIL_USER = Environment.GetEnvironmentVariable("MAIL_USER") ?? "";
        private readonly string MAIL_PASSWORD = Environment.GetEnvironmentVariable("MAIL_PASSWORD") ?? "";

        private readonly IWebHostEnvironment _hostEnvironment;
        public MailService(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }


        public async Task SendMailAsync(string email, string subject, string mailTemplate)
        {
            // Cấu hình SMTP

            using (var smtp = new SmtpClient()
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(MAIL_USER, MAIL_PASSWORD),
                DeliveryMethod = SmtpDeliveryMethod.Network
            })
            {

                // Tạo email
                MailMessage message = new MailMessage();
                message.From = new MailAddress(MAIL_USER);
                message.ReplyToList.Add(MAIL_USER);
                message.To.Add(new MailAddress(email));
                message.Subject = subject;

                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(mailTemplate, Encoding.UTF8, MediaTypeNames.Text.Html);

                message.AlternateViews.Add(htmlView);
                message.IsBodyHtml = true;

                // Gửi email
                try
                {
                    await smtp.SendMailAsync(message);
                } catch (Exception ex)
                {
                    await Console.Out.WriteLineAsync(ex.Message);
                    throw;
                }
            }
        }

        public async Task SendAccountCreationOtp(string email, string otpCode)
        {
            string filePath = Path.Combine(_hostEnvironment.WebRootPath, "Templates", "OtpAccountCreation.html");

            string mailTemplate = await File.ReadAllTextAsync(filePath);

            mailTemplate = mailTemplate.Replace("*|USER_EMAIL|*", email)
                                   .Replace("*|OTP_CODE|*", otpCode);
            await SendMailAsync(email, "Xác thực tài khoản", mailTemplate);
        }

        public async Task SendAccountActivationEmail(
            string email, 
            string activationLink,
            string storeName,
            string staffName,
            string staffEmail,
            string supportStorePhone,
            string supportStoreEmail,
            string storeOwnerName
        )
        {
            string filePath = Path.Combine(_hostEnvironment.WebRootPath, "Templates", "AccountActivation.html");

            string mailTemplate = await File.ReadAllTextAsync(filePath);

            mailTemplate = mailTemplate
                                .Replace("*|STORE_NAME|*", storeName)
                                .Replace("*|STAFF_NAME|*", staffName)
                                .Replace("*|ACCOUNT_ACTIVATION_LINK|*", activationLink)
                                .Replace("*|STAFF_EMAIL|*", staffEmail)
                                .Replace("*|STORE_SUPPORT_PHONE|*", supportStorePhone)
                                .Replace("*|STORE_SUPPORT_EMAIL|*", supportStoreEmail)
                                .Replace("*|STORE_OWNER_NAME|*", storeOwnerName);
            await SendMailAsync(email, "Chào mừng đến với " + storeName, mailTemplate);
        }

        public async Task SendResetPasswordEmail (string username, string email, string resetPasswordLink)
        {
            string filePath = Path.Combine(_hostEnvironment.WebRootPath, "Templates", "ResetPassword.html");

            string mailTemplate = await File.ReadAllTextAsync(filePath);

            mailTemplate = mailTemplate
                .Replace("*|USER_NAME|*", username)
                .Replace("*|RESET_PASSWORD_LINK|*", resetPasswordLink);


            await SendMailAsync(email, "Hướng dẫn lấy lại mật khẩu", mailTemplate);
        }
    }
}
