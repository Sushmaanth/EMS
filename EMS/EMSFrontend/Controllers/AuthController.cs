using Dtos;
using EMSFrontend.Api.Abstraction;
using EMSFrontend.Models;
using Humanizer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

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
                if (e.Message.Contains("Invalid Email") ||
                         e.Message.Contains("Invalid Password"))
                {
                    ModelState.AddModelError("Password", "Invalid Email or Password");
                }
                else
                {
                    ModelState.AddModelError("", e.Message);
                }
                return View(model);
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
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                
                await authRequest.ActivateAccountAsync(model);

                TempData["SuccessfullyAccountActivated"] = "Account Activated Successfully";
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception e)
            {
                if (e.Message.Contains("Employee email not found"))
                {
                    ModelState.AddModelError("EmailId", "Email not found");
                }
                else if (e.Message.Contains("Account already activated"))
                {
                    ModelState.AddModelError("EmailId","Account already activated");
                }
                else
                {
                    ModelState.AddModelError("", e.Message);
                }
                return View(model);
            }
        }

        public IActionResult MicrosoftLogin()
        {
            var redirectUrl = Url.Action("MicrosoftResponse", "Auth");

            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };

            return Challenge(properties, MicrosoftAccountDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> MicrosoftResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded)
            {
                return RedirectToAction("Login");
            }

            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;

            var name = result.Principal.FindFirst(ClaimTypes.Name)?.Value;

            var loginResult =await authRequest.MicrosoftLoginAsync(email);

            HttpContext.Session.SetString("JWToken",loginResult.Token);

            HttpContext.Session.SetString("RefreshToken",loginResult.RefreshToken);

            HttpContext.Session.SetString("Role",loginResult.Role);

            HttpContext.Session.SetString("Email",loginResult.EmailId);

            if (loginResult.Role == "User")
            {
                return RedirectToAction("Dashboard","Employee");
            }

            return RedirectToAction("Index","Home");
        }

        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            var model = new ResetPasswordDto
            {
                Token = token
            };

            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login","Auth");
        }


    }
}
