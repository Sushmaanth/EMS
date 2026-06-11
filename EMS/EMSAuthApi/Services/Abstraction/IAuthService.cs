using Dtos;

namespace EMSAuthApi.Services.Abstraction
{
    public interface IAuthService
    {
        Task<ServiceResponseDto<LoginResponseDTO>> LoginEmployee(LoginDto dto);

        Task<ServiceResponseDto<LoginResponseDTO>> RefreshToken(RefreshTokenDTO dto);

        Task<ServiceResponseDto<LoginResponseDTO>> MicrosoftLogin(string email);

        Task<ServiceResponseDto<string>> ForgotPasswordAsync(ForgotPasswordDto dto);
        
        Task<ServiceResponseDto<string>> ResetPasswordAsync(ResetPasswordDto dto);

        ActivateAccountResponseDTO ActivateAccount(ActivateAccountDTO dto);
    }
}
