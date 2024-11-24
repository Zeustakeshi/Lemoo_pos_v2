using Lemoo_pos.Models;
using Lemoo_pos.Models.Entities;
using System.Xml.Serialization;

namespace Lemoo_pos.Services.Interfaces
{
    public interface IAuthService
    {
        void CreateAccount(RegisterStoreViewModel model);

        Account VerifyEmail(string email, string code);

        Account Login(LoginViewModel model);
    }
}
