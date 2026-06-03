using Azure.Core;
using EMSFrontend.Api.Abstraction;
using Microsoft.AspNetCore.Mvc;

namespace EMSFrontend.Controllers
{
    public class ValidationController : Controller
    {
        private readonly IRequest _request;

        public ValidationController(IRequest request)
        {
            _request = request;
        }

        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> CheckEmail(string emailId)
        {
            bool valid =
                await _request.CheckEmailExistsAsync(emailId);

            return valid
                ? Json(true)
                : Json("Email already exists");
        }

        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> CheckMobile(long mobile)
        {
            bool valid =
                await _request.CheckMobileExistsAsync(mobile);

            return valid
                ? Json(true)
                : Json("Mobile already exists");
        }

    }
}
