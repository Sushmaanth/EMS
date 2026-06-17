using Dtos;
using EMSFrontend.Api.Abstraction;
using EMSFrontend.Models;
using Humanizer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace EMSFrontend.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthRequest authRequest;
        private readonly IConfiguration _configuration;

        public AuthController( IAuthRequest authRequest, IConfiguration configuration)
        {
            this.authRequest = authRequest;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var result = await authRequest.LoginAsync(model);

                HttpContext.Session.SetString("JWToken", result.Token);

                HttpContext.Session.SetString("RefreshToken", result.RefreshToken);

                HttpContext.Session.SetString("Role", result.Role);

                HttpContext.Session.SetString("EmailId", result.EmailId);

                HttpContext.Session.SetInt32("EmployeeId", result.EmployeeId);

                HttpContext.Session.SetString("EmployeeName",result.EmployeeName);

            TempData["SuccessfullyUserLoggegIn"] = "Login Successfully";

            if (result.Role == "Manager")
            {
                return RedirectToAction("TeamLeaves", "Leave");
            }

            if (result.Role == "Employee")
            {
                return RedirectToAction("Dashboard", "Employee");
            }

            return RedirectToAction("Index", "Admin");
        }

        [HttpGet]
        public IActionResult ActivateAccount()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ActivateAccount(AccountActivationViewModel model)
        {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                
                await authRequest.ActivateAccountAsync(model);

                TempData["SuccessfullyAccountActivated"] = "Account Activated Successfully";
                return RedirectToAction("Login", "Auth");
        }

        public IActionResult MicrosoftLogin()
        {
            var redirectUrl = Url.Action("MicrosoftResponse", "Auth");

            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };

            return Challenge(properties,OpenIdConnectDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> MicrosoftResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded)
            {
                return RedirectToAction("Login");
            }

            //foreach (var claim in result.Principal.Claims)
            //{
            //    Debug.WriteLine(
            //        $"{claim.Type} = {claim.Value}");
            //}

            var email = result.Principal.FindFirst("preferred_username")?.Value;

            var loginResult =await authRequest.MicrosoftLoginAsync(email);

            HttpContext.Session.SetString("JWToken",loginResult.Token);

            HttpContext.Session.SetString("RefreshToken",loginResult.RefreshToken);

            HttpContext.Session.SetString("Role",loginResult.Role);

            HttpContext.Session.SetString("EmailId", loginResult.EmailId);

            HttpContext.Session.SetInt32("EmployeeId",loginResult.EmployeeId);

            if (loginResult.Role == "Employee")
            {
                return RedirectToAction("Dashboard","Employee");
            }

            return RedirectToAction("Index","Admin");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await authRequest.SendForgotPasswordAsync(model);

            TempData["OTPSentSuccessMessage"] = result;
            return RedirectToAction("ResetPassword");
        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await authRequest.SendResetPasswordAsync(model);

            if (result == "Invalid Otp")
            {
                ModelState.AddModelError("Otp", result);
                return View(model);
            }
            if (result == "Otp Expired")
            {
                ModelState.AddModelError("Otp", result);
                return View(model);
            }
            if (result == "Too many invalid attempts. Please request new OTP.")
            {
                ModelState.AddModelError("Otp", result);
                return View(model);
            }
            if (result == "Invalid User")
            {
                ModelState.AddModelError("Email", result);
                return View(model);
            }


            TempData["PasswordSuccessResetMessage"] = result;
            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["loggedOutSuccessfully"] = "Logged out successfully";
            return RedirectToAction("Login","Auth");
        }


    }
}
