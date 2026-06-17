using Dtos;
using Dtos.LeaveRequestDto;

namespace EMSBackend.Service.Abstraction
{
    public interface ILeaveRequestService
    {
        Task<ServiceResponseDto<LeaveRequestResponseDto>>ApplyLeaveAsync(int employeeId,ApplyLeaveDto dto);

        Task<ServiceResponseDto<LeaveRequestResponseDto>>ReviewLeaveAsync(int managerId,ReviewLeaveDto dto);

        Task<ServiceResponseDto<ICollection<LeaveRequestResponseDto>>>GetMyLeavesAsync(int employeeId);

        Task<ServiceResponseDto<ICollection<LeaveRequestResponseDto>>>GetTeamLeavesAsync(int managerId);
    }
}
