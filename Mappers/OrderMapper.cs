using Elasticsearch.Net;
using Lemoo_pos.Areas.Api.Dto;
using Lemoo_pos.Models.Entities;

namespace Lemoo_pos.Mappers
{
    public class OrderMapper
    {
        public static OrderResponseDto OrderToOrderResponseDto(Order order, List<OrderItemDto> orderItems)
        {
            return new OrderResponseDto()
            {
                Id = order.Id,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                PaymentMethod = order.PaymentMethod.GetStringValue(),
                Items = orderItems,
                Total = order.Total
            };
        }

        public static OrderItemDto OrderItemToOrderItemDto(OrderItem orderItem, string? productName)
        {
            return new OrderItemDto()
            {
                Type = orderItem.Type,
                Quantity = orderItem.Quantity,
                Total = orderItem.Total,
                ProductId = orderItem.ProductVariantId,
                ServiceName = orderItem.ServiceName,
                ProductName = productName
            };
        }
    }
}