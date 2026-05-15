using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dtos.Repository.Abstraction
{
    public interface IEmployeeRepository<T> where T: class
    {
        IEnumerable<EmployeeDto> SearchEmployee(string searchText);

        PagenationDto<EmployeeDto> GetEmployees(string? searchText,int pageNumber, int pageSize);
    }
}
