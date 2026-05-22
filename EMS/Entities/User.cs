using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
    public class User
    {
        public int Id { get; set; }

        public string EmailId { get; set; }

        public string PasswordHash { get; set; }

        public bool IsActive { get; set; } = false;

        public int RoleId { get; set; }
        public Role Role { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }

        public string? PasswordResetOtp { get; set; }

        public DateTime? PasswordResetOtpExpiry { get; set; }

        public int OtpFailedAttempts { get; set; }
    }
}
