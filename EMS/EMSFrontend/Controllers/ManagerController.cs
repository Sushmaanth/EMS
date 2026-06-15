using Microsoft.AspNetCore.Mvc;

namespace EMSFrontend.Controllers
{
    public class ManagerController : Controller
    {
        public IActionResult LeaveApproval()
        {
            return View();
        }
    }
}
