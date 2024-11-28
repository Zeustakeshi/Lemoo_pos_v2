using Lemoo_pos.Common.Enums;

namespace Lemoo_pos.Services.Interfaces
{
    public interface IOtpService
    {
        Task<string> SendOtp(string email, OtpType type);

        Task<string> ResendOtp(string otpCode, OtpType type);

        bool VerifyOtp(string otpCode, string plainOtp);

        void ClearOtp(string otpCode);
    }
}
