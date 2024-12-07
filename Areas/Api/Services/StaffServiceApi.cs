using Lemoo_pos.Areas.Api.Dto;
using Lemoo_pos.Areas.Api.Services.Interfaces;
using Lemoo_pos.Common.Enums;
using Lemoo_pos.Data;
using Lemoo_pos.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lemoo_pos.Areas.Api.Services
{
    public class StaffServiceApi : IStaffServiceApi
    {

        private readonly AppDbContext _db;

        public StaffServiceApi(AppDbContext db)
        {
            _db = db;
        }

        public List<StaffResponseDto> GetAllStaff(long storeId, long? branchId)
        {
            branchId ??= _db.Branches.Where(b => b.IsDefaultBranch == true && b.StoreId == storeId)
                .Select(b => b.Id)
                .FirstOrDefault();

            return [.. _db.Staffs
                .Where(s => s.Branch.Store.Id == storeId && s.Branch.Id == branchId && s.Status == StaffStatus.ACTIVE)
                .Include(s => s.Branch)
                    .ThenInclude(branch => branch.Store)
                .Include(s => s.Account)
                .Select(s => new StaffResponseDto(){
                    Id = s.Id,
                    Name = s.Account.Name,
                    Avatar = s.Account.Avatar,
                    BranchId = branchId
                })
            ];

            // foreach (var staff in staffs)
            // {
            //     List<AccountAuthority> authorities = [.. _db.AccountAuthorities.Where(a => a.Account.Id == staff.Account.Id)];
            //     staff.Account.Authorities = authorities;
            // }

        }
    }
}