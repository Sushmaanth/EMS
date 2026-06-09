using AutoMapper;
using Dtos;
using Entities;

namespace EMSBackend.Mapper
{
    public class EmployeeMappingProfile:Profile
    {
        public EmployeeMappingProfile()
        {
            // -- Employee -> EmployeeDto 
            CreateMap<Employee, EmployeeDto>()
               .ForMember(
                   dest => dest.DepartmentName,
                   opt => opt.MapFrom(src =>
                       src.Department != null ? src.Department.DepartmentName : null));

            // -- EmployeeDto -> Employee - for update
            CreateMap<EmployeeDto, Employee>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            // -- Employee -> CreateEmployeeDto
            CreateMap<Employee, CreateEmployeeDto>();

            // -- CreateEmployeeDto -> Employee
            CreateMap<CreateEmployeeDto, Employee>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
