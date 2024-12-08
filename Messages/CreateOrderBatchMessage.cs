using Lemoo_pos.Areas.Api.Dto;

namespace Lemoo_pos.Messages
{
    public class CreateOrderBatchMessage : CreateOrderDto
    {
        public required long AccountId { get; set; }
        public required long StoreId { get; set; }
    }
}