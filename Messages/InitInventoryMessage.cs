namespace Lemoo_pos.Messages
{
    public class InitInventoryMessage
    {
        public required long ProductVariantId { get; set; }
        public required long BranchId { get; set; }
        public required long Quantity { get; set; }
        public required long Available { get; set; }
        public required long StaffId { get; set; }


    }
}