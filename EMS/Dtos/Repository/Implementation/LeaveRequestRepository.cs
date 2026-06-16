using Dtos.Repository.Abstraction;
using Entities;
using Entities.Data;
using Microsoft.EntityFrameworkCore;

namespace Dtos.Repository.Implementation
{
    public class LeaveRequestRepository : ILeaveRequestRepository
    {
        private readonly AppDbContext _context;
        public LeaveRequestRepository(AppDbContext context) => _context = context;

        public async Task<LeaveRequest>ApplyLeaveRequestAsync(LeaveRequest leaveRequest)
        {
            await _context.LeaveRequests.AddAsync(leaveRequest);

            await _context.SaveChangesAsync();

            return leaveRequest;
        }

        public async Task<LeaveRequest?>GetLeaveRequestByIdAsync(int id)
        {
            return await _context.LeaveRequests
                .Include(l => l.Employee)
                .Include(l => l.Manager)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<ICollection<LeaveRequest>>GetEmployeesLeaveRequestsAsync(int employeeId)
        {
            return await _context.LeaveRequests
                .Include(l => l.Employee)
                .Include(l => l.Manager)
                .Where(l => l.EmployeeId == employeeId)
                .OrderByDescending(l => l.AppliedAt)
                .ToListAsync();
        }

        public async Task<ICollection<LeaveRequest>>GetByManagerAsync(int managerId)
        {
            return await _context.LeaveRequests
                .Include(l => l.Employee)
                .Include(l => l.Manager)
                .Where(l => l.ManagerId == managerId)
                .OrderByDescending(l => l.AppliedAt)
                .ToListAsync();
        }

        public async Task SaveLeaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
