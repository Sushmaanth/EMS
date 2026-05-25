using Dtos;
using Dtos.Repository.Abstraction;
using EMSAuthApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EMSAuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository<ActivateAccountDTO> _userRepository;
        private readonly AuthService _authService;

        public AuthController(IUserRepository<ActivateAccountDTO> userRepository,AuthService authService)
        {
            _authService = authService;
            _userRepository = userRepository;
        }


        [Route("activate-account")]
        [HttpPost]
        public IActionResult ActivateAccount([FromBody] ActivateAccountDTO dto)
        {
            var result = _userRepository.AccountActivation(dto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return CreatedAtAction(nameof(ActivateAccount), result);
        }

        [Route("login")]
        [HttpPost]
        public IActionResult Login([FromBody]LoginDto dto)
        {
            var result = _authService.LoginEmployee(dto);

            if (!result.Success)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }

        [Route("refresh-token")]
        [HttpPost]
        public IActionResult RefreshToken(RefreshTokenDTO dto)
        {
            var result = _authService.RefreshToken(dto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
          
        }

        [Route("microsoft-login")]
        [HttpPost]
        public IActionResult MicrosoftLogin([FromBody] string email)
        {
            var result = _authService.MicrosoftLogin(email);

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
