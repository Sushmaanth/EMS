using Dtos;
using EMSFrontend.Models;

namespace EMSFrontend.Api.Abstraction
{
    public interface IAuthRequest
    {
        Task<LoginResponseViewModel> LoginAsync(LoginViewModel model);

        Task<LoginResponseViewModel> RefreshTokenAsync(RefreshTokenViewModel model);

        Task ActivateAccountAsync(AccountActivationViewModel model);
        Task<LoginResponseViewModel> MicrosoftLoginAsync(string email);

        Task<string> SendForgotPasswordAsync(ForgotPasswordViewModel model);

        Task<string> SendResetPasswordAsync(ResetPasswordViewModel model);
    }
}
