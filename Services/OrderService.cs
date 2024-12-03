using Lemoo_pos.Common.Enums;
using Lemoo_pos.Data;
using Lemoo_pos.Models.Dto;
using Lemoo_pos.Models.Entities;
using Lemoo_pos.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security;

namespace Lemoo_pos.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _db;
        public OrderService(AppDbContext db) {
            _db = db;
        }

        public OrderResponseDto CreateOrder(CreateOrderDto dto, long storeId, long accountId)
        {
            Store store = _db.Stores.Single(s => s.Id == storeId) ?? throw new Exception("Store doesn't exist");

            Staff staff = _db.Staffs
                .Include(s => s.Account)
                .Single(s => s.Account.Id == accountId) ?? throw new Exception("Staff doesn't exist");

            PaymentMethod paymentMethod;


            if (Enum.TryParse(dto.PaymentMethod, out PaymentMethod method))
            {
                paymentMethod = method;
            }
            else
            {
                string errorMessage = $"Không thể ánh xạ giá trị '{dto.PaymentMethod}' thành enum.";
                Console.WriteLine(errorMessage);
                throw new Exception(errorMessage);
            }

            Order order = new ()
            {
                Staff = staff,
                StaffId = staff.Id,
                Store = store,
                StoreId = storeId,
                Change = dto.Change,
                PaymentMethod = paymentMethod,
                Total = dto.Total,
            };

            if (dto.CustomerId != null)
            {
                Customer customer = _db.Customers.Single(c => c.Id == dto.CustomerId) ?? throw new Exception("Store doesn't exist");
                order.Customer = customer;
            }

            Order newOrder = _db.Orders.Add(order).Entity;

            List<OrderItem> orderItems = [];

            foreach (var item in dto.Items)
            {

                ProductVariant product = _db.ProductVariants.Single(p => p.Id == item.ProductId) ??
                    throw new Exception($"Product with {item.ProductId} doesn't exist");

                orderItems.Add(new()
                {
                    Order = newOrder,
                    OrderId = newOrder.Id,
                    ProductVariant = product,
                    ProductVariantId = product.Id,
                    Quantity = item.Quantity,
                    Total = item.Total,
                });

                if (item.Quantity < product.Quantity)   product.Quantity -= item.Quantity; 

                _db.ProductVariants.Update(product);
            }

            _db.OrderItems.AddRange(orderItems);

            _db.SaveChanges();

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

            return new ()
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                Items = orderItemResponse,
                PaymentMethod = order.PaymentMethod.ToString(),
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
            };
        }
    }
}
