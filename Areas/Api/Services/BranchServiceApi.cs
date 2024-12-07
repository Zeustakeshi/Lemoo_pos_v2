using Lemoo_pos.Areas.Api.Dto;
using Lemoo_pos.Areas.Api.Services.Interfaces;
using Lemoo_pos.Data;
using Lemoo_pos.Models.Entities;

namespace Lemoo_pos.Areas.Api.Services
{
    public class BranchServiceApi : IBranchServiceApi
    {
        private readonly AppDbContext _db;
        public BranchServiceApi(AppDbContext db)
        {
            _db = db;
        }

        public List<BranchResponseDto> GetAllBranchByStoreId(long storeId)
        {
            List<Branch> branches = [.. _db.Branches.Where(branch => branch.StoreId == storeId)];

            List<BranchResponseDto> branchResponseDtos = [];

            foreach (var branch in branches)
            {
                branchResponseDtos.Add(new()
                {
                    Id = branch.Id,
                    Name = branch.Name,
                    Address = branch.Province != null ? $"{branch.Ward}, {branch.District}, {branch.Province}" : null
                });
            }
            return branchResponseDtos;
        }
    }
}