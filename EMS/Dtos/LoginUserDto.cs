using System;
using System.Collections.Generic;
using System.Text;

namespace Dtos
{
    public class LoginUserDto
    {
        public int UserId { get; set; }

        public int EmployeeId { get; set; }

        public string EmailId { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string EmployeeName { get; set; } = string.Empty;

        public string RoleName { get; set; } = string.Empty;
    }
}
