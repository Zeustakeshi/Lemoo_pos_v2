using Lemoo_pos.Areas.Api.Dto;
using Lemoo_pos.Models.Entities;

namespace Lemoo_pos.Mappers
{
    public class StaffMapper
    {
        public static StaffResponseDto StaffToStaffResponseDto(Staff staff)
        {
            return new()
            {
                Id = staff.Id,
                Name = staff.Account.Name,
                Avatar = staff.Account.Avatar,
                BranchId = staff.Branch.Id
            };
        }
    }
}