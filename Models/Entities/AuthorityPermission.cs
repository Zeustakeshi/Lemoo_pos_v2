using Lemoo_pos.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace Lemoo_pos.Models.Entities
{
    public class AuthorityPermission : BaseEntity
    {
        public long AuthorityId { get; set; }
        public required Authority Authority { get; set; }

        public required Permission Permission { get; set; }
    }
}