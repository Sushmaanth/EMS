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
            var result = userRepository.AccountActivation(dto);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return CreatedAtAction(nameof(ActivateAccount), result);
        }

        [Route("login")]
        [HttpPost]
        public IActionResult Login([FromBody]LoginDto dto)
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


        [Route("refresh-token")]
        [HttpPost]
        public IActionResult RefreshToken(RefreshTokenDTO dto)
        {
            try
            {
                var result = authService.RefreshToken(dto);

                return Ok(result);
            }
            catch (Exception e)
            {
                return Problem($"Exception: {e.Message} and Inner Exception : {e.InnerException?.Message}");
            }
        }
        [Route("microsoft-login")]
        [HttpPost]
        public IActionResult MicrosoftLogin([FromBody] string email)
        {
            try
            {
                var result = authService.MicrosoftLogin(email);
                return Ok(result);
            }
            catch (Exception e)
            {
                return Problem($"Exception: {e.Message} and Inner Exception : {e.InnerException?.Message}");
            }
        }

        [Route("forgot-password")]
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
        {
            try
            {
                var result = await authService.ForgotPasswordAsync(dto);

                return Ok(result);
            }
            catch (Exception e)
            {
                return Problem($"Exception: {e.Message} and Inner Exception : {e.InnerException?.Message}");
            }
        }

        [Route("reset-password")]
        [HttpPost]

        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto) 
        {
            try
            {
                var result = await authService.ResetPasswordAsync(dto);
                return Ok(result);
            }
            catch (Exception e)
            {
                return Problem($"Exception: {e.Message} and Inner Exception : {e.InnerException?.Message}");
            }
        }
    }
}
