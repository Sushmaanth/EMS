
using Entities;

namespace Dtos.Repository.Abstraction
{
    public interface ILeaveRequestRepository
    {
        // Create leave request
        Task<LeaveRequest> ApplyLeaveRequestAsync(LeaveRequest leaveRequest);

        // Get leave request by ID
        Task<LeaveRequest?> GetLeaveRequestByIdAsync(int id);

        // Get all leave requests by employee
        Task<ICollection<LeaveRequest>>GetEmployeesLeaveRequestsAsync(int employeeId);

        // Manager sees all employee leave requests
        Task<ICollection<LeaveRequest>>GetByManagerAsync(int managerId);

        Task SaveLeaveAsync();
    }
}
