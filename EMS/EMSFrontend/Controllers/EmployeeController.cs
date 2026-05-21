using Microsoft.AspNetCore.Mvc;

namespace EMSFrontend.Controllers
{
    public class EmployeeController : Controller
    {
        public IActionResult Dashboard()
        {
            try
            {
                string token = HttpContext.Session.GetString("JWToken");
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "Auth");
                }
                return View();
            }
            catch (UnauthorizedAccessException)
            {
                TempData["SessionExpired"] = "Your session has expired. Please login again.";
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("Error", "Home");
            }
        }
    }
}
