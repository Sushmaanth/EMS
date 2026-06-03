using Entities;

namespace Dtos.Repository.Abstraction
{
    public interface IEmployeeRepository
    {
        //IEnumerable<EmployeeDto> SearchEmployee(string searchText);

        Employee Create(Employee employee);

        Employee? GetById(int id);

        ICollection<Employee> View();

        Employee? Update(Employee employee);

        Employee? Delete(Employee employee);
        IQueryable<Employee> GetEmployees(string? searchText);

        ICollection<Department> GetDepartments();

        Task AddDocumentAsync(EmployeeDocument document);


    }
}
