using Lemoo_pos.Areas.Api.Dto;

namespace Lemoo_pos.Services.Interfaces
{
    public interface IOrderService
    {
        OrderResponseDto CreateOrder(CreateOrderDto dto, long storeId, long accountId);
        void CreateOrderBatch(List<CreateOrderDto> dtos, long storeId, long accountId);
    }
}
