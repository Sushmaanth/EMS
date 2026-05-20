using EMSFrontend.Api.Abstraction;
using EMSFrontend.Models;
using Humanizer;
using Microsoft.AspNetCore.Mvc;

namespace EMSFrontend.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthRequest authRequest;

        public AuthController( IAuthRequest authRequest)
        {
            this.authRequest = authRequest;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                var result =await authRequest.LoginAsync(model);

                HttpContext.Session.SetString("JWToken",result.Token);

                HttpContext.Session.SetString("RefreshToken", result.RefreshToken);

                HttpContext.Session.SetString("Role",result.Role);

                HttpContext.Session.SetString("EmailId",result.EmailId);

                TempData["SuccessfullyUserLoggegIn"] = "Login Successfully";

                if (result.Role =="Admin")
                {
                    return RedirectToAction("Index","Home");
                }

                return RedirectToAction("Dashboard","Employee");
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public IActionResult ActivateAccount()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ActivateAccount(AccountActivationViewModel model)
        {
            try
            {
                await authRequest.ActivateAccountAsync(model);

                TempData["SuccessfullyAccountActivated"] = "Account Activated Successfully";
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login","Auth");
        }
    }
}
