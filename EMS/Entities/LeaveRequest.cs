using Entities.Enums;

namespace Entities
{
    public class LeaveRequest
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public int ManagerId { get; set; }
        public Employee Manager { get; set; }

        public LeaveType LeaveType { get; set; }

        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }

        public string Reason { get; set; }

        public LeaveStatus Status { get; set; } = LeaveStatus.Pending;

        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ReviewedAt { get; set; }

        public string? ManagerComments { get; set; }
    }
}
