using System.Security.Claims;
using Lemoo_pos.Areas.Api.Dto;

namespace Lemoo_pos.Helper
{
    public class ApiHelper
    {
        public static JwtDataDto GetJwtDataDto(ClaimsPrincipal user)
        {
            string accountIdString = user.Claims
             .FirstOrDefault(c => c.Type == "accountId")?.Value ??
             throw new Exception("Invalid jwt token.");

            string storeIdString = user.Claims
                .FirstOrDefault(c => c.Type == "storeId")?.Value ??
                throw new Exception("Invalid jwt token.");

            return new()
            {
                AccountId = Convert.ToInt64(accountIdString),
                StoreId = Convert.ToInt64(storeIdString)
            };
        }
    }
}