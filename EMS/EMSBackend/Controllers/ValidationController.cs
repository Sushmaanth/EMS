using Entities.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EMSBackend.Controllers
{
    public class ValidationController: ControllerBase
    {
        private readonly AppDbContext _context;
        public ValidationController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("email")]
        public async Task<IActionResult> CheckEmail(string email)
        {
            bool exists = await _context.Employees
                .AnyAsync(x => x.EmailId.ToLower().Trim() ==
                               email.ToLower().Trim());

            return Ok(!exists);
        }

        [HttpGet("mobile")]
        public async Task<IActionResult> CheckMobile(long mobile)
        {
            bool exists = await _context.Employees
                .AnyAsync(x => x.Mobile == mobile);

            return Ok(!exists);
        }
    }
}
