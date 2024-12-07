using Lemoo_pos.Areas.Api.Services.Interfaces;
using Lemoo_pos.Data;
using Lemoo_pos.Areas.Api.Dto;
using Lemoo_pos.Models.Entities;
using Lemoo_pos.Services.Interfaces;

namespace Lemoo_pos.Areas.Api.Services
{
    public class AccountServiceApi : IAccountServiceApi
    {
        private readonly AppDbContext _db;

        public AccountServiceApi(AppDbContext db)
        {
            _db = db;
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
