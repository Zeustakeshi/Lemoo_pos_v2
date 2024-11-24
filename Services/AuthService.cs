using Lemoo_pos.Data;
using Lemoo_pos.Helper;
using Lemoo_pos.Models;
using Lemoo_pos.Models.Entities;
using Lemoo_pos.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using StackExchange.Redis;
using System.Text.Json;
using static System.Formats.Asn1.AsnWriter;

namespace Lemoo_pos.Services
{
    public class AuthService(
        AppDbContext db, 
        PasswordHelper passwordHelper, 
        IConnectionMultiplexer redis,
        IStoreService storeService,
        IAuthorityService authorityService,
        IAccountService accountService
    ) : IAuthService
    {
        private readonly AppDbContext _db = db;
        private readonly PasswordHelper _passwordHelper = passwordHelper;
        private readonly StackExchange.Redis.IDatabase _redis = redis.GetDatabase();
        private readonly IStoreService _storeService = storeService;
        private readonly IAuthorityService _authorityService = authorityService;
        private readonly IAccountService _accountSerivce = accountService;


        public void CreateAccount(RegisterStoreViewModel model)
        {
            bool isExitedAccount = _db.Accounts.Any(account => account.Email == model.Email || account.Phone == model.Phone);

            bool isExistedStore = _db.Stores.Any(s => s.Name.Equals(model.StoreName));

            if (isExitedAccount)
            {
                throw new Exception("Email hoặc số điện thoại đã tồn tại.");
            }

            if (isExistedStore)
            {
                throw new Exception("Tên cửa hàng đã tồn tại");
            }


            // Hash password
            model.Password = _passwordHelper.HashPassword(model.Password);


            // Save confimation to cache 
            string confirmationCode = Guid.NewGuid().ToString();
            string hashConfirmationCode = _passwordHelper.HashPassword(confirmationCode);

            CreateStoreComfirmation confirmation = new CreateStoreComfirmation
            {
                Email = model.Email,
                Password = model.Password,
                Phone = model.Phone,
                StoreName = model.StoreName,
                StoreOwnerName = model.Name,
                Code = hashConfirmationCode
            };

            string jsonData = JsonSerializer.Serialize(confirmation);

            Console.WriteLine(jsonData);

            _redis.StringSetAsync("EMAIL_VEIRFY_" + model.Email , jsonData, TimeSpan.FromHours(12));


            // send email verify account

            Console.WriteLine(confirmationCode);
        }

        public Account VerifyEmail (string email, string code)
        {
            string confirmationJsonData = _redis.StringGet("EMAIL_VEIRFY_" + email).ToString();
            

            if (confirmationJsonData == null)
            {
                Console.WriteLine("verify email " + email + " failed!. Invalid confirmation email.");
                return null;
            }

            try
            {
                CreateStoreComfirmation confirmation = JsonSerializer.Deserialize<CreateStoreComfirmation>(confirmationJsonData);
                
                if (confirmation == null)
                {
                    Console.WriteLine("verify email " + email + " failed!. Invalid confirmation code.");
                    return null;
                }

                if (!_passwordHelper.VerifyPassword(confirmation.Code, code))
                {
                    Console.WriteLine("verify email " + email + " failed!. Invalid confirmation code.");
                    return null;
                }

                _redis.StringGetDelete("EMAIL_VEIRFY_" + email);

                // save account to database

                Store newStore =  _storeService.CreateNewStore(confirmation.StoreName);

                Branch defaultBranch = _storeService.CreateDefaultBranch(newStore);

                _authorityService.InitNewStoreAuthority(newStore);

                return _accountSerivce.CreateStoreOwner(
                    email: confirmation.Email,
                    phone: confirmation.Phone,
                    name: confirmation.StoreOwnerName,
                    password: confirmation.Password,
                    store: newStore,
                    branch: defaultBranch
                );


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public Account Login (LoginViewModel model)
        {

            Account account = _db.Accounts.SingleOrDefault(a => a.Email == model.Email);

            if (account == null)
            {
                throw new Exception("Email hoặc mật khẩu không hợp lệ.");
            }

            if (!_passwordHelper.VerifyPassword(account.Password, model.Password))
            {
                throw new Exception("Email hoặc mật khẩu không hợp lệ.");
            }

            return account;
        }
    }
}
