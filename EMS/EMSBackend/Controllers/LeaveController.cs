using Dtos.LeaveRequestDto;
using EMSBackend.Service.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
            var result = await _leaveRequestService.ApplyLeaveAsync(dto);

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
            var result = await _leaveRequestService.ReviewLeaveAsync(dto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }


        [Authorize(Roles = "Employee,Manager")]
        [HttpGet("my-leaves/{employeeId}")]
        public async Task<IActionResult> MyLeaves(int employeeId)
        {
            var result = await _leaveRequestService.GetMyLeavesAsync(employeeId);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }


        [Authorize(Roles = "Manager,Admin")]
        [HttpGet("team-leaves/{managerId}")]
        public async Task<IActionResult> TeamLeaves(
            int managerId)
        {
            var result = await _leaveRequestService.GetTeamLeavesAsync(managerId);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
