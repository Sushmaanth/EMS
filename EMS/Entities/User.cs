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
    }
}
