namespace Lemoo_pos.Models
{
    public abstract class AccountOtpInformation 
    {
        public string Code { get; } = Guid.NewGuid().ToString();
        public required string OtpCode { get; set; }

        public int ValidateCount { get; set; } = 0;
    }
}
