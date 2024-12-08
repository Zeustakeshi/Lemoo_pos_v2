using Lemoo_pos.Areas.Api.Dto;

namespace Lemoo_pos.Areas.Api.Services.Interfaces
{
    public interface IOrderServiceApi
    {
        Task<OrderResponseDto> CreateOrder(CreateOrderDto dto, long storeId, long accountId);
        void CreateOrderBatch(List<CreateOrderDto> dtos, long storeId, long accountId);
        Task UpdateOrderCustomer(long orderId, long customerId);
        Task CreateOrderBatchAsync(List<CreateOrderDto> dtos, long storeId, long accountId);
        Task<List<OrderResponseDto>> GetAllOrder(long storeId);
    }
}
