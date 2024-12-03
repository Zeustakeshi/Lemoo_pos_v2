using Lemoo_pos.Models.Entities;
using Lemoo_pos.Models.ViewModels;
using System.Security.Claims;
using System.Xml.Serialization;

namespace Lemoo_pos.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> CreateAccount(RegisterStoreViewModel model);

        Task<string> ResendAccountCreationOtp(string code);

        Account VerifyAccountCreationOtp(string code, string plainOtp);

        Account Login(LoginViewModel model);

        void Logout();

        string GenerateResetPasswordToken(long accountId, string email);

        string GetnerateAuthorizationToken(long accountId, long storeId);

        ClaimsPrincipal? ValidateJwtToken(string token);

        Task RecoverPassword(RecoverPasswordViewModel model);

        Account ResetPassword(ResetPasswordViewModel model, string token);
    }
}
