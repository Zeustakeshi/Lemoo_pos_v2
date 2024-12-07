using System.Runtime.ConstrainedExecution;

namespace Lemoo_pos.Areas.Api.Dto
{
    public class CustomerSearchDto
    {
        public required long Id { get; set; }
        public required string Name { get; set; }
        public required string Keyword { get; set; }
        public required long StoreId { get; set; }
        public required string Phone { get; set; }
        public string? Email { get; set; }
    }
}
