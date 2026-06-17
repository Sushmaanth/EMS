using System.ComponentModel.DataAnnotations;

namespace EMSFrontend.Models.Leavemodels
{
    public class LeaveRequestViewModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string LeaveType { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int TotalDays { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public DateTime AppliedAt { get; set; }
        public string? ManagerComments { get; set; }
    }
}
