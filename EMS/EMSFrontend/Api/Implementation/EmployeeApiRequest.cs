using Dtos;
using EMSFrontend.Api.Abstraction;
using EMSFrontend.Api.ApiException;
using EMSFrontend.Helpers;
using EMSFrontend.Models;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
namespace EMSFrontend.Api.Implementation
{
    public class EmployeeApiRequest : IRequest
    {
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _accessor;
        private readonly IApiRequestHelper _apiRequestHelper;

        public EmployeeApiRequest(HttpClient client, IHttpContextAccessor accessor, IApiRequestHelper apiRequestHelper)
        {
            _client = client;
            _accessor = accessor;
            _apiRequestHelper = apiRequestHelper;
        }
            public async Task<IEnumerable<EmployeeViewModel>> SendViewAllEmployeeRequestAsync()
            {

                _apiRequestHelper.SetBearerToken(_client);
                var response = await _client.GetAsync("all");
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    bool refreshed = await _apiRequestHelper.RefreshTokenJwtAsync(_client);
                    if (!refreshed)
                    {
                        _accessor.HttpContext.Session.Clear();

                        throw new UnauthorizedException("Session expired");
                    }
                    _apiRequestHelper.SetBearerToken(_client);

                    response = await _client.GetAsync("all");
                }
                await _apiRequestHelper.HandleErrorResponse(response);

                var result = await response.Content.ReadFromJsonAsync<ServiceResponseViewModel<IEnumerable<EmployeeViewModel>>>();
                return result.Data;

            }

        public async Task<CreateEmployeeViewModel> SendCreateEmployeeRequestAsync(CreateEmployeeViewModel model)
        {
            _apiRequestHelper.SetBearerToken(_client);
            var response = await _client.PostAsJsonAsync<CreateEmployeeViewModel>("add", model);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                bool refreshed = await _apiRequestHelper.RefreshTokenJwtAsync(_client);
                if (!refreshed)
                {
                    _accessor.HttpContext.Session.Clear();
                    throw new UnauthorizedException("Session expired");
                }
                _apiRequestHelper.SetBearerToken(_client);
                response = await _client.PostAsJsonAsync<CreateEmployeeViewModel>("add", model);
            }

            await _apiRequestHelper.HandleErrorResponse(response);
            var result = await response.Content.ReadFromJsonAsync<ServiceResponseViewModel<CreateEmployeeViewModel>>();

            return result.Data;
        }

        public async Task<EmployeeViewModel> SendDeleteEmployeeRequestAsync(int id)
        {
            _apiRequestHelper.SetBearerToken(_client);
            var response = await _client.DeleteAsync($"delete/{id}");
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                bool refreshed = await _apiRequestHelper.RefreshTokenJwtAsync(_client);
                if (!refreshed)
                {
                    _accessor.HttpContext.Session.Clear();
                    throw new UnauthorizedException("Session expired");
                }
                _apiRequestHelper.SetBearerToken(_client);

                response = await _client.DeleteAsync($"delete/{id}");
            }
            await _apiRequestHelper.HandleErrorResponse(response);

            var result = await response.Content.ReadFromJsonAsync<ServiceResponseViewModel<EmployeeViewModel>>();
            return result.Data;
        }

        public async Task<EmployeeViewModel> SendUpdateEmployeeRequestAsync(int id, EmployeeViewModel model)
        {
            _apiRequestHelper.SetBearerToken(_client);
            var response = await _client.PutAsJsonAsync<EmployeeViewModel>($"update/{id}", model);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                bool refreshed = await _apiRequestHelper.RefreshTokenJwtAsync(_client);
                if (!refreshed)
                {
                    _accessor.HttpContext.Session.Clear();

                    throw new UnauthorizedException("Session expired");
                }
                _apiRequestHelper.SetBearerToken(_client);

                response = await _client.PutAsJsonAsync<EmployeeViewModel>($"update/{id}", model);
            }

            await _apiRequestHelper.HandleErrorResponse(response);

            var result = await response.Content.ReadFromJsonAsync<ServiceResponseViewModel<EmployeeViewModel>>();

            return result.Data;
        }

        public async Task<EmployeeViewModel> SendGetAEmployeeRequestAsync(int id)
        {
            _apiRequestHelper.SetBearerToken(_client);
            var response = await _client.GetAsync($"employee/{id}");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                bool refreshed = await _apiRequestHelper.RefreshTokenJwtAsync(_client);
                if (!refreshed)
                {
                    _accessor.HttpContext.Session.Clear();

                    throw new UnauthorizedException("Session expired");

                }
                _apiRequestHelper.SetBearerToken(_client);

                response = await _client.GetAsync($"employee/{id}");
            }

            await _apiRequestHelper.HandleErrorResponse(response);

            var result = await response.Content.ReadFromJsonAsync<ServiceResponseViewModel<EmployeeViewModel>>();

            return result.Data;

        }

        public async Task<IEnumerable<EmployeeViewModel>> SendSearchEmployeeRequestAsync(string searchText)
        {

            var response = await _client.GetAsync($"employee/search?searchText={searchText}");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                bool refreshed = await _apiRequestHelper.RefreshTokenJwtAsync(_client);
                if (!refreshed)
                {
                    _accessor.HttpContext.Session.Clear();

                    throw new UnauthorizedException("Session expired");
                }
                _apiRequestHelper.SetBearerToken(_client);

                response = await _client.GetAsync($"employee/search?searchText={searchText}");
            }

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return new List<EmployeeViewModel>();
            }

            await _apiRequestHelper.HandleErrorResponse(response);

            var result = await response.Content.ReadFromJsonAsync<ServiceResponseViewModel<IEnumerable<EmployeeViewModel>>>();

            return result.Data ?? new List<EmployeeViewModel>();
        }

        public async Task<PaginationViewModel<EmployeeViewModel>> SendGetEmployeesAsync(string? searchText, int pageNumber, int pageSize)
        {

            _apiRequestHelper.SetBearerToken(_client);
            var response = await _client.GetAsync($"employees?searchText={searchText}&pageNumber={pageNumber}&pageSize={pageSize}");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                bool refreshed = await _apiRequestHelper.RefreshTokenJwtAsync(_client);
                if (!refreshed)
                {
                    _accessor.HttpContext.Session.Clear();

                    throw new UnauthorizedException("Session expired");
                }
                _apiRequestHelper.SetBearerToken(_client);

                response = await _client.GetAsync($"employees?searchText={searchText}&pageNumber={pageNumber}&pageSize={pageSize}");
            }

            await _apiRequestHelper.HandleErrorResponse(response);

            var result = await response.Content.ReadFromJsonAsync<ServiceResponseViewModel<PaginationViewModel<EmployeeViewModel>>>();

            return result.Data;

        }

        public async Task<IEnumerable<DepartmentViewmodel>> SendGetDepartmentsAsync()
        {
            _apiRequestHelper.SetBearerToken(_client);
            var response = await _client.GetAsync("department/all");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                bool refreshed = await _apiRequestHelper.RefreshTokenJwtAsync(_client);
                if (!refreshed)
                {
                    _accessor.HttpContext.Session.Clear();

                    throw new UnauthorizedException("Session expired");
                }
                _apiRequestHelper.SetBearerToken(_client);

                response = await _client.GetAsync("department/all");
            }

            await _apiRequestHelper.HandleErrorResponse(response);

            var result = await response.Content.ReadFromJsonAsync<ServiceResponseViewModel<IEnumerable<DepartmentViewmodel>>>();

            return result.Data;
        }

        public async Task<IEnumerable<DocumentCategoryViewModel>> SendGetDocumentCategoriesAsync()
        {
            _apiRequestHelper.SetBearerToken(_client);

            var response = await _client.GetAsync("documentcategory");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                bool refreshed = await _apiRequestHelper.RefreshTokenJwtAsync(_client);

                if (!refreshed)
                {
                    _accessor.HttpContext.Session.Clear();

                    throw new UnauthorizedException("Session expired");
                }

                _apiRequestHelper.SetBearerToken(_client);

                response =await _client.GetAsync("documentcategory");
            }

            await _apiRequestHelper.HandleErrorResponse(response);

            var result =await response.Content.ReadFromJsonAsync<ServiceResponseViewModel<IEnumerable<DocumentCategoryViewModel>>>();

            return result.Data;
        }

        public async Task<IEnumerable<DocumentTypeViewModel>> SendGetDocumentTypesByCategoryAsync(int categoryId, int employeeId)
        {
            _apiRequestHelper.SetBearerToken(_client);

            var response = await _client.GetAsync($"category/{categoryId}/employee/{employeeId}");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                bool refreshed =await _apiRequestHelper.RefreshTokenJwtAsync(_client);

                if (!refreshed)
                {
                    _accessor.HttpContext.Session.Clear();

                    throw new UnauthorizedException("Session expired");
                }

                _apiRequestHelper.SetBearerToken(_client);

                response =await _client.GetAsync($"category/{categoryId}/employee/{employeeId}");
            }

            await _apiRequestHelper.HandleErrorResponse(response);

            var result =await response.Content.ReadFromJsonAsync<ServiceResponseViewModel< IEnumerable<DocumentTypeViewModel>>>();

            return result.Data;
        }

        public async Task<DeleteDocumentResponseViewModel> SendDeleteDocumentAsync(int documentId)
        {
            _apiRequestHelper.SetBearerToken(_client);

            var response = await _client.DeleteAsync($"delete-document/{documentId}");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                bool refreshed = await _apiRequestHelper.RefreshTokenJwtAsync(_client);
                if (!refreshed)
                {
                    _accessor.HttpContext.Session.Clear();
                    throw new UnauthorizedException("Session expired");
                }
                _apiRequestHelper.SetBearerToken(_client);

                response = await _client.DeleteAsync($"delete-document/{documentId}");
            }
            await _apiRequestHelper.HandleErrorResponse(response);

            var result = await response.Content.ReadFromJsonAsync<ServiceResponseViewModel<DeleteDocumentResponseViewModel>>();
            return result.Data;
        }

        public async Task<EmployeeDocumentResponseViewModel> SendUploadDocumentAsync(EmployeeDocumentUploadViewModel model)
        {
            _apiRequestHelper.SetBearerToken(_client);

            MultipartFormDataContent content = new();

            content.Add(new StringContent(model.EmployeeId.ToString()),"EmployeeId");

            content.Add(new StringContent(model.DocumentTypeId.ToString()),"DocumentTypeId");

            content.Add(new StreamContent(model.File.OpenReadStream()),"File",model.File.FileName);

            var response =await _client.PostAsync("upload",content);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                bool refreshed = await _apiRequestHelper.RefreshTokenJwtAsync(_client);
                if (!refreshed)
                {
                    _accessor.HttpContext.Session.Clear();
                    throw new UnauthorizedException("Session expired");
                }
                _apiRequestHelper.SetBearerToken(_client);
                response = await _client.PostAsync("upload", content);
            }

            await _apiRequestHelper.HandleErrorResponse(response);

            var result = await response.Content.ReadFromJsonAsync
                <ServiceResponseViewModel
                <EmployeeDocumentResponseViewModel>>();

            return result.Data;
        }

        public async Task<EmployeeDocumentResponseViewModel> SendReplaceDocumentAsync(ReplaceDocumentViewModel model)
        {
            _apiRequestHelper.SetBearerToken(_client);

            MultipartFormDataContent content = new();

            content.Add(new StringContent(model.DocumentId.ToString()),"DocumentId");

            content.Add(new StreamContent(model.File.OpenReadStream()),"File", model.File.FileName);

            var response = await _client.PutAsync("edit-document", content);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                bool refreshed = await _apiRequestHelper.RefreshTokenJwtAsync(_client);
                if (!refreshed)
                {
                    _accessor.HttpContext.Session.Clear();
                    throw new UnauthorizedException("Session expired");
                }
                _apiRequestHelper.SetBearerToken(_client);
                response = await _client.PutAsync("edit-document", content);
            }

            await _apiRequestHelper.HandleErrorResponse(response);

            var result = await response.Content.ReadFromJsonAsync
                <ServiceResponseViewModel
                <EmployeeDocumentResponseViewModel>>();

            return result.Data;
        }

        public async Task<DocumentViewResponseViewModel> SendViewDocumentAsync(int documentId)
        {
            _apiRequestHelper.SetBearerToken(_client);

            var response = await _client.GetAsync($"view/{documentId}");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                bool refreshed = await _apiRequestHelper.RefreshTokenJwtAsync(_client);

                if (!refreshed)
                {
                    _accessor.HttpContext.Session.Clear();

                    throw new UnauthorizedException("Session expired");
                }

                _apiRequestHelper.SetBearerToken(_client);

                response = await _client.GetAsync($"view/{documentId}");
            }

            await _apiRequestHelper.HandleErrorResponse(response);

            var result = await response.Content.ReadFromJsonAsync<ServiceResponseViewModel<DocumentViewResponseViewModel>>();

            return result.Data;
        }

        public async Task<DashboardViewModel> GetDashboardAsync(int employeeId)
        {
            _apiRequestHelper.SetBearerToken(_client);

            var response = await _client.GetAsync($"dashboard/{employeeId}");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                bool refreshed = await _apiRequestHelper.RefreshTokenJwtAsync(_client);

                if (!refreshed)
                {
                    _accessor.HttpContext.Session.Clear();
                    throw new UnauthorizedException("Session expired");
                }

                _apiRequestHelper.SetBearerToken(_client);

                response = await _client.GetAsync($"dashboard/{employeeId}");
            }

            await _apiRequestHelper.HandleErrorResponse(response);

            var result = await response.Content.ReadFromJsonAsync<
                        ServiceResponseViewModel<DashboardViewModel>>();

            return result.Data;
        }

        public async Task<ServiceResponseDto<EmployeeUploadExcelResponseDto>>UploadEmployeesAsync(IFormFile file)
        {
            _apiRequestHelper.SetBearerToken(_client);

            using var content = new MultipartFormDataContent();

            using var stream = file.OpenReadStream();

            var fileContent = new StreamContent(stream);

            content.Add(fileContent,"file",file.FileName);

            var response =await _client.PostAsync("upload-employees",content);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                bool refreshed = await _apiRequestHelper.RefreshTokenJwtAsync(_client);

                if (!refreshed)
                {
                    _accessor.HttpContext.Session.Clear();

                    throw new UnauthorizedException("Session expired");
                }

                _apiRequestHelper.SetBearerToken(_client);

                using var retryContent =new MultipartFormDataContent();

                using var retryStream = file.OpenReadStream();

                var retryFileContent =
                    new StreamContent(retryStream);

                retryContent.Add(retryFileContent,"file",file.FileName);

                response = await _client.PostAsync("upload-employees",retryContent);
            }

            await _apiRequestHelper.HandleErrorResponse(response);

            return await response.Content.ReadFromJsonAsync<ServiceResponseDto<EmployeeUploadExcelResponseDto>>();
        }

        public async Task<byte[]> DownloadTemplateAsync()
        {
            _apiRequestHelper.SetBearerToken(_client);

            var response = await _client.GetAsync("download-template");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                bool refreshed = await _apiRequestHelper.RefreshTokenJwtAsync(_client);

                if (!refreshed)
                {
                    _accessor.HttpContext.Session.Clear();

                    throw new UnauthorizedException("Session expired");
                }

                _apiRequestHelper.SetBearerToken(_client);

                response = await _client.GetAsync("download-template");
            }

            await _apiRequestHelper.HandleErrorResponse(response);

            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task<byte[]> DownloadFailedRecordsAsync(List<UploadEmployeeExcelErrorDto> errors)
        {
            _apiRequestHelper.SetBearerToken(_client);

            var response = await _client.PostAsJsonAsync("download-failed-records",errors);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                bool refreshed = await _apiRequestHelper.RefreshTokenJwtAsync(_client);

                if (!refreshed)
                {
                    _accessor.HttpContext.Session.Clear();

                    throw new UnauthorizedException("Session expired");
                }

                _apiRequestHelper.SetBearerToken(_client);

                response = await _client.PostAsJsonAsync("download-failed-records", errors);
            }

            await _apiRequestHelper.HandleErrorResponse(response);

            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task<IEnumerable<RoleViewModel>> SendGetRolesAsync()
        {
            _apiRequestHelper.SetBearerToken(_client);

            var response = await _client.GetAsync("roles/all");
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                bool refreshed = await _apiRequestHelper.RefreshTokenJwtAsync(_client);

                if (!refreshed)
                {
                    _accessor.HttpContext.Session.Clear();

                    throw new UnauthorizedException("Session expired");
                }

                _apiRequestHelper.SetBearerToken(_client);
                response = await _client.GetAsync("roles/all");
            }

            await _apiRequestHelper.HandleErrorResponse(response);

            var result = await response.Content.ReadFromJsonAsync<
            ServiceResponseViewModel<IEnumerable<RoleViewModel>>>();

            return result.Data;
        }

        public async Task<IEnumerable<EmployeeDropdownViewModel>> SendGetManagersAsync()
        {
            _apiRequestHelper.SetBearerToken(_client);

            var response = await _client.GetAsync("managers/all");
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                bool refreshed = await _apiRequestHelper.RefreshTokenJwtAsync(_client);

                if (!refreshed)
                {
                    _accessor.HttpContext.Session.Clear();

                    throw new UnauthorizedException("Session expired");
                }

                _apiRequestHelper.SetBearerToken(_client);
                response = await _client.GetAsync("managers/all");
            }

            await _apiRequestHelper.HandleErrorResponse(response);

            var result = await response.Content.ReadFromJsonAsync<
            ServiceResponseViewModel<IEnumerable<EmployeeDropdownViewModel>>>();

            return result.Data;
        }
    }
}
 
