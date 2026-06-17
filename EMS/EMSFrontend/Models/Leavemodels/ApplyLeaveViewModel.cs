using System.ComponentModel.DataAnnotations;

namespace EMSFrontend.Models.Leavemodels
{
    public class ApplyLeaveViewModel
    {
        [Required(ErrorMessage = "Leave Type is required")]
        public int LeaveType { get; set; }

        [Required(ErrorMessage = "Leave Start data is required")]
        public DateOnly StartDate { get; set; }

        [Required(ErrorMessage = "Leave End date is required")]
        public DateOnly EndDate { get; set; }

        [Required(ErrorMessage = "Reason for leave is required")]
        [StringLength(1000, ErrorMessage = "Reason cannot exceed more than 1000 characters")]
        public string Reason { get; set; }
    }
}
