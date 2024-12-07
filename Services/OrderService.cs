using Elasticsearch.Net;
using Lemoo_pos.Areas.Api.Dto;
using Lemoo_pos.Common.Enums;
using Lemoo_pos.Data;
using Lemoo_pos.Models.Entities;
using Lemoo_pos.Models.ViewModels;
using Lemoo_pos.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Formats.Asn1;
using System.Security;

namespace Lemoo_pos.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _db;
        private readonly IElasticsearchService _elasticsearchService;
        private readonly ISessionService _sessionService;

        public OrderService(AppDbContext db, IElasticsearchService elasticsearchService, ISessionService sessionService)
        {
            _elasticsearchService = elasticsearchService;
            _db = db;
            _sessionService = sessionService;
        }


        public List<PaymentMethodAnaliticsViewModel> GetPaymentMethodAnalytics()
        {
            long storeId = _sessionService.GetStoreIdSession();

            var orderGroups = _db.Orders
                    .Where(order => order.StoreId == storeId)
                    .GroupBy(order => order.PaymentMethod);

            List<PaymentMethodAnaliticsViewModel> responses = [];

            foreach (var group in orderGroups)
            {
                responses.Add(new()
                {
                    PaymentMethod = group.Key.GetStringValue(),
                    TotalOrder = group.Count()
                });
            }

            return responses;
        }


        public List<SalesOverviewViewModel> GetSalesOverview()
        {
            var now = DateTime.UtcNow.Date; // Chỉ lấy phần ngày
            var sevenDaysAgo = now.AddDays(-7);

            // Tạo danh sách các ngày từ sevenDaysAgo đến now
            var allDates = Enumerable.Range(0, 8) // 8 ngày: từ ngày 0 đến ngày 7
                .Select(offset => sevenDaysAgo.AddDays(offset))
                .ToList();

            // Truy vấn doanh thu từ cơ sở dữ liệu
            var salesData = _db.Orders
                .Where(order => order.CreatedAt.Date >= sevenDaysAgo && order.CreatedAt.Date <= now)
                .GroupBy(order => order.CreatedAt.Date)
                .Select(group => new SalesOverviewViewModel()
                {
                    Date = group.Key,
                    TotalRevenue = group.Sum(order => order.Total), // Tổng doanh thu
                })
                .ToList(); // Chuyển thành danh sách

            // Hợp nhất danh sách đầy đủ với kết quả truy vấn
            var result = allDates.Select(date =>
            {
                // Tìm doanh thu theo ngày, nếu không có thì mặc định là 0
                var existingData = salesData.FirstOrDefault(stat => stat.Date == date);
                return existingData ?? new SalesOverviewViewModel
                {
                    Date = date,
                    TotalRevenue = 0
                };
            })
            .OrderBy(stat => stat.Date) // Sắp xếp theo ngày
            .ToList();

            return result;
        }
    }


}
