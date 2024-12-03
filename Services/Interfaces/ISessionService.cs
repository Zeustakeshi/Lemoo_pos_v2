using Lemoo_pos.Models.Entities;

namespace Lemoo_pos.Services.Interfaces
{
    public interface ISessionService
    {
        Store GetStoreSession();

        long GetStoreIdSession();

        Staff GetStaffSession();

        long GetAccountIdSession();

        void SaveAuthSession(Account account, Store store);
    }
}
