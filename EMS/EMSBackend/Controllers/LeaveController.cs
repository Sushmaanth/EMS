using Dtos;
using Dtos.LeaveRequestDto;
using EMSBackend.Helpers;
using EMSBackend.Service.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMSBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveController : ControllerBase
    {
        private readonly ILeaveRequestService _leaveRequestService;

        public LeaveController(ILeaveRequestService leaveRequestService)
        {
           _leaveRequestService = leaveRequestService;
        }

        [Authorize(Roles = "Employee,Manager")]
        [HttpPost("apply")]
        public async Task<IActionResult> Apply([FromBody] ApplyLeaveDto dto)
        {
            int employeeId = User.GetEmployeeId();
            var result = await _leaveRequestService.ApplyLeaveAsync(employeeId,dto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpPut("review")]
        public async Task<IActionResult> Review([FromBody] ReviewLeaveDto dto)
        {
            int managerId = User.GetEmployeeId();
            var result = await _leaveRequestService.ReviewLeaveAsync(managerId,dto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }


        [Authorize(Roles = "Employee,Manager")]
        [HttpGet("my-leaves")]
        public async Task<IActionResult> MyLeaves()
        {
            int employeeId = User.GetEmployeeId();
            var result = await _leaveRequestService.GetMyLeavesAsync(employeeId);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }


        [Authorize(Roles = "Manager,Admin")]
        [HttpGet("team-leaves")]
        public async Task<IActionResult> TeamLeaves()
        {
            int managerId = User.GetEmployeeId();

            var result = await _leaveRequestService.GetTeamLeavesAsync(managerId);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
