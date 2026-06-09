using Dtos;

namespace EMSBackend.Service.Abstraction
{
    public interface IEmployeeService
    {
        Task<ServiceResponseDto<CreateEmployeeDto>> Create(CreateEmployeeDto dto);

        ServiceResponseDto<ICollection<EmployeeDto>>View();

        Task<ServiceResponseDto<CreateEmployeeDto>>Update(int id, CreateEmployeeDto dto);

        ServiceResponseDto<EmployeeDto>Delete(int id);

        ServiceResponseDto<EmployeeDto>GetById(int id);

        ServiceResponseDto<PagenationDto<EmployeeDto>>GetEmployees(string? searchText,int pageNumber,int pageSize);

        ServiceResponseDto<ICollection<DepartmentDto>>GetDepartments();

        Task<ServiceResponseDto<EmployeeDocumentResponseDto>> UploadDocumentAsync(EmployeeDocumentUploadDto dto);

        ServiceResponseDto<IEnumerable<DocumentTypeResponseDto>>GetByCategory(int categoryId);

        ServiceResponseDto<IEnumerable<DocumentCategoryResponseDto>>GetAll();

        Task<ServiceResponseDto<DeleteDocumentResponseDto>> DeleteDocumentAsync(int documentId);

        Task<ServiceResponseDto<EmployeeDocumentResponseDto>> ReplaceDocumentAsync(ReplaceDocumentDto dto);

        Task<ServiceResponseDto<DocumentViewResponseDto>> GetDocumentUrlAsync(int documentId);

        Task<ServiceResponseDto<ICollection<DocumentTypeDto>>>GetDocumentTypesByCategoryAsync(int categoryId, int employeeId);

        ServiceResponseDto<DashboardDto> GetEmployeeDashboard(int employeeId);

        ServiceResponseDto<EmployeeDocumentResponseDto>? ValidateFile(IFormFile file);

        Task<ServiceResponseDto<EmployeeUploadExcelResponseDto>>UploadEmployeesAsync(IFormFile file);

        ServiceResponseDto<byte[]> DownloadTemplate();

        ServiceResponseDto<byte[]> DownloadFailedRecordsAsync(List<UploadEmployeeExcelErrorDto> errors);
    }

}
