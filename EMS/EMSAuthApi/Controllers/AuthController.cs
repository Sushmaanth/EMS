using Dtos;
using Dtos.Repository.Abstraction;
using EMSAuthApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EMSAuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository<ActivateAccountDTO> userRepository;
        private readonly AuthService authService;

        public AuthController(IUserRepository<ActivateAccountDTO> userRepository,AuthService authService)
        {
            this.authService = authService;
            this.userRepository = userRepository;
        }

        [Route("activate-account")]
        [HttpPost]
        public IActionResult ActivateAccount([FromBody] ActivateAccountDTO dto)
        {
            try
            {
                var result = userRepository.AccountActivation(dto);

                return CreatedAtAction(nameof(ActivateAccount), result);
            }
            catch (Exception e)
            {
                return Problem($"Exception: {e.Message} and Inner Exception : {e.InnerException?.Message}");
            }
        }

        [Route("login")]
        [HttpPost]
        public IActionResult Login(LoginDto dto)
        {
            try
            {
                var result = authService.LoginEmployee(dto);

                return Ok(result);
            }
            catch (Exception e)
            {
                return Problem($"Exception: {e.Message} and Inner Exception : {e.InnerException?.Message}");
            }
        }
    }
}
