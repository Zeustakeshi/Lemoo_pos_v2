using Elasticsearch.Net;
using Lemoo_pos.Common.Enums;
using Lemoo_pos.Data;
using Lemoo_pos.Messages;
using Lemoo_pos.Models.Entities;
using Lemoo_pos.Models.ViewModels;
using Lemoo_pos.Services.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using static System.Formats.Asn1.AsnWriter;

namespace Lemoo_pos.Services
{
    public class AuthorityService : IAuthorityService
    {

        private readonly AppDbContext _db;
        private readonly ISessionService _sessionService;
        private readonly IPublishEndpoint _publishEndpoint;

        public AuthorityService(AppDbContext db, ISessionService sessionService, IPublishEndpoint publishEndpoint)
        {
            _db = db;
            _sessionService = sessionService;
            _publishEndpoint = publishEndpoint;
        }

        public async Task SavePermissionBatch(long authorityId, List<PermissionType> permissions)
        {

            Authority authority = _db.Authorities.FirstOrDefault(a => a.Id == authorityId) ?? throw new Exception($"Authority {authorityId} not found");
            List<AuthorityPermission> authorityPermissions = [];
            foreach (var permission in permissions)
            {
                authorityPermissions.Add(new()
                {
                    Authority = authority,
                    Permission = _db.Permissions.FirstOrDefault(p => p.Type == permission) ?? throw new KeyNotFoundException($"Permission {permission} not found"),
                    AuthorityId = authority.Id
                });
            }
            await _db.AuthorityPermissions.AddRangeAsync(authorityPermissions);
        }

        public async Task<Authority> InitNewStoreAuthority(Store store)
        {
            // create default role for new store
            Authority storeOwnerAuthority = await CreateStoreOwnerAuthority(store);

            await _publishEndpoint.Publish(new CreateAuthorityMessage()
            {
                Name = "Nhân viên kho",
                Description = "Quyền quản lý và cân bằng kho.",
                StoreId = store.Id,
                Permissions = [
                    PermissionType.VIEW_AUDIT_REPORT.GetStringValue(),   // Xem báo cáo kiểm kê
                    PermissionType.CREATE_AUDIT_REPORT.GetStringValue(),  // Tạo báo cáo kiểm kê
                    PermissionType.DELETE_AUDIT_REPORT.GetStringValue(),  // Xóa báo cáo kiểm kê
                    PermissionType.EDIT_AUDIT_REPORT.GetStringValue(),    // Chỉnh sửa báo cáo kiểm kê
                    PermissionType.BALANCE_WAREHOUSE.GetStringValue(),    // Cân bằng kho
                    PermissionType.EXPORT_AUDIT_FILE.GetStringValue(),    // Xuất báo cáo kiểm kê
                    PermissionType.IMPORT_AUDIT_FILE.GetStringValue()    // Nhập báo cáo kiểm kê
                ]
            });

            await _publishEndpoint.Publish(new CreateAuthorityMessage()
            {
                Name = "Nhân viên bán hàng",
                Description = "Bán hàng trực tiếp tại quầy, bán hàng online.",
                StoreId = store.Id,
                Permissions = [
                   PermissionType.VIEW_ASSIGNED_ORDER.GetStringValue(),  // Xem đơn hàng được giao cho mình
                    PermissionType.VIEW_ALL_ORDER.GetStringValue(),      // Xem tất cả đơn hàng
                    PermissionType.ADD_ORDER.GetStringValue(),           // Thêm đơn hàng
                    PermissionType.EDIT_ORDER.GetStringValue(),          // Sửa đơn hàng
                    PermissionType.APPROVE_ORDER.GetStringValue(),       // Phê duyệt đơn hàng
                    PermissionType.CANCEL_ORDER.GetStringValue(),        // Hủy đơn hàng
                    PermissionType.VIEW_ASSIGNED_CUSTOMER.GetStringValue(),  // Xem khách hàng được giao
                    PermissionType.VIEW_ALL_CUSTOMER.GetStringValue(),   // Xem tất cả khách hàng
                    PermissionType.ADD_CUSTOMER.GetStringValue(),        // Thêm khách hàng
                    PermissionType.EDIT_CUSTOMER.GetStringValue()       // Chỉnh sửa thông tin khách hàng
              ]
            });

            return storeOwnerAuthority;
        }

        private async Task<Authority> CreateStoreOwnerAuthority(Store store)
        {
            return await CreateAuthorityAsync(new()
            {
                Name = "Chủ cửa hàng",
                Description = "Chủ cửa hàng có quyền hạn cao nhất trong cửa hàng.",
                Permissions = [],

            }, store.Id, true);
        }



        public List<Authority> GetAllAuthorities()
        {
            long storeId = _sessionService.GetStoreIdSession();

            List<Authority> authorities = [.. _db.Authorities
                .Where(authority => authority.StoreId.Equals(storeId))];

            return authorities;
        }

        public async Task CreateRole(CreateRoleViewModel model)
        {
            long storeId = _sessionService.GetStoreIdSession();
            await CreateAuthorityAsync(model, storeId);
        }

        public async Task<Authority> CreateAuthorityAsync(CreateRoleViewModel model, long storeId, bool? hasAllPermission = false)
        {

            Store store = await _db.Stores.FirstOrDefaultAsync(s => s.Id == storeId) ?? throw new Exception($"Store {storeId} not found");

            bool existed = await _db.Authorities
                .AnyAsync(a => a.StoreId.Equals(store.Id) &&
                a.Name.Equals(model.Name)
            );

            if (existed)
            {
                throw new Exception("Vai trò đã tồn tại trong cửa hàng.");
            }


            Authority authority = new()
            {
                Name = model.Name,
                Store = store,
                StoreId = store.Id,
                Description = model.Description,
                Permissions = [],
                HasAllPermission = hasAllPermission
            };

            var newAuthority = _db.Authorities.Add(authority).Entity;

            await _db.SaveChangesAsync();

            foreach (string permission in model.Permissions)
            {
                if (Enum.TryParse(permission, out PermissionType permissionEnum))
                {
                    await _publishEndpoint.Publish(new CreateAuthorityPermissionMessage()
                    {
                        AuthorityId = newAuthority.Id,
                        permissionType = permissionEnum
                    });
                }
                else
                {
                    string errorMessage = $"Không thể ánh xạ giá trị '{permission}' thành enum.";
                    Console.WriteLine(errorMessage);
                    throw new Exception(errorMessage);
                }
            }

            return newAuthority;
        }
    }
}
