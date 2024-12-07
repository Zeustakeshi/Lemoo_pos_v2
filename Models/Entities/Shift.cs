using Lemoo_pos.Common.Enums;

namespace Lemoo_pos.Models.Entities
{
    public class Shift : BaseEntity
    {
        public required DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public required Staff Staff { get; set; }
        public required long StaffId { get; set; }
        public string? OpenNote { get; set; }
        public string? CloseNote { get; set; }
        public ShiftStatus Status { get; set; }
        public required Store Store { get; set; }
        public required long StoreId { get; set; }
        public List<Order> Orders { get; set; } = [];
    }
}