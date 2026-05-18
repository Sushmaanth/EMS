using System;
using System.Collections.Generic;
using System.Text;

namespace Dtos
{
    public class LoginResponseDTO
    {
        public string Token { get; set; }

        public string EmailId { get; set; }

        public string Role { get; set; }
    }
        
}
