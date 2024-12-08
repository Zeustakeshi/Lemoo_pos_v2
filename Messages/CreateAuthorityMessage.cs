using Lemoo_pos.Common.Enums;
using Lemoo_pos.Models.Entities;
using Lemoo_pos.Models.ViewModels;

namespace Lemoo_pos.Messages
{
    public class CreateAuthorityMessage : CreateRoleViewModel
    {
        public required long StoreId { get; set; }
    }
}