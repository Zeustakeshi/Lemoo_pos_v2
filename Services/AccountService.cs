using Lemoo_pos.Data;
using Lemoo_pos.Areas.Api.Dto;
using Lemoo_pos.Models.Entities;
using Lemoo_pos.Services.Interfaces;

namespace Lemoo_pos.Services
{
    public class AccountService : IAccountService
    {

        private readonly AppDbContext _db;

        public AccountService(AppDbContext db)
        {
            _db = db;
        }

        public Account CreateAccount(
            string email,
            string phone,
            string name,
            string password,
            Store store,
            bool isActive,
            List<Authority> authorities
         )
        {
            Account account = new Account()
            {
                Email = email,
                Phone = phone,
                Name = name,
                Password = password,
                Avatar = "https://res.cloudinary.com/dymmvrufy/image/upload/v1733197320/lemoo_pos/user/avatar/aqg8kncycubekkiwpvys.jpg",
                Authorities = [],
                Store = store,
                IsActive = isActive
            };

            _db.Accounts.Add(account);

            foreach (Authority authority in authorities)
            {
                AccountAuthority accountAuthority = _db.AccountAuthorities.Add(new() { Account = account, Authority = authority }).Entity;
                account.Authorities.Add(accountAuthority);
            }

            _db.SaveChanges();

            return account;
        }

        public AccountInfoResponseDto GetAccountById(long accountId)
        {
            Account account = _db.Accounts.Single(a => a.Id == accountId) ?? throw new Exception($"Account doesn't exist");
            return new()
            {
                Id = accountId,
                Email = account.Email,
                Name = account.Name,
                Avatar = account.Avatar,
                Phone = account.Phone,
            };
        }
    }
}
