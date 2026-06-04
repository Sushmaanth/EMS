using EMSFrontend.Models;
using System.Collections;

namespace EMSFrontend.Api.Abstraction
{
    public interface IRequest
    {
        Task<IEnumerable<EmployeeViewModel>> SendViewAllEmployeeRequestAsync();

        Task<EmployeeViewModel> SendGetAEmployeeRequestAsync(int id);

        Task<CreateEmployeeViewModel> SendCreateEmployeeRequestAsync(CreateEmployeeViewModel model);

        Task<EmployeeViewModel> SendDeleteEmployeeRequestAsync(int id);

        Task<EmployeeViewModel> SendUpdateEmployeeRequestAsync(int id, EmployeeViewModel model);

        Task<IEnumerable<EmployeeViewModel>> SendSearchEmployeeRequestAsync(string searchText);

        Task<PaginationViewModel<EmployeeViewModel>> SendGetEmployeesAsync(string? searchText,int pageNumber, int pageSize);
        Task<IEnumerable<DepartmentViewmodel>>SendGetDepartmentsAsync();

        Task<IEnumerable<DocumentCategoryViewModel>> SendGetDocumentCategoriesAsync();

        Task<IEnumerable<DocumentTypeViewModel>>SendGetDocumentTypesByCategoryAsync(int categoryId, int employeeId);

        Task<DeleteDocumentResponseViewModel>SendDeleteDocumentAsync(int documentId);

        Task<EmployeeDocumentResponseViewModel>SendUploadDocumentAsync(EmployeeDocumentUploadViewModel model);
        Task<EmployeeDocumentResponseViewModel>SendReplaceDocumentAsync(ReplaceDocumentViewModel model);

        Task<DocumentViewResponseViewModel>SendViewDocumentAsync(int documentId);

        Task<DashboardViewModel> GetDashboardAsync(int employeeId);
    }
}
