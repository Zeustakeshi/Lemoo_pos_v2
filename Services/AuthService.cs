using Lemoo_pos.Data;
using Lemoo_pos.Helper;
using Lemoo_pos.Models;
using Lemoo_pos.Models.Entities;
using Lemoo_pos.Services.Interfaces;
using StackExchange.Redis;
using System.Text.Json;
using Lemoo_pos.Models.ViewModels;
using Lemoo_pos.Common.Enums;
using Microsoft.EntityFrameworkCore;

namespace Lemoo_pos.Services
{
    public class AuthService (
        AppDbContext db, 
        PasswordHelper passwordHelper, 
        IConnectionMultiplexer redis,
        IStoreService storeService,
        IAuthorityService authorityService,
        IAccountService accountService,
        IMailService mailService,
        IWebHostEnvironment hostEnvironment,
        IOtpService otpService,
        IHttpContextAccessor httpContextAccessor
    ) : IAuthService
    {
        private readonly int MAXIMUM_NUMBER_OF_VALIDATE_OTP_REQUEST = 3;


        private readonly AppDbContext _db = db;
        private readonly PasswordHelper _passwordHelper = passwordHelper;
        private readonly StackExchange.Redis.IDatabase _redis = redis.GetDatabase();
        private readonly IStoreService _storeService = storeService;
        private readonly IAuthorityService _authorityService = authorityService;
        private readonly IAccountService _accountSerivce = accountService;
        private readonly IMailService _mailService = mailService;
        private readonly IWebHostEnvironment _hostEnvironment = hostEnvironment;
        private readonly IOtpService _otpService = otpService;
        private readonly string serverBaseUrl = "https://localhost:7278";
        private readonly HttpContext _httpContext = httpContextAccessor.HttpContext;


        public async Task<string> CreateAccount(RegisterStoreViewModel model)
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


            // send account creation otp
            string otpCode = await _otpService.SendOtp(model.Email, OtpType.ACCOUNT_CREATION);

            // Save confimation to cache 

            CreateStoreComfirmation confirmation = new CreateStoreComfirmation
            {
                Email = model.Email,
                Password = model.Password,
                Phone = model.Phone,
                StoreName = model.StoreName,
                StoreOwnerName = model.Name,
                OtpCode = otpCode
            };

            string jsonData = JsonSerializer.Serialize(confirmation);

            await _redis.StringSetAsync(confirmation.Code, jsonData, TimeSpan.FromMinutes(30));

            _httpContext.Session.SetString("email", model.Email);
            _httpContext.Session.SetString("name", model.Name);
            _httpContext.Session.SetString("code", confirmation.Code);
            _httpContext.Session.SetString("type", OtpType.ACCOUNT_CREATION.ToString());

            return confirmation.Code;

        }

        public async Task<string> ResendAccountCreationOtp(string code)
        {
            string confirmationJsonData = _redis.StringGet(code).ToString();

            if (confirmationJsonData == null)
            {
                throw new Exception("OTP không hợp lệ. Vui lòng thử lại sau.");
            }

            CreateStoreComfirmation comfirmation = JsonSerializer.Deserialize<CreateStoreComfirmation>(confirmationJsonData);

            if (comfirmation == null)
            {
                throw new Exception("OTP không hợp lệ. Vui lòng thử lại sau.");
            }

            string otpCode = await _otpService.ResendOtp(comfirmation.OtpCode, OtpType.ACCOUNT_CREATION);

            comfirmation.OtpCode = otpCode;

            string comfirmationJsonData = JsonSerializer.Serialize(comfirmation);

            _redis.StringSet(code, comfirmationJsonData, _redis.KeyTimeToLive(code));

            return otpCode;
        }

        public Account VerifyAccountCreationOtp(string code, string plainOtp)
        {
            string confirmationJsonData = _redis.StringGet(code).ToString();

            if (confirmationJsonData == null)
            {
                throw new Exception("OTP không hợp lệ. Vui lòng thử lại sau.");
            }

            CreateStoreComfirmation confirmation = VerifyOtpAndRetrieveData<CreateStoreComfirmation>(code, plainOtp);

            // save account to database

            Store newStore = _storeService.CreateNewStore(confirmation.StoreName);

            Branch defaultBranch = _storeService.CreateDefaultBranch(newStore);

            _authorityService.InitNewStoreAuthority(newStore);

            Account newAccount =  _accountSerivce.CreateStoreOwner(
                email: confirmation.Email,
                phone: confirmation.Phone,
                name: confirmation.StoreOwnerName,
                password: confirmation.Password,
                store: newStore,
                branch: defaultBranch
            );

            SaveAuthSession(newAccount, newStore);

            return newAccount;

        }


        public Account Login (LoginViewModel model)
        {

            Account account = _db.Accounts
                .Include(a => a.Store)
                .SingleOrDefault(a => a.Email == model.Email);

            if (account == null)
            {
                throw new Exception("Email hoặc mật khẩu không hợp lệ.");
            }

            if (!_passwordHelper.VerifyPassword(account.Password, model.Password))
            {
                throw new Exception("Email hoặc mật khẩu không hợp lệ.");
            }

            SaveAuthSession(account, account.Store);

            return account;
        }

        public void Logout ()
        {
            _httpContext.Session.Clear();
        }


        private T VerifyOtpAndRetrieveData<T>(string code, string plainOtp) where T : AccountOtpInformation
        {
            string otpDataJson = _redis.StringGet(code).ToString();

            T otpData = JsonSerializer.Deserialize<T>(otpDataJson);

            if (otpData == null)
            {
                throw new Exception("OTP không hợp lệ");
            }

            string validationCode = otpData.Code;

            if (otpData.ValidateCount >= MAXIMUM_NUMBER_OF_VALIDATE_OTP_REQUEST)
            {

                ClearValidationInfo(validationCode, otpData.OtpCode);

                throw new Exception("Bạn đã vượt gới hạn số lần xác thực OTP. Vui lòng yêu cầu OTP mới.");
            }

            if (!_otpService.VerifyOtp(otpData.OtpCode, plainOtp))
            {
                otpData.ValidateCount++;
                string otpJsonData = JsonSerializer.Serialize(otpData);
                _redis.StringSet(code, otpJsonData, _redis.KeyTimeToLive(code));

                throw new Exception("OTP không hợp lệ");
            }

            ClearValidationInfo(validationCode, otpData.OtpCode);

            return otpData;
        }



        private void ClearValidationInfo (string validateionCode, string otpCode)
        {
            _redis.StringGetDelete(validateionCode);
            _otpService.ClearOtp(otpCode);
            _httpContext.Session.Remove("code");
            _httpContext.Session.Remove("type");
            _httpContext.Session.Remove("email");
            _httpContext.Session.Remove("name");
        }

        private void SaveAuthSession (Account account, Store store)
        {
            _httpContext.Session.SetString("AccountId", account.Id.ToString());
            _httpContext.Session.SetString("UserName", account.Name);
            _httpContext.Session.SetString("Email", account.Email);
            _httpContext.Session.SetString("Avatar", account.Avatar ?? "");
            _httpContext.Session.SetString("StoreName", store.Name);
            _httpContext.Session.SetString("StoreId", store.Id.ToString());
        }

      
    }
}
