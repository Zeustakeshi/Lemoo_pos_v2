using Lemoo_pos.Data;
using Lemoo_pos.Models.Entities;
using Lemoo_pos.Models.ViewModels;
using Lemoo_pos.Services.Interfaces;

namespace Lemoo_pos.Services
{
    public class StoreService : IStoreService
    {

        private readonly AppDbContext _db;
        private readonly ISessionService _sessionService;

        public StoreService(AppDbContext db, IHttpContextAccessor httpContextAccessor, ISessionService sessionService)
        {
            _db = db;
            _sessionService = sessionService;
        }

        public async Task<Store> CreateNewStore(string name)
        {
            Store store = _db.Stores.Add(new() { Name = name }).Entity;
            await _db.SaveChangesAsync();
            return store;
        }

        public StoreOverviewViewModel GetStoreOverview()
        {
            long storeId = _sessionService.GetStoreIdSession();

            List<Order> orders = [.. _db.Orders.Where(o => o.StoreId == storeId)];

            return new()
            {
                TotalProducts = _db.Products.Where(p => p.Store.Id == storeId).Count(),
                TotalSales = orders.Count,
                Revenue = orders.Aggregate(0L, (acc, curr) => acc + curr.Total)
            };
        }

    }
}
