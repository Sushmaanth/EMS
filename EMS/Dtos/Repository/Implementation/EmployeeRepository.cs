using Dtos.Constants;
using Dtos.Repository.Abstraction;
using Dtos.Repository.Model;
using Entities;
using Entities.Data;
using Microsoft.EntityFrameworkCore;

namespace Dtos.Repository.Implementation
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _context;

        public EmployeeRepository(AppDbContext context)
        {
            _context = context;
        }
        public Employee Create(Employee employee)
        {
            _context.Employees.Add(employee);

            int result = _context.SaveChanges();

            if (result <= 0)
            {
                throw new Exception("Unable to create employee");
            }

            return employee;
        }

        public ICollection<Employee> View()
        {
            return _context.Employees
                     .AsNoTracking()
                     .Include(e => e.Role)
                     .Include(e => e.Department)
                     .Include(e => e.Manager)
                     .ToList();
        }

        public Employee Delete(Employee employee)
        {

            _context.Employees.Remove(employee);

            int result = _context.SaveChanges();

            if (result <= 0)
            {
                throw new Exception("Unable to delete employee");
            }

            return employee;        

        }

        public Employee Update(Employee employee)
        {
            _context.Employees.Update(employee);

            int result = _context.SaveChanges();

            if (result <= 0)
            {
                throw new Exception("Unable to update employee");
            }

            return employee;
        }

        public Employee? GetById(int id)
        {

            return _context.Employees.Include(e => e.Department).Include(e => e.Role).FirstOrDefault(e => e.Id == id);

        }

        //public IEnumerable<EmployeeDto> SearchEmployee(string? searchText)
        //{
        //    try
        //    {
        //        IQueryable<Employee> query = context.Employees;

        //        if (!string.IsNullOrWhiteSpace(searchText))
        //        {
        //            query = query.Where(e => e.Id.ToString().Contains(searchText) ||
        //            e.Name.ToLower().ToString().Contains(searchText));
        //        }

        //        var employees = query.Select(e => new EmployeeDto
        //        {
        //            Id = e.Id,
        //            Name = e.Name,
        //            EmailId = e.EmailId,
        //            Mobile = e.Mobile
        //        }).ToList();

        //        return employees;
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        public IQueryable<Employee> GetEmployees(string? searchText)
        {
            IQueryable<Employee> query =
                 _context.Employees.AsNoTracking().Include(e => e.Department); ;

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(e =>
                    e.EmployeeCode.ToString().Contains(searchText)
                    ||
                    e.Name.ToLower().Contains(searchText.ToLower()));
            }

            return query;
        }

        public IEnumerable<Department> GetDepartments()
        {
            return _context.Departments.ToList();

        }

        public async Task<Department?> GetByIdAsync(int id)
        {
            return await _context.Departments.FirstOrDefaultAsync(d => d.Id == id);
        }
        public async Task AddDocumentAsync(EmployeeDocument document)
        {
            await _context.EmployeeDocuments.AddAsync(document);
            await _context.SaveChangesAsync();
        }

        public EmployeeDashboardData GetDashboardData(int employeeId)
        {
            var employee = _context.Employees
                            .Include(e => e.Department)
                            .FirstOrDefault(e => e.Id == employeeId);

            var mandatoryDocumentType = _context.DocumentTypes
                                            .Where(d => d.IsMandatory)
                                            .ToList();

            var uploadedDocuments = _context.EmployeeDocuments
                                    .Where(d => d.EmployeeId == employeeId)
                                    .ToList();
            
            return new EmployeeDashboardData
            {
                Employee = employee,
                MandatoryDocumentTypes = mandatoryDocumentType,
                UploadedDocuments = uploadedDocuments
            };
        }

        public async Task<List<string>> GetExistingEmployeeCodesAsync(IEnumerable<string> employeeCodes)
        {
            return await _context.Employees.Where(e => employeeCodes.Contains(e.EmployeeCode)).Select(e => e.EmployeeCode).ToListAsync();
        }

        public async Task<List<string>> GetExistingEmailsAsync(IEnumerable<string> emails)
        {
            return await _context.Employees
                .Where(e => emails.Contains(e.EmailId))
                .Select(e => e.EmailId)
                .ToListAsync();
        }

        public async Task<List<long>> GetExistingMobilesAsync(IEnumerable<long> mobiles)
        {
            return await _context.Employees
                .Where(e => mobiles.Contains(e.Mobile))
                .Select(e => e.Mobile)
                .ToListAsync();
        }

        public async Task BulkInsertAsync(List<Employee> employees)
        {
            await _context.Employees.AddRangeAsync(employees);

            await _context.SaveChangesAsync();
        }

        public IEnumerable<Employee> GetManagers()
        {
            return _context.Employees.Where(e => e.RoleId == RoleConstants.Manager).ToList();
        }
    }
}
