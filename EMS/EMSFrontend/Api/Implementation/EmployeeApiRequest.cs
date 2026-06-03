using EMSFrontend.Api.Abstraction;
using EMSFrontend.Api.ApiException;
using EMSFrontend.Models;
using System.Net;
using System.Net.Http.Headers;
namespace EMSFrontend.Api.Implementation
{
    public class EmployeeApiRequest : IRequest
    {
        private readonly HttpClient client;
        private readonly IHttpContextAccessor accessor;
        private readonly IAuthRequest authRequest;

        public EmployeeApiRequest(HttpClient client, IHttpContextAccessor accessor, IAuthRequest authRequest)
        {
            this.client = client;
            this.accessor = accessor;
            this.authRequest = authRequest;
        }

        private void SetBearerToken()
        {
            try
            {
                string token =
                accessor.HttpContext.Session
                    .GetString("JWToken");

                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue(
                            "Bearer",
                            token);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        private async Task<bool> RefreshTokenJwtAsync()
        {
            try
            {
                var session = accessor.HttpContext.Session;

                string jwtToken = session.GetString("JWToken");

                string refreshToken = session.GetString("RefreshToken");

                if (string.IsNullOrEmpty(jwtToken) || string.IsNullOrEmpty(refreshToken))
                {
                    return false;
                }
                var response = await authRequest.RefreshTokenAsync(
                    new RefreshTokenViewModel
                    {
                        AccessToken = jwtToken,

                        RefreshToken =
                            refreshToken
                    });
                if (response == null)
                {
                    return false;
                }

                session.SetString("JWToken", response.Token);

                session.SetString("RefreshToken", response.RefreshToken);
                return true;
            }
            catch
            {
                accessor.HttpContext.Session.Clear();

                return false;
            }
        }

        private async Task HandleErrorResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return;
            }

            var error =
                await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedException("Session expired");
            }

            if ((int)response.StatusCode >= 500)
            {
                throw new Exception("Internal server error");
            }

            throw new ApiRequestException(error,(int)response.StatusCode);
        }
        public async Task<IEnumerable<EmployeeViewModel>> SendViewAllEmployeeRequestAsync()
        {

            SetBearerToken();
            var response = await client.GetAsync("all");
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                bool refreshed = await RefreshTokenJwtAsync();
                if (!refreshed)
                {
                    accessor.HttpContext.Session.Clear();

                    throw new UnauthorizedException("Session expired");
                }
                SetBearerToken();

                response = await client.GetAsync("all");
            }
            await HandleErrorResponse(response);

            var result = await response.Content.ReadFromJsonAsync<ServiceResponseViewModel<IEnumerable<EmployeeViewModel>>>();
            return result.Data;

        }

        public async Task<CreateEmployeeViewModel> SendCreateEmployeeRequestAsync(CreateEmployeeViewModel model)
        {
            SetBearerToken();
            var response = await client.PostAsJsonAsync<CreateEmployeeViewModel>("add", model);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                bool refreshed = await RefreshTokenJwtAsync();
                if (!refreshed)
                {
                    accessor.HttpContext.Session.Clear();
                    throw new UnauthorizedException("Session expired");
                }
                SetBearerToken();
                response = await client.PostAsJsonAsync<CreateEmployeeViewModel>("add", model);
            }

            await HandleErrorResponse(response);
            var result = await response.Content.ReadFromJsonAsync<ServiceResponseViewModel<CreateEmployeeViewModel>>();

            return result.Data;
        }

        public async Task<EmployeeViewModel> SendDeleteEmployeeRequestAsync(int id)
        {
            SetBearerToken();
            var response = await client.DeleteAsync($"delete/{id}");
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                bool refreshed = await RefreshTokenJwtAsync();
                if (!refreshed)
                {
                    accessor.HttpContext.Session.Clear();
                    throw new UnauthorizedException("Session expired");
                }
                SetBearerToken();

                response = await client.DeleteAsync($"delete/{id}");
            }
            await HandleErrorResponse(response);

            var result = await response.Content.ReadFromJsonAsync<ServiceResponseViewModel<EmployeeViewModel>>();
            return result.Data;
        }

        public async Task<EmployeeViewModel> SendUpdateEmployeeRequestAsync(int id, EmployeeViewModel model)
        {
            SetBearerToken();
            var response = await client.PutAsJsonAsync<EmployeeViewModel>($"update/{id}", model);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                bool refreshed = await RefreshTokenJwtAsync();
                if (!refreshed)
                {
                    accessor.HttpContext.Session.Clear();

                    throw new UnauthorizedException("Session expired");
                }
                SetBearerToken();

                response = await client.PutAsJsonAsync<EmployeeViewModel>($"update/{id}", model);
            }

            await HandleErrorResponse(response);

            var result = await response.Content.ReadFromJsonAsync<ServiceResponseViewModel<EmployeeViewModel>>();

            return result.Data;
        }

        public async Task<EmployeeViewModel> SendGetAEmployeeRequestAsync(int id)
        {
            SetBearerToken();
            var response = await client.GetAsync($"employee/{id}");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                bool refreshed = await RefreshTokenJwtAsync();
                if (!refreshed)
                {
                    accessor.HttpContext.Session.Clear();

                    throw new UnauthorizedException("Session expired");

                }
                SetBearerToken();

                response = await client.GetAsync($"employee/{id}");
            }

            await HandleErrorResponse(response);

            var result = await response.Content.ReadFromJsonAsync<ServiceResponseViewModel<EmployeeViewModel>>();

            return result.Data;

        }

        public async Task<IEnumerable<EmployeeViewModel>> SendSearchEmployeeRequestAsync(string searchText)
        {

            var response = await client.GetAsync($"employee/search?searchText={searchText}");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                bool refreshed = await RefreshTokenJwtAsync();
                if (!refreshed)
                {
                    accessor.HttpContext.Session.Clear();

                    throw new UnauthorizedException("Session expired");
                }
                SetBearerToken();

                response = await client.GetAsync($"employee/search?searchText={searchText}");
            }

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return new List<EmployeeViewModel>();
            }

            await HandleErrorResponse(response);

            var result = await response.Content.ReadFromJsonAsync<ServiceResponseViewModel<IEnumerable<EmployeeViewModel>>>();

            return result.Data ?? new List<EmployeeViewModel>();
        }

        public async Task<PaginationViewModel<EmployeeViewModel>> SendGetEmployeesAsync(string? searchText, int pageNumber, int pageSize)
        {

            SetBearerToken();
            var response = await client.GetAsync($"employees?searchText={searchText}&pageNumber={pageNumber}&pageSize={pageSize}");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                bool refreshed = await RefreshTokenJwtAsync();
                if (!refreshed)
                {
                    accessor.HttpContext.Session.Clear();

                    throw new UnauthorizedException("Session expired");
                }
                SetBearerToken();

                response = await client.GetAsync($"employees?searchText={searchText}&pageNumber={pageNumber}&pageSize={pageSize}");
            }

            await HandleErrorResponse(response);

            var result = await response.Content.ReadFromJsonAsync<ServiceResponseViewModel<PaginationViewModel<EmployeeViewModel>>>();

            return result.Data;

        }

        public async Task<IEnumerable<DepartmentViewmodel>> SendGetDepartmentsAsync()
        {
            SetBearerToken();
            var response = await client.GetAsync("department/all");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                bool refreshed = await RefreshTokenJwtAsync();
                if (!refreshed)
                {
                    accessor.HttpContext.Session.Clear();

                    throw new UnauthorizedException("Session expired");
                }
                SetBearerToken();

                response = await client.GetAsync("department/all");
            }

            await HandleErrorResponse(response);

            var result = await response.Content.ReadFromJsonAsync<ServiceResponseViewModel<IEnumerable<DepartmentViewmodel>>>();

            return result.Data;
        }

        public async Task<IEnumerable<DocumentCategoryViewModel>> SendGetDocumentCategoriesAsync()
        {
            SetBearerToken();

            var response = await client.GetAsync("documentcategory");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                bool refreshed = await RefreshTokenJwtAsync();

                if (!refreshed)
                {
                    accessor.HttpContext.Session.Clear();

                    throw new UnauthorizedException("Session expired");
                }

                SetBearerToken();

                response =await client.GetAsync("documentcategory");
            }

            await HandleErrorResponse(response);

            var result =await response.Content.ReadFromJsonAsync<ServiceResponseViewModel<IEnumerable<DocumentCategoryViewModel>>>();

            return result.Data;
        }

        public async Task<IEnumerable<DocumentTypeViewModel>> SendGetDocumentTypesByCategoryAsync(int categoryId, int employeeId)
        {
            SetBearerToken();

            var response = await client.GetAsync($"category/{categoryId}/employee/{employeeId}");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                bool refreshed =await RefreshTokenJwtAsync();

                if (!refreshed)
                {
                    accessor.HttpContext.Session.Clear();

                    throw new UnauthorizedException("Session expired");
                }

                SetBearerToken();

                response =await client.GetAsync($"category/{categoryId}/employee/{employeeId}");
            }

            await HandleErrorResponse(response);

            var result =await response.Content.ReadFromJsonAsync<ServiceResponseViewModel< IEnumerable<DocumentTypeViewModel>>>();

            return result.Data;
        }

        public async Task<DeleteDocumentResponseViewModel> SendDeleteDocumentAsync(int documentId)
        {
            SetBearerToken();

            var response = await client.DeleteAsync($"delete-document/{documentId}");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                bool refreshed = await RefreshTokenJwtAsync();
                if (!refreshed)
                {
                    accessor.HttpContext.Session.Clear();
                    throw new UnauthorizedException("Session expired");
                }
                SetBearerToken();

                response = await client.DeleteAsync($"delete-document/{documentId}");
            }
            await HandleErrorResponse(response);

            var result = await response.Content.ReadFromJsonAsync<ServiceResponseViewModel<DeleteDocumentResponseViewModel>>();
            return result.Data;
        }

        public async Task<EmployeeDocumentResponseViewModel> SendUploadDocumentAsync(EmployeeDocumentUploadViewModel model)
        {
            SetBearerToken();

            MultipartFormDataContent content = new();

            content.Add(new StringContent(model.EmployeeId.ToString()),"EmployeeId");

            content.Add(new StringContent(model.DocumentTypeId.ToString()),"DocumentTypeId");

            content.Add(new StreamContent(model.File.OpenReadStream()),"File",model.File.FileName);

            var response =await client.PostAsync("upload",content);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                bool refreshed = await RefreshTokenJwtAsync();
                if (!refreshed)
                {
                    accessor.HttpContext.Session.Clear();
                    throw new UnauthorizedException("Session expired");
                }
                SetBearerToken();
                response = await client.PostAsync("upload", content);
            }

            await HandleErrorResponse(response);

            var result = await response.Content.ReadFromJsonAsync
                <ServiceResponseViewModel
                <EmployeeDocumentResponseViewModel>>();

            return result.Data;
        }

        public async Task<EmployeeDocumentResponseViewModel> SendReplaceDocumentAsync(ReplaceDocumentViewModel model)
        {
            SetBearerToken();

            MultipartFormDataContent content = new();

            content.Add(new StringContent(model.DocumentId.ToString()),"DocumentId");

            content.Add(new StreamContent(model.File.OpenReadStream()),"File", model.File.FileName);

            var response = await client.PutAsync("edit-document", content);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                bool refreshed = await RefreshTokenJwtAsync();
                if (!refreshed)
                {
                    accessor.HttpContext.Session.Clear();
                    throw new UnauthorizedException("Session expired");
                }
                SetBearerToken();
                response = await client.PutAsync("edit-document", content);
            }

            await HandleErrorResponse(response);

            var result = await response.Content.ReadFromJsonAsync
                <ServiceResponseViewModel
                <EmployeeDocumentResponseViewModel>>();

            return result.Data;
        }

        public async Task<DocumentViewResponseViewModel> SendViewDocumentAsync(int documentId)
        {
            SetBearerToken();

            var response = await client.GetAsync($"view/{documentId}");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                bool refreshed = await RefreshTokenJwtAsync();

                if (!refreshed)
                {
                    accessor.HttpContext.Session.Clear();

                    throw new UnauthorizedException("Session expired");
                }

                SetBearerToken();

                response = await client.GetAsync($"view/{documentId}");
            }

            await HandleErrorResponse(response);

            var result = await response.Content.ReadFromJsonAsync<ServiceResponseViewModel<DocumentViewResponseViewModel>>();

            return result.Data;
        }
    }
}
 
