namespace Lemoo_pos.Areas.Api.Dto
{
    public class BranchResponseDto
    {
        public required long Id { get; set; }

        public required string Name { get; set; }

        public string? Address { get; set; }
    }
}
