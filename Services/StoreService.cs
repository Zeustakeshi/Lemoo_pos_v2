using Lemoo_pos.Data;
using Lemoo_pos.Models.Entities;
using Lemoo_pos.Services.Interfaces;

namespace Lemoo_pos.Services
{
    public class StoreService : IStoreService
    {

        private readonly AppDbContext _db;
        private readonly HttpContext _httpContext;

        public StoreService(AppDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContext = httpContextAccessor.HttpContext;
        }

        public Store CreateNewStore(string name)
        {
            return  _db.Stores.Add(new() { Name = name }).Entity;
        }


    }
}
