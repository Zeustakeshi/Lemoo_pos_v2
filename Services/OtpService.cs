using Lemoo_pos.Common.Enums;
using Lemoo_pos.Helper;
using Lemoo_pos.Models;
using Lemoo_pos.Services.Interfaces;
using StackExchange.Redis;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace Lemoo_pos.Services
{
    public class OtpService : IOtpService
    {

        private readonly int MAXIMUM_NUMBER_OF_SEND_OTP_REQUEST = 5;
        private readonly int OTP_SIZE = 6;
        private static readonly TimeSpan OTP_EXPIRED_TIME = TimeSpan.FromMinutes(3);
        private static readonly char[] OTP_CHARSET = ['1', '2', '3', '4', '5', '6', '7', '8', '9', '0'];
        private readonly PasswordHelper _passwordHelper;
        private IDatabase _redis;
        private readonly IMailService _mailService;

        public OtpService (
            IConnectionMultiplexer redis, 
            PasswordHelper passwordHelper, 
            IMailService mailService  
        ){
            _redis = redis.GetDatabase();
            _mailService = mailService;
            _passwordHelper = passwordHelper;
        }

        public async Task<string> SendOtp(string email, OtpType type)
        {
            return await SendOtpWithResendCount(
                email: email,
                type: type,
                resendCount: 0,
                handleSendOtp: GetSendOtpHandler(type)
            );
        }

        public async Task<string> ResendOtp(string otpCode, OtpType type)
        {
          
            Otp existingOtp = GetOtpByCode(otpCode);

            if (existingOtp == null || !existingOtp.Type.Equals(type))
            {
                throw new Exception("OTP code does not match the requested type.");
            }


            if (existingOtp.ResendCount >= MAXIMUM_NUMBER_OF_SEND_OTP_REQUEST)
            {
                throw new Exception(
                        "You have reached the limit for OTP resend attempts. Please try again later.");
            }

            ClearOtp(otpCode);

            return await SendOtpWithResendCount(
                email: existingOtp.Email,
                type: type, 
                resendCount: existingOtp.ResendCount + 1,
                handleSendOtp: GetSendOtpHandler(type)
            );

        }

        public bool VerifyOtp(string otpCode, string plainOtp)
        {
            Otp existingOtp = GetOtpByCode(otpCode);
            return _passwordHelper.VerifyPassword(existingOtp.Value, plainOtp);
        }

        public void ClearOtp(string otpCode)
        {
            _redis.StringGetDelete(otpCode);
        }

        private async Task<string> SendOtpWithResendCount (string email, OtpType type, int resendCount, Func<string, string, Task> handleSendOtp)
        {
            string plainOtp = GenerateOtp();
            string hashOtp = _passwordHelper.HashPassword(plainOtp);

            Otp otp = new()
            {
                Type = type,
                Value = hashOtp,
                ResendCount = resendCount,
                Email = email
            };

            string otpJson = JsonSerializer.Serialize(otp);

            _redis.StringSetAsync(otp.Code, otpJson, OTP_EXPIRED_TIME);

            try
            {
                handleSendOtp(email, plainOtp);
            }catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                throw;
            }


            return otp.Code;
        }


        public string GenerateOtp()
        {
            var random = new Random();
            var otp = new char[OTP_SIZE];

            for (int i = 0; i < OTP_SIZE; i++)
            {
                otp[i] = OTP_CHARSET[random.Next(OTP_CHARSET.Length)];
            }

            return new string(otp);
        }

        private Otp GetOtpByCode (string otpCode)
        {
            string otpString = _redis.StringGet(otpCode).ToString();
            
            if (otpString == null || otpString.Length == 0)
            {
                throw new Exception("Invalid Otp code.");
            }

            return JsonSerializer.Deserialize<Otp>(otpString);
        }

        private async Task HandleSendCreationOtp (string email, string plainOtp)
        {
            await _mailService.SendAccountCreationOtp(email, plainOtp);
        }

        private async Task HandleSendMfaOtp(string email, string plainOtp)
        {
            await Console.Out.WriteLineAsync();
        }

        private Func<string, string, Task> GetSendOtpHandler (OtpType type)
        {
            return type switch
            {
                OtpType.ACCOUNT_CREATION => HandleSendCreationOtp,
                OtpType.MFA => HandleSendMfaOtp,
                _ => HandleSendCreationOtp,
            };
        } 
    }
}
