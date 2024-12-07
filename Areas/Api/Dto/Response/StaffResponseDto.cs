namespace Lemoo_pos.Areas.Api.Dto
{
    public class StaffResponseDto
    {
        public required long Id { get; set; }
        public required string Name { get; set; }
        public string? Avatar { get; set; }
        public long? BranchId { get; set; }
    }
}