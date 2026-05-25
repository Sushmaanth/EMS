using Dtos;

namespace EMSBackend.Service.Abstraction
{
    public interface IEmployeeService
    {
        ServiceResponseDto<CreateEmployeeDto>Create(CreateEmployeeDto dto);

        ServiceResponseDto<ICollection<EmployeeDto>>View();

        ServiceResponseDto<EmployeeDto>Update(int id, EmployeeDto dto);

        ServiceResponseDto<EmployeeDto>Delete(int id);

        ServiceResponseDto<EmployeeDto>GetById(int id);

        ServiceResponseDto<PagenationDto<EmployeeDto>>GetEmployees(string? searchText,int pageNumber,int pageSize);

        ServiceResponseDto<ICollection<DepartmentDto>>GetDepartments();
    }
}
