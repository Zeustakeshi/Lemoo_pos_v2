using Lemoo_pos.Common.Enums;
using Lemoo_pos.Data;
using Lemoo_pos.Models.Entities;
using Lemoo_pos.Models.ViewModels;
using Lemoo_pos.Services.Interfaces;
using static System.Formats.Asn1.AsnWriter;

namespace Lemoo_pos.Services
{
    public class AuthorityService : IAuthorityService
    {

        private readonly AppDbContext _db;
        private readonly ISessionService _sessionService;

        public AuthorityService(AppDbContext db, ISessionService sessionService)
        {
            _db = db;
            _sessionService = sessionService;
        }

        public void InitNewStoreAuthority(Store store)
        {

            // create default role for new store
            Authority storeOwnerAuthority = CreateStoreOwnerAuthority(store);
            Authority warehouseStaffAuthority = CreateWarehouseStaffAuthority(store);
            Authority sellerAuthority = CreateSellerAuthority(store);

            _db.Authorities.AddRange(
                storeOwnerAuthority,
                warehouseStaffAuthority,
                sellerAuthority
             );

            _db.SaveChanges();

        }

        private Authority CreateStoreOwnerAuthority(Store store)
        {
            Authority storeOwnerAuthority = new()
            {
                Name = "Chủ cửa hàng",
                Store = store,
                StoreId = store.Id,
                Description = "Chủ cửa hàng có quyền hạn cao nhất trong cửa hàng.",
                Permissions = new List<AuthorityPermission>()
            };

            storeOwnerAuthority.Permissions.AddRange(Enum.GetValues<Common.Enums.PermissionType>()
                 .Select(permission => new AuthorityPermission
                 {
                     Authority = storeOwnerAuthority,
                     Permission = _db.Permissions.Single(p => p.Type == permission),
                     AuthorityId = storeOwnerAuthority.Id
                 }));

            return storeOwnerAuthority;
        }

        private Authority CreateWarehouseStaffAuthority(Store store)
        {
            // Tạo đối tượng Authority cho nhân viên kho
            Authority warehouseStaffAuthority = new()
            {
                Name = "Nhân viên kho",
                Store = store,
                StoreId = store.Id,
                Description = "Quyền quản lý và cân bằng kho.",
                Permissions = new List<AuthorityPermission>()
            };

            
            var warehousePermissions = new List<Common.Enums.PermissionType>
            {
                Common.Enums.PermissionType.VIEW_AUDIT_REPORT,   // Xem báo cáo kiểm kê
                Common.Enums.PermissionType.CREATE_AUDIT_REPORT,  // Tạo báo cáo kiểm kê
                Common.Enums.PermissionType.DELETE_AUDIT_REPORT,  // Xóa báo cáo kiểm kê
                Common.Enums.PermissionType.EDIT_AUDIT_REPORT,    // Chỉnh sửa báo cáo kiểm kê
                Common.Enums.PermissionType.BALANCE_WAREHOUSE,    // Cân bằng kho
                Common.Enums.PermissionType.EXPORT_AUDIT_FILE,    // Xuất báo cáo kiểm kê
                Common.Enums.PermissionType.IMPORT_AUDIT_FILE     // Nhập báo cáo kiểm kê
            };

            warehouseStaffAuthority.Permissions.AddRange(
                warehousePermissions.Select(permission => new AuthorityPermission
                {
                    Authority = warehouseStaffAuthority,
                    Permission = _db.Permissions.Single(p => p.Type == permission),
                    AuthorityId = warehouseStaffAuthority.Id
                })
            );

            return warehouseStaffAuthority;
        }

        private Authority CreateSellerAuthority(Store store)
        {
            Authority sellerAuthority = new()
            {
                Name = "Nhân viên bán hàng",
                Store = store,
                StoreId = store.Id,
                Description = "Bán hàng trực tiếp tại quầy, bán hàng online.",
                Permissions = []
            };

            var sellerPermissions = new List<Common.Enums.PermissionType>
            {
                Common.Enums.PermissionType.VIEW_ASSIGNED_ORDER,  // Xem đơn hàng được giao cho mình
                Common.Enums.PermissionType.VIEW_ALL_ORDER,      // Xem tất cả đơn hàng
                Common.Enums.PermissionType.ADD_ORDER,           // Thêm đơn hàng
                Common.Enums.PermissionType.EDIT_ORDER,          // Sửa đơn hàng
                Common.Enums.PermissionType.APPROVE_ORDER,       // Phê duyệt đơn hàng
                Common.Enums.PermissionType.CANCEL_ORDER,        // Hủy đơn hàng
                Common.Enums.PermissionType.VIEW_ASSIGNED_CUSTOMER,  // Xem khách hàng được giao
                Common.Enums.PermissionType.VIEW_ALL_CUSTOMER,   // Xem tất cả khách hàng
                Common.Enums.PermissionType.ADD_CUSTOMER,        // Thêm khách hàng
                Common.Enums.PermissionType.EDIT_CUSTOMER        // Chỉnh sửa thông tin khách hàng
            };

            sellerAuthority.Permissions.AddRange(
                sellerPermissions.Select(permission => new AuthorityPermission
                {
                    Authority = sellerAuthority,
                    Permission = _db.Permissions.Single(p => p.Type == permission),
                    AuthorityId = sellerAuthority.Id
                })
            );

            return sellerAuthority;
        }

        public List<Authority> GetAllAuthorities()
        {
            long storeId = _sessionService.GetStoreIdSession();

            List<Authority> authorities = [.. _db.Authorities
                .Where(authority => authority.StoreId.Equals(storeId))];

            return authorities;
        }

        public void CreateRole(CreateRoleViewModel model)
        {
            Store store = _sessionService.GetStoreSession();

            bool existed = _db.Authorities
                .Any(a => a.StoreId.Equals(store.Id) && 
                a.Name.Equals(model.Name)
            );
            
            if (existed )
            {
                throw new Exception("Vai trò đã tồn tại trong cửa hàng.");
            }


            Authority authority = new()
            {
                Name = model.Name,
                Store = store,
                StoreId = store.Id,
                Description = model.Description,
                Permissions = []
            };

            List<PermissionType> permissions = [];

            foreach (string permission in model.Permissions)
            {
                if (Enum.TryParse(permission, out PermissionType permissionEnum))
                {
                    permissions.Add(permissionEnum);
                }
                else
                {
                    string errorMessage = $"Không thể ánh xạ giá trị '{permission}' thành enum.";
                    Console.WriteLine(errorMessage);
                    throw new Exception(errorMessage);
                }
            }

            authority.Permissions.AddRange(
                permissions.Select(permission => new AuthorityPermission
                {
                    Authority = authority,
                    Permission = _db.Permissions.Single(p => p.Type == permission),
                    AuthorityId = authority.Id
                })
            );

            _db.Authorities.Add( authority );

            _db.SaveChanges();
        }
    }
}
