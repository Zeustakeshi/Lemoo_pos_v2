using Lemoo_pos.Common.Enums;

namespace Lemoo_pos.Models
{
    public class Otp
    {
        public string Code { get; } = Guid.NewGuid().ToString();
        public required string Value { get; set; }
        public required string Email { get; set; }
        public required OtpType Type { get; set; }
        public int ResendCount { get; set; } = 0;

    }
}
