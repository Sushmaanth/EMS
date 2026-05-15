namespace Entities
{
    public class Department
    {
        public int Id { get; set; }

        public string DepartmentName { get; set; }

        public ICollection<Employee> Employees { get; set; }
    }
}