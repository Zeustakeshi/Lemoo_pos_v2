using Lemoo_pos.Data;
using Lemoo_pos.Models.Entities;
using Lemoo_pos.Services.Interfaces;

namespace Lemoo_pos.Services
{
    public class AccountService : IAccountService
    {

        private readonly AppDbContext _db;

        public AccountService (AppDbContext db)
        {
            _db = db;
        }

        public Account CreateStoreOwner(string email, string phone, string name, string password, Store store, Branch branch)
        {

            Account storeOwnerAccount = new Account()
            {
                Email = email,
                Phone = phone,
                Name = name,
                Password = password,
                Avatar = "https://i.pravatar.cc/152",
                Authorities = new List<AccountAuthority>(),
                Store = store
            };

            Authority storeOwnerAuthority = _db.Authorities.Single(a => a.Name == "Chủ cửa hàng" && a.Store.Id == store.Id);

        
            if (storeOwnerAuthority == null)
            {
                throw new Exception("Store owner authority is not found");
            }

            _db.Accounts.Add(storeOwnerAccount);

            AccountAuthority accountAuthority = _db.AccountAuthorities.Add(new() { Account = storeOwnerAccount, Authority = storeOwnerAuthority }).Entity;

            storeOwnerAccount.Authorities.Add(accountAuthority);


            Staff staff = new() { Account = storeOwnerAccount, Branch = branch };

            _db.Staffs.Add(staff);

            _db.SaveChanges();


            return storeOwnerAccount;
        }

    }
}
