using Dtos.Repository.Model;
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

        EmployeeDashboardData GetDashboardData(int employeeId);

        Task<List<string>> GetExistingEmployeeCodesAsync(IEnumerable<string> employeeCodes);

        Task BulkInsertAsync(List<Employee> employees);

        Task<List<string>> GetExistingEmailsAsync(IEnumerable<string> emails);

        Task<List<long>> GetExistingMobilesAsync(IEnumerable<long> mobiles);
    }
}
