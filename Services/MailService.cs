using Lemoo_pos.Services.Interfaces;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.Text;
using System.Web;
using Microsoft.Extensions.Hosting;

namespace Lemoo_pos.Services
{
    public class MailService : IMailService
    {

        private readonly string MAIL_USER = "gaconmvp2312@gmail.com";
        private readonly string MAIL_PASSWORD = "wdgb dheg spym wjkn";

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
                await smtp.SendMailAsync(message);
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

    }
}
