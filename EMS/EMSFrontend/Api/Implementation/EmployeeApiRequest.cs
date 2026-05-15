using EMSFrontend.Api.Abstraction;
using EMSFrontend.Models;
using System.Collections;
using System.Net;
namespace EMSFrontend.Api.Implementation
{
    public class EmployeeApiRequest : IRequest
    {
        private readonly HttpClient client;

        public EmployeeApiRequest(HttpClient client)
        {
            this.client = client;
        }
        public async Task<IEnumerable<EmployeeViewModel>> SendViewAllEmployeeRequestAsync()
        {
            try
            {
                var response = await client.GetAsync("all");
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Api Error: {response.StatusCode}, Details: {error}");
                }
                var data = await response.Content.ReadFromJsonAsync<IEnumerable<EmployeeViewModel>>();
                return data;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<CreateEmployeeViewModel> SendCreateEmployeeRequestAsync(CreateEmployeeViewModel model)
        {
            try
            {
                var response = await client.PostAsJsonAsync<CreateEmployeeViewModel>("add", model);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Api Error: {response.StatusCode}, Details: {error}");
                }
                return await response.Content.ReadFromJsonAsync<CreateEmployeeViewModel>();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<EmployeeViewModel> SendDeleteEmployeeRequestAsync(int id)
        {
            try
            {
                var response = await client.DeleteAsync($"delete/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Api Error: {response.StatusCode}, Details: {error}");
                }
                return await response.Content.ReadFromJsonAsync<EmployeeViewModel>();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<EmployeeViewModel> SendUpdateEmployeeRequestAsync(int id, EmployeeViewModel model)
        {
            try
            {
                var response = await client.PutAsJsonAsync<EmployeeViewModel>($"update/{id}",model);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Api Error: {response.StatusCode}, Details: {error}");
                }
                return await response.Content.ReadFromJsonAsync<EmployeeViewModel>();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<EmployeeViewModel> SendGetAEmployeeRequestAsync(int id)
        {
            try
            {
                var response = await client.GetAsync($"employee/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Api Error: {response.StatusCode}, Details: {error}");
                }
                return await response.Content.ReadFromJsonAsync<EmployeeViewModel>();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<IEnumerable<EmployeeViewModel>> SendSearchEmployeeRequestAsync(string searchText)
        {
            try
            {
                var response = await client.GetAsync($"employee/search?searchText={searchText}");
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
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<PaginationViewModel<EmployeeViewModel>> SendGetEmployeesAsync(string? searchText, int pageNumber, int pageSize)
        {
            try
            {
                var response = await client.GetAsync($"employees?searchText={searchText}&pageNumber={pageNumber}&pageSize={pageSize}");
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Api Error: {response.StatusCode}, Details: {error}");
                }

                var data = await response.Content.ReadFromJsonAsync<PaginationViewModel<EmployeeViewModel>>();

                return data;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<IEnumerable<DepartmentViewmodel>>
     SendGetDepartmentsAsync()
        {
            try
            {
                var response =
                    await client.GetAsync("department/all");

                if (!response.IsSuccessStatusCode)
                {
                    var error =
                        await response.Content
                        .ReadAsStringAsync();

                    throw new Exception(
                        $"Api Error: {response.StatusCode}, Details: {error}");
                }

                var data =
                    await response.Content
                    .ReadFromJsonAsync<
                        IEnumerable<DepartmentViewmodel>>();

                return data;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
