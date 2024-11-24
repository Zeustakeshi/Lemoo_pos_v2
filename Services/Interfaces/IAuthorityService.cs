using Lemoo_pos.Models;
using Lemoo_pos.Models.Entities;
using System.Xml.Serialization;

namespace Lemoo_pos.Services.Interfaces
{
    public interface IAuthorityService
    {
        void InitNewStoreAuthority(Store store);    
        
    }
}
