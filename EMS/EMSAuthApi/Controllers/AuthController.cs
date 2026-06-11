using Dtos;
using Dtos.Repository.Abstraction;
using EMSAuthApi.Services.Abstraction;
using Microsoft.AspNetCore.Mvc;

namespace EMSAuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        [Route("activate-account")]
        [HttpPost]
        public IActionResult ActivateAccount([FromBody] ActivateAccountDTO dto)
        {
            var result = _authService.ActivateAccount(dto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return CreatedAtAction(nameof(ActivateAccount), result);
        }

        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody]LoginDto dto)
        {
            var result = await _authService.LoginEmployee(dto);

            if (!result.Success)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }

        [Route("refresh-token")]
        [HttpPost]
        public async Task<IActionResult> RefreshToken(RefreshTokenDTO dto)
        {
            var result = await _authService.RefreshToken(dto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
          
        }

        [Route("microsoft-login")]
        [HttpPost]
        public async Task<IActionResult> MicrosoftLogin([FromBody] string email)
        {
            var result = await _authService.MicrosoftLogin(email);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [Route("forgot-password")]
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
        {
            var result = await _authService.ForgotPasswordAsync(dto);

            if (!result.Success)
            {
               return BadRequest(result);
            }

            return Ok(result);
        }

        [Route("reset-password")]
        [HttpPost]

        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto) 
        {
           var result = await _authService.ResetPasswordAsync(dto);
           if (!result.Success)
           {
              return BadRequest(result);
           }
            return Ok(result);
        }
    }
}
