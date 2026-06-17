using EMSFrontend.Models.Leavemodels;

namespace EMSFrontend.Api.Abstraction
{
    public interface ILeaveApiRequest
    {
        Task<LeaveRequestViewModel> ApplyLeaveAsync(ApplyLeaveViewModel model);

        Task<ICollection<LeaveRequestViewModel>>GetMyLeavesAsync();

        Task<IEnumerable<LeaveRequestViewModel>> GetTeamLeavesAsync();

        Task<ReviewLeaveViewModel> ReviewLeaveAsync(ReviewLeaveViewModel model);
    }
}
