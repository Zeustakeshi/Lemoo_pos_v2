using Lemoo_pos.Data;
using Lemoo_pos.Areas.Api.Dto;
using Lemoo_pos.Models.Entities;
using Lemoo_pos.Models.ViewModels;
using Lemoo_pos.Services.Interfaces;

namespace Lemoo_pos.Services
{
    public class BranchService : IBranchService
    {
        private readonly AppDbContext _db;
        private readonly ISessionService _sessionService;

        public BranchService(AppDbContext db, ISessionService sessionService)
        {
            _db = db;
            _sessionService = sessionService;
        }

        public Branch CreateDefaultBranch(Store store, string email, string phone)
        {
            Branch defaultBranch = new()
            {
                Name = "Cửa hàng chính",
                Store = store,
                StoreId = store.Id,
                Email = email,
                Phone = phone,
                IsDefaultBranch = true,
                IsActive = true
            };
            return _db.Branches.Add(defaultBranch).Entity;
        }

        public Branch CreateBranch(SaveBranchViewModel model)
        {
            long storeId = _sessionService.GetStoreIdSession();

            bool isExistedBranch = _db.Branches.Any(b => b.Name.Equals(model.Name) && b.StoreId.Equals(storeId));
            if (isExistedBranch)
            {
                throw new Exception("Tên chi nhánh đã tồn tại");
            }

            Store store = _sessionService.GetStoreSession();

            Branch branch = new()
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                Store = store,
                StoreId = store.Id,
                IsDefaultBranch = false,
                IsActive = model.IsActive,
                Province = model.Province,
                ProvinceCode = model.ProvinceCode,
                District = model.District,
                DistrictCode = model.DistrictCode,
                Ward = model.Ward,
                WardCode = model.WardCode
            };

            Branch newBranch = _db.Branches.Add(branch).Entity;
            _db.SaveChanges();

            return newBranch;

        }

        public List<Branch> GetAllBranch()
        {
            long storeId = _sessionService.GetStoreIdSession();
            return [.. _db.Branches.Where(branch => branch.StoreId == storeId)];
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

        public void UpdateBranch(long branchId, SaveBranchViewModel model)
        {
            long storeId = _sessionService.GetStoreIdSession();
            Branch branch = _db.Branches
                .Single(branch =>
                branch.Id.Equals(branchId) &&
                branch.StoreId.Equals(storeId)
                ) ?? throw new Exception("Chi nhánh không tồn tại");

            branch.IsActive = model.IsActive;
            branch.Province = model.Province;
            branch.ProvinceCode = model.ProvinceCode;
            branch.District = model.District;
            branch.DistrictCode = model.DistrictCode;
            branch.Ward = model.Ward;
            branch.WardCode = model.WardCode;
            branch.Email = model.Email;
            branch.Phone = model.Phone;
            branch.Name = model.Name;


            _db.Branches.Update(branch);
            _db.SaveChanges();

        }
    }
}
