using Dtos;

namespace EMSAuthApi.Services.Abstraction
{
    public interface IAuthService
    {
        ServiceResponseDto<LoginResponseDTO> LoginEmployee(LoginDto dto);

        ServiceResponseDto<LoginResponseDTO> RefreshToken(RefreshTokenDTO dto);

        ServiceResponseDto<LoginResponseDTO> MicrosoftLogin(string email);

        Task<ServiceResponseDto<string>> ForgotPasswordAsync(ForgotPasswordDto dto);
        
        Task<ServiceResponseDto<string>> ResetPasswordAsync(ResetPasswordDto dto);

        ActivateAccountResponseDTO ActivateAccount(ActivateAccountDTO dto);
    }
}
