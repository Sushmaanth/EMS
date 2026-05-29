using Dtos;
using Dtos.Repository.Abstraction;
using Dtos.Repository.Implementation;
using EMSBackend.Service.Abstraction;
using Entities;
using System.Reflection.Metadata;

namespace EMSBackend.Service.Implementation
{
    public class EmployeeService : IEmployeeService
    {

        private readonly IEmployeeRepository _employeeRepository;
        private readonly IBlobService _blobService;
        private readonly IDocumentRepository _documentRepository;

        public EmployeeService(IEmployeeRepository repository,IBlobService blobService,IDocumentRepository documentRepository)
        {
            _employeeRepository = repository;
            _blobService = blobService;
            _documentRepository = documentRepository;
        }

        public ServiceResponseDto<CreateEmployeeDto> Create(CreateEmployeeDto dto)
        {
            Employee employee = new()
            {
                Name = dto.Name,
                Gender = dto.Gender,
                DateOfBirth = dto.DateOfBirth,
                EmailId = dto.EmailId,
                Mobile = dto.Mobile,
                Salary = dto.Salary,
                DateOfJoining = dto.DateOfJoining,
                DepartmentId = dto.DepartmentId
            };

            var createdEmployee = _employeeRepository.Create(employee);

            CreateEmployeeDto responseDto = new()
            {
                Id = createdEmployee.Id,
                Name = createdEmployee.Name,
                Gender = createdEmployee.Gender,
                DateOfBirth = createdEmployee.DateOfBirth,
                EmailId = createdEmployee.EmailId,
                Mobile = createdEmployee.Mobile,
                Salary = createdEmployee.Salary,
                DateOfJoining = createdEmployee.DateOfJoining,
                DepartmentId = createdEmployee.DepartmentId
            };

            return new ServiceResponseDto<CreateEmployeeDto>
            {
                Success = true,
                Message = "Employee created successfully",
                Data = responseDto
            };
        }


        public ServiceResponseDto<ICollection<EmployeeDto>> View()
        {

            var employees = _employeeRepository.View();

            var employeeDtos = employees
               .Select(e => new EmployeeDto
               {
                   Id = e.Id,
                   Name = e.Name,
                   Gender = e.Gender,
                   DateOfBirth = e.DateOfBirth,
                   EmailId = e.EmailId,
                   Mobile = e.Mobile,
                   Salary = e.Salary,
                   DateOfJoining = e.DateOfJoining,
                   DepartmentId = e.DepartmentId,
                   DepartmentName =
                        e.Department != null
                            ? e.Department.DepartmentName
                            : null

               }).ToList();

            return new ServiceResponseDto<ICollection<EmployeeDto>>
            {
                Success = true,
                Message = "Employees fetched successfully",
                Data = employeeDtos
            };
        }


        public ServiceResponseDto<EmployeeDto> Update(int id, EmployeeDto dto)
        {

            var foundEmployee = _employeeRepository.GetById(id);

            if (foundEmployee == null)
            {
                return new ServiceResponseDto<EmployeeDto>
                {
                    Success = false,
                    Message = "Employee not found"
                };
            }

            foundEmployee.Name = dto.Name;
            foundEmployee.Gender = dto.Gender;
            foundEmployee.DateOfBirth = dto.DateOfBirth;
            foundEmployee.EmailId = dto.EmailId;
            foundEmployee.Mobile = dto.Mobile;
            foundEmployee.Salary = dto.Salary;
            foundEmployee.DateOfJoining = dto.DateOfJoining;
            foundEmployee.DepartmentId = dto.DepartmentId;

            var updatedEmployee = _employeeRepository.Update(foundEmployee);

            EmployeeDto employeeDto = new()
            {
                Id = updatedEmployee.Id,
                Name = updatedEmployee.Name,
                Gender = updatedEmployee.Gender,
                DateOfBirth = updatedEmployee.DateOfBirth,
                EmailId = updatedEmployee.EmailId,
                Mobile = updatedEmployee.Mobile,
                Salary = updatedEmployee.Salary,
                DateOfJoining = updatedEmployee.DateOfJoining,
                DepartmentId = updatedEmployee.DepartmentId
            };

            return new ServiceResponseDto<EmployeeDto>
            {
                Success = true,
                Message = "Employee updated successfully",
                Data = employeeDto
            };
        }


        public ServiceResponseDto<EmployeeDto> Delete(int id)
        {

            var foundEmployee = _employeeRepository.GetById(id);

            if (foundEmployee == null)
            {
                return new ServiceResponseDto<EmployeeDto>
                {
                    Success = false,
                    Message = "Employee not found"
                };
            }

            var deletedEmployee = _employeeRepository.Delete(foundEmployee);

            EmployeeDto dto = new()
            {
                Id = deletedEmployee.Id,
                Name = deletedEmployee.Name,
                Gender = deletedEmployee.Gender,
                DateOfBirth = deletedEmployee.DateOfBirth,
                EmailId = deletedEmployee.EmailId,
                Mobile = deletedEmployee.Mobile,
                Salary = deletedEmployee.Salary,
                DateOfJoining = deletedEmployee.DateOfJoining,
                DepartmentId = deletedEmployee.DepartmentId
            };

            return new ServiceResponseDto<EmployeeDto>
            {
                Success = true,
                Message = "Employee deleted successfully",
                Data = dto
            };
        }


        public ServiceResponseDto<EmployeeDto> GetById(int id)
        {
            var employee = _employeeRepository.GetById(id);

            if (employee == null)
            {
                return new ServiceResponseDto<EmployeeDto>
                {
                    Success = false,
                    Message = "Employee not found"
                };
            }

            EmployeeDto dto = new()
            {
                Id = employee.Id,
                Name = employee.Name,
                Gender = employee.Gender,
                DateOfBirth = employee.DateOfBirth,
                EmailId = employee.EmailId,
                Mobile = employee.Mobile,
                Salary = employee.Salary,
                DateOfJoining = employee.DateOfJoining,
                DepartmentId = employee.DepartmentId,
                DepartmentName =
                        employee.Department != null
                            ? employee.Department.DepartmentName
                            : null
            };

            return new ServiceResponseDto<EmployeeDto>
            {
                Success = true,
                Message = "Employee fetched successfully",
                Data = dto
            };
        }


        public ServiceResponseDto<PagenationDto<EmployeeDto>> GetEmployees(string? searchText, int pageNumber, int pageSize)
        {

            var query = _employeeRepository.GetEmployees(searchText);

            var totalRecords = query.Count();

            var employees = query
                .OrderBy(e => e.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new EmployeeDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    Gender = e.Gender,
                    DateOfBirth = e.DateOfBirth,
                    EmailId = e.EmailId,
                    Mobile = e.Mobile,
                    Salary = e.Salary,
                    DateOfJoining = e.DateOfJoining,
                    DepartmentId = e.DepartmentId,

                    DepartmentName =
                        e.Department != null
                            ? e.Department.DepartmentName
                            : null
                })
                .ToList();

            return new ServiceResponseDto<PagenationDto<EmployeeDto>>
            {
                Success = true,
                Message = "Employees fetched successfully",

                Data = new PagenationDto<EmployeeDto>
                {
                    Data = employees,
                    TotalRecords = totalRecords,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    SearchText = searchText
                }
            };
        }


        public ServiceResponseDto<ICollection<DepartmentDto>> GetDepartments()
        {

            var departments = _employeeRepository.GetDepartments();

            var departmentDtos = departments.Select(d => new DepartmentDto
            {
                Id = d.Id,
                DepartmentName = d.DepartmentName
            }).ToList();

            return new ServiceResponseDto<ICollection<DepartmentDto>>
            {
                Success = true,
                Message = "Departments fetched successfully",
                Data = departmentDtos
            };
        }

        public async Task<ServiceResponseDto<EmployeeDocumentResponseDto>> UploadDocumentAsync(EmployeeDocumentUploadDto dto)
        {
            var employee = _employeeRepository.GetById(dto.EmployeeId);

            if (employee == null)
            {
                return new ServiceResponseDto<EmployeeDocumentResponseDto>
                {
                    Success = false,
                    Message = "Employee not found"
                };
            }

            if (dto.File == null || dto.File.Length == 0)
            {
                return new ServiceResponseDto<EmployeeDocumentResponseDto>
                {
                    Success = false,
                    Message = "Please upload a file"
                };
            }

            var documentType = _documentRepository.GetById(dto.DocumentTypeId);

            if (documentType == null)
            {
                return new ServiceResponseDto<EmployeeDocumentResponseDto>
                {
                    Success = false,
                    Message = "Invalid document type"
                };
            }

            var uploadResult = await _blobService.UploadFileAsync(dto.File);

            EmployeeDocument employeeDocument = new()
            {
                EmployeeId = dto.EmployeeId,
                DocumentTypeId = dto.DocumentTypeId,
                OriginalFileName = dto.File.FileName,
                StoredFileName = uploadResult.storedFileName,
                BlobUrl = uploadResult.bloburl,
                UploadedDate = DateTime.UtcNow
            };

            await _employeeRepository.AddDocumentAsync(employeeDocument);

            EmployeeDocumentResponseDto responseDto = new()
            {
                DocumentCategory =documentType.DocumentCategory.Name,

                DocumentType = documentType.Name,

                OriginalFileName = employeeDocument.OriginalFileName,

                StoredFileName = employeeDocument.StoredFileName,

                BlobUrl = employeeDocument.BlobUrl,

                UploadedDate = employeeDocument.UploadedDate
            };

            return new ServiceResponseDto<EmployeeDocumentResponseDto>
            {
                Success = true,
                Message = "Document Uploaded Successfully",
                Data = responseDto
            };
        }

        public ServiceResponseDto<IEnumerable<DocumentTypeResponseDto>> GetByCategory(int categoryId)
        {
            var documentTypes =_documentRepository.GetByCategory(categoryId);

            var dtos = documentTypes.Select(dt =>
                new DocumentTypeResponseDto
                {
                    Id = dt.Id,

                    Name = dt.Name,

                    IsMandatory = dt.IsMandatory,

                    DocumentCategoryId =dt.DocumentCategoryId,

                    DocumentCategoryName =dt.DocumentCategory.Name
                }).ToList();

            return new ServiceResponseDto<IEnumerable<DocumentTypeResponseDto>>
            {
                Success = true,

                Message ="Document types fetched successfully",

                Data = dtos
            };
        }
    }
}


