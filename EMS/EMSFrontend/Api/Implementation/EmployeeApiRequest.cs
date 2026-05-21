using EMSFrontend.Api.Abstraction;
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
        public async Task<IEnumerable<EmployeeViewModel>> SendViewAllEmployeeRequestAsync()
        {
            try
            {
                SetBearerToken();
                var response = await client.GetAsync("all");
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    bool refreshed = await RefreshTokenJwtAsync();
                    if (!refreshed)
                    {
                        accessor.HttpContext.Session.Clear();

                        throw new UnauthorizedAccessException("Session expired");
                    }
                    SetBearerToken();

                    response = await client.GetAsync("all");
                }
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Api Error: {response.StatusCode}, Details: {error}");
                }
                var data = await response.Content.ReadFromJsonAsync<IEnumerable<EmployeeViewModel>>();
                return data;
            }
            catch
            {
                throw;
            }
        }

        public async Task<CreateEmployeeViewModel> SendCreateEmployeeRequestAsync(CreateEmployeeViewModel model)
        {
            try
            {
                SetBearerToken();
                var response = await client.PostAsJsonAsync<CreateEmployeeViewModel>("add", model);
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    bool refreshed = await RefreshTokenJwtAsync();
                    if (!refreshed)
                    {
                        accessor.HttpContext.Session.Clear();

                        throw new UnauthorizedAccessException("Session expired");
                    }
                    SetBearerToken();
                    response = await client.PostAsJsonAsync<CreateEmployeeViewModel>("add", model);
                }

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Api Error: {response.StatusCode}, Details: {error}");
                }
                return await response.Content.ReadFromJsonAsync<CreateEmployeeViewModel>();
            }
            catch
            {
                throw;
            }
        }

        public async Task<EmployeeViewModel> SendDeleteEmployeeRequestAsync(int id)
        {
            try
            {
                SetBearerToken();
                var response = await client.DeleteAsync($"delete/{id}");
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    bool refreshed = await RefreshTokenJwtAsync();
                    if (!refreshed)
                    {
                        accessor.HttpContext.Session.Clear();

                        throw new UnauthorizedAccessException("Session expired");
                    }
                    SetBearerToken();

                    response = await client.DeleteAsync($"delete/{id}");
                }
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Api Error: {response.StatusCode}, Details: {error}");
                }
                return await response.Content.ReadFromJsonAsync<EmployeeViewModel>();
            }
            catch
            {
                throw;
            }
        }

        public async Task<EmployeeViewModel> SendUpdateEmployeeRequestAsync(int id, EmployeeViewModel model)
        {
            try
            {
                SetBearerToken();
                var response = await client.PutAsJsonAsync<EmployeeViewModel>($"update/{id}",model);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    bool refreshed = await RefreshTokenJwtAsync();
                    if (!refreshed)
                    {
                        accessor.HttpContext.Session.Clear();

                        throw new UnauthorizedAccessException("Session expired");
                    }
                    SetBearerToken();

                    response = await client.PutAsJsonAsync<EmployeeViewModel>($"update/{id}", model);
                }

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Api Error: {response.StatusCode}, Details: {error}");
                }
                return await response.Content.ReadFromJsonAsync<EmployeeViewModel>();
            }
            catch
            {
                throw;
            }
        }

        public async Task<EmployeeViewModel> SendGetAEmployeeRequestAsync(int id)
        {
            try
            {
                SetBearerToken();
                var response = await client.GetAsync($"employee/{id}");

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    bool refreshed = await RefreshTokenJwtAsync();
                    if (!refreshed)
                    {
                        accessor.HttpContext.Session.Clear();

                        throw new UnauthorizedAccessException("Session expired");
                    }
                    SetBearerToken();

                    response = await client.GetAsync($"employee/{id}");
                }

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Api Error: {response.StatusCode}, Details: {error}");
                }
                return await response.Content.ReadFromJsonAsync<EmployeeViewModel>();
            }
            catch
            {
                throw;
            }
        }

        public async Task<IEnumerable<EmployeeViewModel>> SendSearchEmployeeRequestAsync(string searchText)
        {
            try
            {
                var response = await client.GetAsync($"employee/search?searchText={searchText}");

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    bool refreshed = await RefreshTokenJwtAsync();
                    if (!refreshed)
                    {
                        accessor.HttpContext.Session.Clear();

                        throw new UnauthorizedAccessException("Session expired");
                    }
                    SetBearerToken();

                    response = await client.GetAsync($"employee/search?searchText={searchText}");
                }

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Api Error: {response.StatusCode}, Details: {error}");
                }
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    return new List<EmployeeViewModel>();
                }
                var data = await response.Content.ReadFromJsonAsync<IEnumerable<EmployeeViewModel>>();
                return data ?? new List<EmployeeViewModel>();
            }
            catch
            {
                throw;
            }
        }

        public async Task<PaginationViewModel<EmployeeViewModel>> SendGetEmployeesAsync(string? searchText, int pageNumber, int pageSize)
        {
            try
            {
                SetBearerToken();
                var response = await client.GetAsync($"employees?searchText={searchText}&pageNumber={pageNumber}&pageSize={pageSize}");

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    bool refreshed = await RefreshTokenJwtAsync();
                    if (!refreshed)
                    {
                        accessor.HttpContext.Session.Clear();

                        throw new UnauthorizedAccessException("Session expired");
                    }
                    SetBearerToken();

                    response = await client.GetAsync($"employees?searchText={searchText}&pageNumber={pageNumber}&pageSize={pageSize}");
                }

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Api Error: {response.StatusCode}, Details: {error}");
                }

                var data = await response.Content.ReadFromJsonAsync<PaginationViewModel<EmployeeViewModel>>();

                return data;
            }
            catch
            {
                throw;
            }
        }

        public async Task<IEnumerable<DepartmentViewmodel>>SendGetDepartmentsAsync()
        {
            try
            {
                SetBearerToken();
                var response = await client.GetAsync("department/all");

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    bool refreshed = await RefreshTokenJwtAsync();
                    if (!refreshed)
                    {
                        accessor.HttpContext.Session.Clear();

                        throw new UnauthorizedAccessException("Session expired");
                    }
                    SetBearerToken();

                    response = await client.GetAsync("department/all");
                }

                if (!response.IsSuccessStatusCode)
                {
                    var error =await response.Content.ReadAsStringAsync();

                    throw new Exception(
                        $"Api Error: {response.StatusCode}, Details: {error}");
                }

                var data =
                    await response.Content
                    .ReadFromJsonAsync<
                        IEnumerable<DepartmentViewmodel>>();

                return data;
            }
            catch
            {
                throw;
            }
        }
    }
}
