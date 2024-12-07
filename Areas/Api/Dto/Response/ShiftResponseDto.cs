using Lemoo_pos.Common.Enums;

namespace Lemoo_pos.Areas.Api.Dto
{
    public class ShiftResponseDto
    {
        public required long Id { get; set; }
        public required DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public required ShiftStatus Status { get; set; }
        public string? OpenNote { get; set; }
        public string? CloseNote { get; set; }
        public long TotalFunds { get; set; } = 0;
        public required StaffResponseDto Staff { get; set; }
    }
}