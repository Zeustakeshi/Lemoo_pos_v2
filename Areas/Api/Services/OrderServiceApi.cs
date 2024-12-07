using Lemoo_pos.Areas.Api.Dto;
using Lemoo_pos.Areas.Api.Services.Interfaces;
using Lemoo_pos.Common.Enums;
using Lemoo_pos.Data;
using Lemoo_pos.Models.Entities;
using Lemoo_pos.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Lemoo_pos.Areas.Api.Services
{
    public class OrderServiceApi : IOrderServiceApi
    {
        private readonly AppDbContext _db;
        private readonly IElasticsearchService _elasticsearchService;

        public OrderServiceApi(AppDbContext db, IElasticsearchService elasticsearchService)
        {
            _elasticsearchService = elasticsearchService;
            _db = db;
        }

        public async Task<OrderResponseDto> CreateOrder(CreateOrderDto dto, long storeId, long accountId)
        {
            Store store = _db.Stores.FirstOrDefault(s => s.Id == storeId) ??
             throw new KeyNotFoundException($"Store id = {storeId} not found ");

            Staff staff = _db.Staffs
                .FirstOrDefault(s => s.Account.Id == accountId) ?? throw new KeyNotFoundException($"Staff with accountId= {accountId} not found ");

            PaymentMethod paymentMethod;

            if (Enum.TryParse(dto.PaymentMethod, out PaymentMethod method))
            {
                paymentMethod = method;
            }
            else
            {
                string errorMessage = $"Không thể ánh xạ giá trị '{dto.PaymentMethod}' thành enum.";
                Console.WriteLine(errorMessage);
                throw new ArgumentException(errorMessage);
            }

            Order order = new()
            {
                Staff = staff,
                StaffId = staff.Id,
                Store = store,
                StoreId = storeId,
                Change = dto.Change,
                PaymentMethod = paymentMethod,
                Total = dto.Total,
                Description = dto.Description
            };

            if (dto.CustomerId != null)
            {
                Customer customer = _db.Customers.FirstOrDefault(c => c.Id == dto.CustomerId) ?? throw new KeyNotFoundException("Store doesn't exist");
                order.Customer = customer;
            }

            Order newOrder = _db.Orders.Add(order).Entity;

            List<OrderItem> orderItems = [];

            foreach (var item in dto.Items)
            {
                OrderItem orderItem = new()
                {
                    Order = newOrder,
                    OrderId = newOrder.Id,
                    Quantity = item.Quantity,
                    Total = item.Total,
                    Type = item.Type,
                    ServiceName = item.ServiceName
                };

                if (item.ProductId == null)
                {
                    orderItems.Add(orderItem);
                    continue;
                }

                ProductVariant product = _db.ProductVariants
                    .FirstOrDefault(p => p.Id == item.ProductId) ??
                    throw new KeyNotFoundException($"Product with {item.ProductId} doesn't exist");

                orderItem.ProductVariant = product;
                orderItem.ProductVariantId = product.Id;
                orderItems.Add(orderItem);

                Inventory inventory = _db.Inventories
                    .FirstOrDefault(inventory => inventory.Branch.Id == dto.BranchId && inventory.ProductVariantId == product.Id) ??
                    throw new KeyNotFoundException("Inventory not found");

                if (!product.AllowNegativeInventory && inventory.Available < item.Quantity && inventory.Quantity < item.Quantity)
                {
                    throw new ArgumentException("This product not allow save negative inventory");
                }

                inventory.Available -= item.Quantity;
                inventory.Quantity -= item.Quantity;

                _db.Inventories.Update(inventory);
                await _elasticsearchService.SaveDocumentById(new { Quantity = inventory.Available }, product.Id.ToString(), "products");

            }
            _db.OrderItems.AddRange(orderItems);
            _db.SaveChanges();

            await UpdateOrderShiftAsync(newOrder, staff.Id);

            List<OrderItemDto> orderItemResponse = [];
            foreach (var item in orderItems)
            {
                orderItemResponse.Add(new()
                {
                    ProductId = item.ProductVariantId,
                    Quantity = item.Quantity,
                    Total = item.Total,
                });
            }
            return new()
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                Items = orderItemResponse,
                PaymentMethod = order.PaymentMethod.ToString(),
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
            };
        }

        public void CreateOrderBatch(List<CreateOrderDto> dtos, long storeId, long accountId)
        {
            foreach (var dto in dtos)
            {
                CreateOrder(dto, storeId, accountId);
            }
        }

        private async Task UpdateOrderShiftAsync(Order order, long staffId)
        {
            Shift shift = _db.Shifts.FirstOrDefault(shift => shift.StaffId == staffId && shift.Status.Equals(ShiftStatus.OPENING)) ??
                throw new KeyNotFoundException($"Staff {staffId} currently has no shifts shift.");
            order.Shift = shift;
            order.ShiftId = shift.Id;
            _db.Orders.Update(order);
            await _db.SaveChangesAsync();
        }

    }

}