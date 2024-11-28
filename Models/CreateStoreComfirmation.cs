using System.ComponentModel.DataAnnotations;

namespace Lemoo_pos.Models
{
    public class CreateStoreComfirmation : AccountOtpInformation
    {
        public required string StoreName { get; set; }

        public required string StoreOwnerName { get; set; }

        public required string Email { get; set; }

        public required string Phone { get; set; }

        public required string Password { get; set; }
       
    }
}
