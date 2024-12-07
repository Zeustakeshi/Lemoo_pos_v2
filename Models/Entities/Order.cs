using Lemoo_pos.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace Lemoo_pos.Models.Entities
{
    public class Order : BaseEntity
    {
        public required Store Store { get; set; }
        public required long StoreId { get; set; }
        public required Staff Staff { get; set; }
        public required long StaffId { get; set; }
        public Customer? Customer { get; set; }
        public long? CustomerId { get; set; }
        public required long Total { get; set; }
        public required PaymentMethod PaymentMethod { get; set; } = PaymentMethod.CASH;
        public required long Change { get; set; } = 0;
        public string? Description { get; set; }
        public Shift? Shift { get; set; }
        public long? ShiftId { get; set; }
    }
}