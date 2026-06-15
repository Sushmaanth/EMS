using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dtos
{
    public class RoleDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [StringLength(200, ErrorMessage = "Role cannot exceed more than 200 characters")]
        public string RoleName { get; set; }
    }
}
