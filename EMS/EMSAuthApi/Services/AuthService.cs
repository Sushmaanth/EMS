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
        public LoginResponseDTO LoginEmployee(LoginDto dto)
        {
            try
            {
                var user = _appcontext.Users.Include(u => u.Role)
                  .FirstOrDefault(u => u.EmailId == dto.Email);

                if (user == null)
                {
                    throw new Exception("Invalid Email");
                }

                bool isValidPassword = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password) == PasswordVerificationResult.Success;

                if (!isValidPassword)
                {
                    throw new Exception("Invalid Password");
                }
                if (!user.IsActive)
                {
                    throw new Exception("User account inactive");
                }

                string token = _tokenService.GenerateToken(user);

                string refreshToken = _tokenService.GenerateRefreshToken();

                user.RefreshToken = refreshToken;

                user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(2);

                _appcontext.SaveChanges();
                return new LoginResponseDTO
                {
                    Token = token,
                    EmailId = user.EmailId,
                    Role = user.Role.RoleName,
                    RefreshToken = refreshToken
                };
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public LoginResponseDTO RefreshToken(RefreshTokenDTO dto)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(dto.AccessToken);

            string email = principal.FindFirst(ClaimTypes.Email)?.Value;

            var user = _appcontext.Users
                .Include(u => u.Role)
                .FirstOrDefault(u =>
                u.EmailId == email);


            if (user == null)
            {
                throw new Exception("User not found");
            }

            if (user.RefreshToken != dto.RefreshToken)
            {
                throw new Exception("Invalid refresh token");
            }

            if (user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                throw new Exception("Refresh token expired");
            }

            string newAccessToken = _tokenService.GenerateToken(user);

            string newRefreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;

            user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(1);

            _appcontext.SaveChanges();

            return new LoginResponseDTO
            {
                Token = newAccessToken,

                RefreshToken = newRefreshToken,

                EmailId = user.EmailId,

                Role = user.Role.RoleName
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
                string resetToken = Guid.NewGuid().ToString();

                user.PasswordResetToken = resetToken;

                user.ResetTokenExpiry =
                    DateTime.Now.AddMinutes(2);

                _appcontext.SaveChanges();

                string resetLink = $"https://localhost:7193/Auth/ResetPassword?token={resetToken}";

                string body = $@"
                            <div style='font-family:Arial,sans-serif; background-color:#f4f6f8; padding:30px;'>
                                <div style='max-width:500px; margin:auto; background:#ffffff; padding:30px; border-radius:10px; text-align:center; box-shadow:0 2px 10px rgba(0,0,0,0.1);'>
                                    <h2 style='color:#2563eb; margin-bottom:10px;'>
                                        Password Reset Request
                                    </h2>
                                    <p style='color:#555; font-size:15px; line-height:1.5;'>
                                        We received a request to reset your password.<br/>
                                        Click the button below to continue.
                                    </p>
                                    <a href='{{resetLink}}'
                                       style='display:inline-block;
                                              margin-top:20px;
                                              padding:12px 25px;
                                              background:#2563eb;
                                              color:#fff;
                                              text-decoration:none;
                                              border-radius:6px;
                                              font-weight:bold;'>
                                        Reset Password
                                    </a>
                                    <p style='margin-top:25px; font-size:12px; color:#888;'>
                                        This link will expire in 2 minutes for security reasons.
                                    </p>
                                    <hr style='margin:25px 0; border:none; border-top:1px solid #eee;' />
                                    <p style='font-size:11px; color:#aaa;'>
                                        If you did not request this, you can safely ignore this email.
                                    </p>
                                </div>
                            </div>";

                await _emailService.SendEmailAsync(
                    user.EmailId,
                    "Reset Password",
                    body);
            }
            return "If the email exists, a reset link has been sent";
        }

        public async Task<string> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = _appcontext.Users.FirstOrDefault(u => u.PasswordResetToken == dto.Token);

            if(user == null)
            {
                return "Invalid token";
            }
            if (user.ResetTokenExpiry< DateTime.Now)
            {
                return "Token Expired";
            }

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.NewPassword);

            user.PasswordResetToken = null;
            user.ResetTokenExpiry = null;

            _appcontext.SaveChanges();

            return "Password reset successfully";
        }
    }
}
