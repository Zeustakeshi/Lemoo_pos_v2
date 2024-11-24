using System.ComponentModel.DataAnnotations;

namespace Lemoo_pos.Models.Entities
{
    public class AccountAuthority : BaseEntity
    {
        public required Account Account { get; set; }
        public required Authority Authority { get; set; }

    }
}