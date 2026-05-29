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
        public ServiceResponseDto<LoginResponseDTO> LoginEmployee(LoginDto dto)
        {

            var user = _authrepository.GetUserByEmail(dto.Email);

            if (user == null)
            {
                return new ServiceResponseDto<LoginResponseDTO>
                {
                    Success = false,
                    Message = "Invalid email"
                };
            }

            bool isValidPassword = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password) == PasswordVerificationResult.Success;

            if (!isValidPassword)
            {
                return new ServiceResponseDto<LoginResponseDTO>
                {
                    Success = false,
                    Message = "Invalid password"
                };
            }
            if (!user.IsActive)
            {
                return new ServiceResponseDto<LoginResponseDTO>
                {
                    Success = false,
                    Message = "User account inactive"
                };
            }

            string token = _tokenService.GenerateToken(user);

            string refreshToken = _tokenService.GenerateRefreshToken();

            _authrepository.UpdateRefreshToken(user, refreshToken, DateTime.Now.AddMinutes(30));


            return new ServiceResponseDto<LoginResponseDTO>
            {
                Success = true,

                Data = new LoginResponseDTO
                {
                    Token = token,
                    EmailId = user.EmailId,
                    Role = user.Role.RoleName,
                    RefreshToken = refreshToken
                }
            };

        }

        public ServiceResponseDto<LoginResponseDTO> RefreshToken(RefreshTokenDTO dto)
        {

            var principal = _tokenService.GetPrincipalFromExpiredToken(dto.AccessToken);

            string email = principal.FindFirst(ClaimTypes.Email)?.Value;

            var user = _authrepository.GetUserByEmail(email);

            if (user == null)
            {
                return new ServiceResponseDto<LoginResponseDTO>
                {
                    Success = false,
                    Message = "User not found"
                };
            }

            if (user.RefreshToken != dto.RefreshToken)
            {
                return new ServiceResponseDto<LoginResponseDTO>
                {
                    Success = false,
                    Message = "Invalid refresh token"
                };
            }

            if (user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return new ServiceResponseDto<LoginResponseDTO>
                {
                    Success = false,
                    Message = "Refresh token expired"
                };
            }

            string newAccessToken = _tokenService.GenerateToken(user);

            string newRefreshToken = _tokenService.GenerateRefreshToken();

            _authrepository.UpdateRefreshToken(user, newRefreshToken, DateTime.Now.AddMinutes(30));


            return new ServiceResponseDto<LoginResponseDTO>
            {
                Success = true,
                Message = "Logged in succesfully",
                Data = new LoginResponseDTO
                {
                    Token = newAccessToken,

                    RefreshToken = newRefreshToken,

                    EmailId = user.EmailId,

                    Role = user.Role.RoleName
                }
            };
        }

        public ServiceResponseDto<LoginResponseDTO> MicrosoftLogin(string email)
        {

            var user = _authrepository.GetUserByEmail(email);

            if (user == null)
            {
                throw new Exception("User not registered in EMS");
            }

            if (!user.IsActive)
            {
                throw new Exception("User account inactive");
            }

            string token = _tokenService.GenerateToken(user);

            string refreshToken = _tokenService.GenerateRefreshToken();

            _authrepository.UpdateRefreshToken(user, refreshToken, DateTime.Now.AddMinutes(30));


            return new ServiceResponseDto<LoginResponseDTO>
            {
                Success = true,
                Message = "Login successful",

                Data = new LoginResponseDTO
                {
                    Token = token,
                    RefreshToken = refreshToken,
                    EmailId = user.EmailId,
                    Role = user.Role.RoleName
                }
            };
        }

        public async Task<ServiceResponseDto<string>> ForgotPasswordAsync(ForgotPasswordDto dto)
        {

            var user = _authrepository.GetUserByEmail(dto.Email);

            if (user != null)
            {
                Random random = new Random();

                string otp = random.Next(100000, 999999).ToString();

                _authrepository.UpdatePasswordResetOtp(user, otp, DateTime.Now.AddMinutes(2));

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
            return new ServiceResponseDto<string>
            {
                Success = true,
                Message = "If the email exists, OTP has been sent"
            };
        }

    

        public async Task<ServiceResponseDto<string>> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = _authrepository.GetUserByEmail(dto.Email);

            if (user == null)
            {
                return new ServiceResponseDto<string>
                {
                    Success = false,
                    Message = "Invalid user"
                };
            }

            if (user.PasswordResetOtp != dto.Otp)
            {
                _authrepository.IncrementOtpFailedAttempts(user);

                if (user.OtpFailedAttempts >= 5)
                {
                    _authrepository.ClearOtp(user);

                    _authrepository.Save();

                    return new ServiceResponseDto<string>
                    {
                        Success = false,
                        Message = "Too many invalid attempts"
                    };
                }

                _authrepository.Save();

                return new ServiceResponseDto<string>
                {
                    Success = false,
                    Message = "Invalid OTP"
                };
            }

            if (user.PasswordResetOtpExpiry < DateTime.Now)
            {
                return new ServiceResponseDto<string>
                {
                    Success = false,
                    Message = "OTP expired"
                };
            }

            string hashedPassword = _passwordHasher.HashPassword(user, dto.NewPassword);

            _authrepository.UpdatePassword(user, hashedPassword);

            _authrepository.ClearOtp(user);

            _authrepository.ResetOtpAttempts(user);

            _authrepository.Save();

            return new ServiceResponseDto<string>
            {
                Success = true,
                Message = "Password reset successfully"
            };
        }

        public ActivateAccountResponseDTO ActivateAccount(ActivateAccountDTO dto)
        {
            return _authrepository.AccountActivation(dto);
        }
    }
}

