namespace Lemoo_pos.Models.Dto
{
    public class CustomerSearchResponseDto
    {
        public required long Id { get; set; }
        public required string Name { get; set; }
        public required string Phone { get; set; }
        public string? Email { get; set; }
    }
}