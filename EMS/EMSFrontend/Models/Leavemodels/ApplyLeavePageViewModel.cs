namespace EMSFrontend.Models.Leavemodels
{
    public class ApplyLeavePageViewModel
    {
        public ApplyLeaveViewModel ApplyLeave { get; set; } = new();

        public ICollection<LeaveRequestViewModel> MyLeaves { get; set; }= new List<LeaveRequestViewModel>();
    }
}
