using Dtos;
using Dtos.Repository.Abstraction;
using EMSAuthApi.Services.Abstraction;
using Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace EMSAuthApi.Services
{
    public class AuthService : IAuthService
    {

        private readonly IAuthRepository _authrepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;

        public AuthService(IAuthRepository repository, IPasswordHasher<User> passwordHasher, ITokenService tokenService, IEmailService emailService)
        {
            _authrepository = repository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _emailService = emailService;
        }
        public async Task<ServiceResponseDto<LoginResponseDTO>> LoginEmployee(LoginDto dto)
        {

            var user = await _authrepository.GetUserByEmailAsync(dto.Email);

            if (user == null)
            {
                return ServiceResponseDto<LoginResponseDTO>.Fail("Invalid Email or Password");
            }

            bool isValidPassword = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password) == PasswordVerificationResult.Success;

            if (!isValidPassword)
            {
                return ServiceResponseDto<LoginResponseDTO>.Fail("Invalid Email or Password");
            }
            if (!user.IsActive)
            {
                return ServiceResponseDto<LoginResponseDTO>.Fail("User account inactive");
            }

            string token = _tokenService.GenerateToken(user);

            string refreshToken = _tokenService.GenerateRefreshToken();

            _authrepository.UpdateRefreshToken(user, refreshToken, DateTime.UtcNow.AddDays(7)); 


            return ServiceResponseDto<LoginResponseDTO>.Ok(
                new LoginResponseDTO
                {
                    Token = token,
                    EmailId = user.EmailId,
                    Role = user.Role.RoleName,
                    RefreshToken = refreshToken,
                    EmployeeId = user.EmployeeId,
                    EmployeeName = user.Employee.Name
                },
                "Login successful"
            );

        }

        public async Task<ServiceResponseDto<LoginResponseDTO>> RefreshToken(RefreshTokenDTO dto)
        {

            var principal = _tokenService.GetPrincipalFromExpiredToken(dto.AccessToken);

            string email = principal.FindFirst(ClaimTypes.Email)?.Value;

            var user = await _authrepository.GetUserByEmailAsync(email);

            if (user == null)
            {
                return ServiceResponseDto<LoginResponseDTO>.Fail("User not found");
            }

            if (user.RefreshToken != dto.RefreshToken)
            {
                return ServiceResponseDto<LoginResponseDTO>.Fail("Invalid refresh token");
            }

            if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return ServiceResponseDto<LoginResponseDTO>.Fail("Refresh token expired");
            }

            string newAccessToken = _tokenService.GenerateToken(user);

            string newRefreshToken = _tokenService.GenerateRefreshToken();

            _authrepository.UpdateRefreshToken(user, newRefreshToken, DateTime.UtcNow.AddDays(7));


            return ServiceResponseDto<LoginResponseDTO>.Ok(
                new LoginResponseDTO
                {
                    Token = newAccessToken,
                    RefreshToken = newRefreshToken,
                    EmailId = user.EmailId,
                    Role = user.Role.RoleName,
                    EmployeeId = user.EmployeeId,
                    EmployeeName = user.Employee.Name
                },
                "Logged in successfully"
            );
        }

        public async Task<ServiceResponseDto<LoginResponseDTO>> MicrosoftLogin(string email)
        {

            var user = await _authrepository.GetUserByEmailAsync(email);

            if (user == null)
            {
                return ServiceResponseDto<LoginResponseDTO>.Fail("User not registered in EMS");
            }

            if (!user.IsActive)
            {
                return ServiceResponseDto<LoginResponseDTO>.Fail("User account inactive");
            }

            string token = _tokenService.GenerateToken(user);

            string refreshToken = _tokenService.GenerateRefreshToken();

            _authrepository.UpdateRefreshToken(user, refreshToken, DateTime.UtcNow.AddDays(7));


            return ServiceResponseDto<LoginResponseDTO>.Ok(
                new LoginResponseDTO
                {
                    Token = token,
                    RefreshToken = refreshToken,
                    EmailId = user.EmailId,
                    Role = user.Role.RoleName,
                    EmployeeId = user.EmployeeId
                },
                "Login successful"
            );
        }

        public async Task<ServiceResponseDto<string>> ForgotPasswordAsync(ForgotPasswordDto dto)
        {

            var user = await _authrepository.GetUserByEmailAsync(dto.Email);

            if (user != null)
            {
                Random random = new Random();

                string otp = random.Next(100000, 999999).ToString();

                _authrepository.UpdatePasswordResetOtp(user, otp, DateTime.UtcNow.AddMinutes(30));

                string body = $@"
                            <div style='font-family:Arial;padding:20px;'>
                                <h2>Password Reset OTP</h2>
                                <p>
                                    Use the OTP below to reset your password:
                                </p>
                                <h1 style='letter-spacing:5px;
                                           color:#2563eb;'>
                                    {otp}
                                </h1>
                                <p>
                                    OTP expires in 2 minutes.
                                </p>
                            </div>";

                await _emailService.SendEmailAsync(
                    user.EmailId,
                    "Reset Password OTP",
                    body);
            }
            return ServiceResponseDto<string>.Ok(null,"If the email exists, OTP has been sent");
        }

        public async Task<ServiceResponseDto<string>> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _authrepository.GetUserByEmailAsync(dto.Email);

            if (user == null)
            {
                return ServiceResponseDto<string>.Fail("Invalid user");
            }

            if (user.PasswordResetOtpExpiry < DateTime.UtcNow)
            {
                _authrepository.ClearOtp(user);
                _authrepository.Save();
                return ServiceResponseDto<string>.Fail("OTP expired");
            }

            if (user.PasswordResetOtp != dto.Otp)
            {
                _authrepository.IncrementOtpFailedAttempts(user);

                if (user.OtpFailedAttempts >= 5)
                {
                    _authrepository.ClearOtp(user);

                    _authrepository.Save();

                    return ServiceResponseDto<string>.Fail("Too many invalid attempts");
                }

                _authrepository.Save();

                return ServiceResponseDto<string>.Fail("Invalid OTP");
            }

            string hashedPassword = _passwordHasher.HashPassword(user, dto.NewPassword);

            _authrepository.UpdatePassword(user, hashedPassword);

            _authrepository.ClearOtp(user);

            _authrepository.ResetOtpAttempts(user);

            _authrepository.Save();

            return ServiceResponseDto<string>.Ok(null,"Password reset successfully");
        }

        public ActivateAccountResponseDTO ActivateAccount(ActivateAccountDTO dto)
        {
            return _authrepository.AccountActivation(dto);
        }
    }
}

