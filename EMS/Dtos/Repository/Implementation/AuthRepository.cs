using Dtos.Repository.Abstraction;
using Entities;
using Entities.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dtos.Repository.Implementation
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AuthRepository(AppDbContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }
        public User? GetUserByEmail(string email)
        {
            return _context.Users.Include(u => u.Employee).Include(u => u.Role).FirstOrDefault(u => u.EmailId == email);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void UpdateRefreshToken(User user, string refreshToken, DateTime expiryTime)
        {
            user.RefreshToken = refreshToken;

            user.RefreshTokenExpiryTime = expiryTime;

            _context.SaveChanges();
        }

        public void IncrementOtpFailedAttempts(User user)
        {
            user.OtpFailedAttempts++;
        }

        public void ClearOtp(User user)
        {
            user.PasswordResetOtp = null;
            user.PasswordResetOtpExpiry = null;
        }

        public void UpdatePassword(User user, string passwordHash)
        {
            user.PasswordHash = passwordHash;
        }

        public void UpdatePasswordResetOtp(User user, string otp, DateTime expiryTime)
        {
            user.PasswordResetOtp = otp;
            user.PasswordResetOtpExpiry = expiryTime;
            _context.SaveChanges();
        }

        public void ResetOtpAttempts(User user)
        {
            user.OtpFailedAttempts = 0;
        }

        public ActivateAccountResponseDTO AccountActivation(ActivateAccountDTO data)
        {
            try
            {
                var employeeExists = _context.Employees.FirstOrDefault(e => e.EmailId == data.EmailId);

                if (employeeExists == null)
                {
                    return new ActivateAccountResponseDTO
                    {
                        Success = false,
                        Message = "Employee email not found"
                    };
                }

                var employeeRole = _context.Role
                    .FirstOrDefault(e => e.RoleName == "User");

                if (employeeRole == null)
                {
                    throw new Exception("Role configuration missing");
                }

                var userExists = _context.Users
                .Any(u => u.EmailId == data.EmailId);

                if (userExists)
                {
                    return new ActivateAccountResponseDTO
                    {
                        Success = false,
                        Message = "Account already activated"
                    };
                }

                User user = new()
                {
                    EmailId = data.EmailId,
                    IsActive = true,
                    EmployeeId = employeeExists.Id,
                    RoleId = employeeRole.Id = 2
                };

                user.PasswordHash = _passwordHasher.HashPassword(user, data.Password);

                _context.Add(user);
                int result = _context.SaveChanges();

                return new ActivateAccountResponseDTO
                {
                    Success = true,
                    EmailId = user.EmailId,
                    Message = "Account activated successfully"
                };
            }
            catch
            {
                throw;
            }
        }
    }
}
