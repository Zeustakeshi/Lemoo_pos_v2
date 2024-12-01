using Lemoo_pos.Data;
using Lemoo_pos.Models.Entities;
using Lemoo_pos.Services.Interfaces;
using static System.Formats.Asn1.AsnWriter;

namespace Lemoo_pos.Services
{
    public class SessionService : ISessionService
    {

        private readonly HttpContext _httpContext;
        private readonly AppDbContext _db;

        public SessionService(IHttpContextAccessor httpContextAccessor, AppDbContext db) {
            _httpContext = httpContextAccessor.HttpContext;
            _db = db;
        }

        public long GetStoreIdSession()
        {
            return Convert.ToInt64(_httpContext.Session.GetString("StoreId"));
        }

        public Store GetStoreSession()
        {
            long storeId = Convert.ToInt64(_httpContext.Session.GetString("StoreId"));

            Store store = _db.Stores.Single(s => s.Id.Equals(storeId));

            return store ?? throw new Exception("Store not found. ");
        }

        public void SaveAuthSession (Account account, Store store)
        {
            Staff staff = _db.Staffs.Single(s => s.Id == account.Id);

            _httpContext.Session.SetString("AccountId", account.Id.ToString());
            _httpContext.Session.SetString("StaffId", staff.Id.ToString());
            _httpContext.Session.SetString("UserName", account.Name);
            _httpContext.Session.SetString("Email", account.Email);
            _httpContext.Session.SetString("Avatar", account.Avatar ?? "");
            _httpContext.Session.SetString("StoreName", store.Name);
            _httpContext.Session.SetString("StoreId", store.Id.ToString());
        }

        public Staff GetStaffSession ()
        {
            long staffId = Convert.ToInt64(_httpContext.Session.GetString("StaffId"));

            Staff staff = _db.Staffs.Single(s => s.Id.Equals(staffId));

            return staff ?? throw new Exception("Store not found. ");
        }

      
    }
}
