using System.Text.Json.Serialization;

namespace EMSFrontend.Models.Leavemodels
{
    public class ReviewLeaveViewModel
    {
        public int LeaveRequestId { get; set; }

        public bool IsApproved { get; set; }
        [System.Text.Json.Serialization.JsonPropertyName("managerComments")]
        public string? ManagerComments { get; set; }

    }
}
