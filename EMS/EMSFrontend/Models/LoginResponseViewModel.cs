namespace EMSFrontend.Models
{
    public class LoginResponseViewModel
    {
            public string Token { get; set; }

            public string RefreshToken { get; set; }

            public string EmailId { get; set; }

            public string Role { get; set; }
    }
}
