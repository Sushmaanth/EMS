

namespace Dtos.LeaveRequestDto
{
    public class ReviewLeaveDto
    {
        public int LeaveRequestId { get; set; }

        public bool IsApproved { get; set; }

        public string Comments { get; set; }
    }
}
