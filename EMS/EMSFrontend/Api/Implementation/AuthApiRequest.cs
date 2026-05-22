using Dtos;
using EMSFrontend.Api.Abstraction;
using EMSFrontend.Models;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace EMSFrontend.Api.Implementation
{
    public class AuthApiRequest : IAuthRequest
    {
        private readonly HttpClient client;

        public AuthApiRequest(HttpClient client)
        {
            this.client = client;
        }

        public async Task ActivateAccountAsync(AccountActivationViewModel model)
        {
            try
            {
                var response = await client.PostAsJsonAsync("activate-account", model);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Api Error: {response.StatusCode}, Details: {error}");
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<LoginResponseViewModel> LoginAsync(LoginViewModel model)
        {
            try
            {
                var response = await client.PostAsJsonAsync("login", model);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Api Error: {response.StatusCode}, Details: {error}");
                }

                var data = await response.Content.ReadFromJsonAsync<LoginResponseViewModel>();

                return data;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<LoginResponseViewModel> RefreshTokenAsync(RefreshTokenViewModel model)
        {
            try
            {
                var response = await client.PostAsJsonAsync("refresh-token", model);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Api Error: {response.StatusCode}, Details: {error}");
                }

                var data = await response.Content.ReadFromJsonAsync<LoginResponseViewModel>();

                return data;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<LoginResponseViewModel> MicrosoftLoginAsync(string email)
        {
            try
            {
                var response = await client.PostAsJsonAsync("microsoft-login", email);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();

                    throw new Exception(error);
                }

                return await response.Content.ReadFromJsonAsync<LoginResponseViewModel>();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
           
        }

        public async Task<string> SendForgotPasswordAsync(ForgotPasswordViewModel model)
        {
            try
            {
                var response = await client.PostAsJsonAsync("forgot-password", model);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();

                    throw new Exception(error);
                }

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<string> SendResetPasswordAsync(ResetPasswordViewModel model)
        {
            try
            {
                var response = await client.PostAsJsonAsync("reset-password", model);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();

                    throw new Exception(error);
                }
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
