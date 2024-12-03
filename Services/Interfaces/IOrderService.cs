using Lemoo_pos.Models.Dto;

namespace Lemoo_pos.Services.Interfaces
{
    public interface IOrderService
    {
        OrderResponseDto CreateOrder (CreateOrderDto dto, long storeId, long accountId);

    }
}
