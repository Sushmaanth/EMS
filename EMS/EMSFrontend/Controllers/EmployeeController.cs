using Microsoft.AspNetCore.Mvc;

namespace EMSFrontend.Controllers
{
    public class EmployeeController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
