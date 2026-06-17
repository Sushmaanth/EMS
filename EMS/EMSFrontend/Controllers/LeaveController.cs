using EMSFrontend.Api.Abstraction;
using EMSFrontend.Api.ApiException;
using EMSFrontend.Models.Leavemodels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMSFrontend.Controllers
{
    public class LeaveController : Controller
    {
        private readonly ILeaveApiRequest _leaveApiRequest;
        public LeaveController(ILeaveApiRequest leaveApiRequest)
        {
            _leaveApiRequest = leaveApiRequest;
        }

        [HttpGet]
        public async Task<IActionResult> Apply()
        {
            var model = new ApplyLeavePageViewModel
            {
                ApplyLeave = new ApplyLeaveViewModel(),
                MyLeaves = await _leaveApiRequest.GetMyLeavesAsync()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Apply(ApplyLeavePageViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.MyLeaves = await _leaveApiRequest.GetMyLeavesAsync();

                return View(model);
            }

            await _leaveApiRequest.ApplyLeaveAsync(model.ApplyLeave);

            TempData["LeaveAppliedSuccessMessage"] ="Leave applied successfully";

            return RedirectToAction(nameof(Apply));
        }

        [HttpGet("approval")]
        public async Task<IActionResult> TeamLeaves()
        {
            var leaves = await _leaveApiRequest.GetTeamLeavesAsync();

            return View(leaves);
        }

        [HttpPost]
        public async Task<IActionResult> Review(ReviewLeaveViewModel model)
        {
            Console.WriteLine("Review action hit");

            Console.WriteLine(model.LeaveRequestId);
            Console.WriteLine(model.IsApproved);
            Console.WriteLine(model.ManagerComments);

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState)
                {
                    foreach (var validationError in error.Value.Errors)
                    {
                        Console.WriteLine(
                            $"{error.Key} : {validationError.ErrorMessage}");
                    }
                }
                return RedirectToAction(nameof(TeamLeaves));
            }

            await _leaveApiRequest.ReviewLeaveAsync(model);

            TempData["SuccessMessage"] =
                model.IsApproved
                ? "Leave approved successfully"
                : "Leave rejected successfully";

            return RedirectToAction(nameof(TeamLeaves));
        }
    }
}
