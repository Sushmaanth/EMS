using Dtos;
using EMSFrontend.Models;

namespace EMSFrontend.Api.Abstraction
{
    public interface IAuthRequest
    {
        Task<LoginResponseViewModel> LoginAsync(LoginViewModel model);

        Task<LoginResponseViewModel> RefreshTokenAsync(RefreshTokenDTO model);

        Task ActivateAccountAsync(AccountActivationViewModel model);
    }
}
