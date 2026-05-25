using Dtos;
using Entities;
using Entities.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EMSAuthApi.Services
{
    public class AuthService
    {
        private readonly AppDbContext _appcontext;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly TokenService _tokenService;
        private readonly EmailService _emailService;

        public AuthService(AppDbContext context, IPasswordHasher<User> passwordHasher, TokenService tokenService, EmailService emailService)
        {
            _appcontext = context;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _emailService = emailService;
        }
        public ServiceResponseDto<LoginResponseDTO> LoginEmployee(LoginDto dto)
        {
            try
            {
                var user = _appcontext.Users.Include(u => u.Role)
                  .FirstOrDefault(u => u.EmailId == dto.Email);

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

                user.RefreshToken = refreshToken;

                user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(2);

                _appcontext.SaveChanges();

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
            catch
            {
                throw;
            }
        }

        public ServiceResponseDto<LoginResponseDTO> RefreshToken(RefreshTokenDTO dto)
        {
            try
            {
                var principal = _tokenService.GetPrincipalFromExpiredToken(dto.AccessToken);

                string email = principal.FindFirst(ClaimTypes.Email)?.Value;

                var user = _appcontext.Users
                    .Include(u => u.Role)
                    .FirstOrDefault(u =>
                    u.EmailId == email);

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

                user.RefreshToken = newRefreshToken;

                user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(1);

                _appcontext.SaveChanges();

                return new ServiceResponseDto<LoginResponseDTO>
                {
                    Success = true,
                    Data = new LoginResponseDTO
                    {
                        Token = newAccessToken,

                        RefreshToken = newRefreshToken,

                        EmailId = user.EmailId,

                        Role = user.Role.RoleName
                    }
                };
            }
            catch
            {

                throw;
            }

        }

        public ServiceResponseDto<LoginResponseDTO> MicrosoftLogin(string email)
        {
            try
            {
                var user = _appcontext.Users
                    .Include(u => u.Role)
                    .FirstOrDefault(u =>
                        u.EmailId == email);

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

                user.RefreshToken = refreshToken;

                user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(30);

                _appcontext.SaveChanges();

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

            catch
            {
                throw;
            }
        }

        public async Task<ServiceResponseDto<string>> ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            try
            {
                var user = _appcontext.Users
                .FirstOrDefault(u => u.EmailId == dto.Email);

                if (user != null)
                {
                    Random random = new Random();

                    string otp = random.Next(100000, 999999).ToString();

                    user.PasswordResetOtp = otp;

                    user.PasswordResetOtpExpiry = DateTime.Now.AddMinutes(2);

                    _appcontext.SaveChanges();

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
            catch
            {

                throw;
            }

        }

        public async Task<ServiceResponseDto<string>> ResetPasswordAsync(ResetPasswordDto dto)
        {
            try
            {
                var user = _appcontext.Users.FirstOrDefault(u => u.EmailId == dto.Email);

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
                    user.OtpFailedAttempts++;

                    if (user.OtpFailedAttempts >= 5)
                    {
                        user.PasswordResetOtp = null;
                        user.PasswordResetOtpExpiry = null;

                        _appcontext.SaveChanges();

                        return new ServiceResponseDto<string>
                        {
                            Success = false,
                            Message = "Too many invalid attempts"
                        };
                    }

                    _appcontext.SaveChanges();

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

                user.PasswordHash = _passwordHasher.HashPassword( user,dto.NewPassword);

                user.PasswordResetOtp = null;
                user.PasswordResetOtpExpiry = null;
                user.OtpFailedAttempts = 0;

                _appcontext.SaveChanges();

                return new ServiceResponseDto<string>
                {
                    Success = true,
                    Message = "Password reset successfully"
                };
            }
            catch
            {
                throw;
            }
        }
    }
}

