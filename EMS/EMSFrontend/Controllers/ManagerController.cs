using Microsoft.AspNetCore.Mvc;

namespace EMSFrontend.Controllers
{
    public class ManagerController : Controller
    {
       public IActionResult Dashboard()
        {
            return View();
        }
    }
}
