using Lemoo_pos.Models.Entities;
using Lemoo_pos.Models.ViewModels;
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
    }
}
