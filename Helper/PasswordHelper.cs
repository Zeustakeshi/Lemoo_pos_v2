

using Microsoft.AspNetCore.Identity;

namespace Lemoo_pos.Helper
{
    public class PasswordHelper
    {
        private readonly PasswordHasher<object> _passwordHasher;

        public PasswordHelper()
        {
            _passwordHasher = new PasswordHasher<object>();
        }

        public string HashPassword( string plainPassword)
        {
            return _passwordHasher.HashPassword(null, plainPassword);
        }

        public bool VerifyPassword( string hashedPassword, string plainPassword)
        {
            var result = _passwordHasher.VerifyHashedPassword(null, hashedPassword, plainPassword);
            return result == PasswordVerificationResult.Success;
        }
    }
}
