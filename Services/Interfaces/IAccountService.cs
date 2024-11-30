using Lemoo_pos.Models;
using Lemoo_pos.Models.Entities;
using System.Xml.Serialization;

namespace Lemoo_pos.Services.Interfaces
{
    public interface IAccountService
    {
        Account CreateStoreOwner(string email, string phone, string name, string password, Store store, Branch branch);      
        
    }
}
