using Azure.Core;
using EMSFrontend.Api.Abstraction;
using Microsoft.AspNetCore.Mvc;

namespace EMSFrontend.Controllers
{
    public class ValidationController : Controller
    {
        private readonly IValidationRequest _validationrequest;

        public ValidationController(IValidationRequest request)
        {
            _validationrequest = request;
        }

        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> CheckEmail(string emailId)
        {
            bool valid =
                await _validationrequest.CheckEmailExistsAsync(emailId);

            return valid
                ? Json(true)
                : Json("Email already exists");
        }

        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> CheckMobile(long mobile)
        {
            bool valid =
                await _validationrequest.CheckMobileExistsAsync(mobile);

            return valid
                ? Json(true)
                : Json("Mobile already exists");
        }

    }
}
