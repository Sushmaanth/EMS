using EMSFrontend.Api.Abstraction;
using EMSFrontend.Models;
using EMSFrontend.Api.ApiException;


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
            var response = await client.PostAsJsonAsync("activate-account", model);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorResponseViewModel>();
                throw new ApiRequestException(error.Message, (int)response.StatusCode);
            }
        }

        public async Task<LoginResponseViewModel> LoginAsync(LoginViewModel model)
        {
            var response = await client.PostAsJsonAsync("login", model);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorResponseViewModel>();
                throw new ApiRequestException(error.Message, (int)response.StatusCode);
            }

            var result = await response.Content.ReadFromJsonAsync<ServiceResponseViewModel<LoginResponseViewModel>>();

            return result.Data;
        }

        public async Task<LoginResponseViewModel> RefreshTokenAsync(RefreshTokenViewModel model)
        {
            var response = await client.PostAsJsonAsync("refresh-token", model);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorResponseViewModel>();
                throw new ApiRequestException(error.Message, (int)response.StatusCode);
            }

            var result = await response.Content.ReadFromJsonAsync<ServiceResponseViewModel<LoginResponseViewModel>>();

            return result.Data;
        }

        public async Task<LoginResponseViewModel> MicrosoftLoginAsync(string email)
        {
            var response = await client.PostAsJsonAsync("microsoft-login", email);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorResponseViewModel>();
                throw new ApiRequestException(error.Message, (int)response.StatusCode);
            }


            var result = await response.Content.ReadFromJsonAsync<ServiceResponseViewModel<LoginResponseViewModel>>();
            return result.Data;
        }

        public async Task<string> SendForgotPasswordAsync(ForgotPasswordViewModel model)
        {
            var response = await client.PostAsJsonAsync("forgot-password", model);

            var result = await response.Content
            .ReadFromJsonAsync<ServiceResponseViewModel<string>>();

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorResponseViewModel>();
                throw new ApiRequestException(error.Message, (int)response.StatusCode);
            }

            return result.Message;
        }

        public async Task<string> SendResetPasswordAsync(ResetPasswordViewModel model)
        {
            var response = await client.PostAsJsonAsync("reset-password", model);
            var result = await response.Content.ReadFromJsonAsync<ServiceResponseViewModel<string>>();
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorResponseViewModel>();
                throw new ApiRequestException(error.Message, (int)response.StatusCode);
            }

            return result.Message;
        }
    }
}
