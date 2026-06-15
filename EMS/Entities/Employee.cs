namespace Entities
{
    public class Employee
    {
        public int Id { get; set; }

        public string EmployeeCode { get; set; }
        public string Name { get; set; }

        public string Gender { get; set; }
        public DateOnly DateOfBirth { get; set; }

        public string EmailId { get; set; }

        public long Mobile { get; set; }

        public decimal Salary { get; set; }

        public DateOnly DateOfJoining { get; set; }

        public int? DepartmentId { get; set; }
        public Department Department { get; set; }

        public int? RoleId { get; set; }
        public Role? Role { get; set; }

        public int? ManagerId { get; set; }

        public Employee? Manager { get; set; }

        public ICollection<Employee> SubOrdinates { get; set; } = new List<Employee>();

        public User? User { get; set; }

        public ICollection<EmployeeDocument> EmployeeDocuments { get; set; }
    }
}
