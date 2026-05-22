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

        public AuthService(AppDbContext context, IPasswordHasher<User> passwordHasher,TokenService tokenService, EmailService emailService)
        {
            _appcontext = context;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _emailService = emailService;
        }
        public ServiceResponseDto<LoginResponseDTO> LoginEmployee(LoginDto dto)
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

        internal object MicrosoftLogin(string email)
        {
            try
            {
                var user = _appcontext.Users
                    .Include(u => u.Role)
                    .FirstOrDefault(u =>
                        u.EmailId == email);

                if (user == null)
                {
                    throw new Exception(
                        "User not registered in EMS");
                }

                if (!user.IsActive)
                {
                    throw new Exception(
                        "User account inactive");
                }

                string token = _tokenService.GenerateToken(user);

                string refreshToken = _tokenService.GenerateRefreshToken();

                user.RefreshToken = refreshToken;

                user.RefreshTokenExpiryTime =DateTime.Now.AddMinutes(30);

                _appcontext.SaveChanges();

                return new LoginResponseDTO
                {
                    Token = token,

                    RefreshToken = refreshToken,

                    EmailId = user.EmailId,

                    Role = user.Role.RoleName
                };
            }

            catch (Exception e)
            {
                throw new Exception($"Exception: {e.Message}");
            }
        }

        public async Task<string> ForgotPasswordAsync(ForgotPasswordDto dto)
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
            return "If the email address exists, an OTP has been sent";
        }

        public async Task<string> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = _appcontext.Users.FirstOrDefault(u => u.EmailId == dto.Email);

            if (user == null)
            {
                return "Invalid User";
            }

            if (user.PasswordResetOtp != dto.Otp)
            {
                user.OtpFailedAttempts++;
                if (user.OtpFailedAttempts >=5)
                {
                    user.PasswordResetOtp = null;
                    user.PasswordResetOtpExpiry = null;
                    _appcontext.SaveChanges();
                    return "Too many invalid attempts. Please request new OTP.";
                }
                _appcontext.SaveChanges();
                return "Invalid Otp";
            }
            if (user.PasswordResetOtpExpiry < DateTime.Now)
            {
                return "Otp Expired";
            }
            
                user.PasswordHash = _passwordHasher.HashPassword(user, dto.NewPassword);

                user.PasswordResetOtp = null;
                user.PasswordResetOtpExpiry = null;

                _appcontext.SaveChanges();
            
            return "Password reset successfully";
        }
    }
}
