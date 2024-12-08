using Lemoo_pos.Areas.Api.Dto;

namespace Lemoo_pos.Messages
{
    public class CreateOrderBatchMessage : CreateOrderDto
    {
        public long AccountId { get; set; }
    }
}