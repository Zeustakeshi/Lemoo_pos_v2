using Lemoo_pos.Models;
using Lemoo_pos.Models.Dto;
using Lemoo_pos.Models.Entities;
using System.Xml.Serialization;

namespace Lemoo_pos.Services.Interfaces
{
    public interface IAccountService
    {
        Account CreateAccount(string email, string phone, string name, string password, Store store, bool isActive, List<Authority> authorities);

        AccountInfoResponseDto GetAccountById(long accountId);
    }
}
