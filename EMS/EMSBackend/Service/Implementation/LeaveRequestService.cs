using AutoMapper;
using Dtos;
using Dtos.LeaveRequestDto;
using Dtos.Repository.Abstraction;
using Dtos.Validation.Abstraction;
using EMSBackend.Service.Abstraction;
using Entities;
using Entities.Enums;

namespace EMSBackend.Service.Implementation
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly ILeaveRequestRepository _leaveRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;
        private readonly ILeaveValidation _leaveValidation;

        public LeaveRequestService(ILeaveRequestRepository leaveRepository,
                            IEmployeeRepository employeeRepository,
                            IMapper mapper,
                            ILeaveValidation leaveValidation)
        {
            _leaveRepository = leaveRepository;
            _employeeRepository = employeeRepository;
            _mapper = mapper;
            _leaveValidation = leaveValidation;
        
        }
        public async Task<ServiceResponseDto<LeaveRequestResponseDto>> ApplyLeaveAsync(ApplyLeaveDto dto)
        {
            var errors = await _leaveValidation.Validate(dto);

            if (errors.Any())
            {
                return ServiceResponseDto<LeaveRequestResponseDto>.Fail(
                                                        "Leave Validation Failed",
                                                        errors);
            }

            var employee = _employeeRepository.GetById(dto.EmployeeId);

            var leave = _mapper.Map<LeaveRequest>(dto);

            leave.EmployeeId = dto.EmployeeId;

            leave.ManagerId = employee.ManagerId!.Value;

            leave.Status = LeaveStatus.Pending;

            leave.AppliedAt = DateTime.UtcNow;

            var created = await _leaveRepository.ApplyLeaveRequestAsync(leave);

            var savedLeave = await _leaveRepository.GetLeaveRequestByIdAsync(created.Id);

            var response = _mapper.Map<LeaveRequestResponseDto>(
            savedLeave);

            return ServiceResponseDto<LeaveRequestResponseDto>.Ok(response,"Leave applied successfully");
        }

        public async Task<ServiceResponseDto<LeaveRequestResponseDto>>ReviewLeaveAsync(ReviewLeaveDto dto)
        {
            var leave = await _leaveRepository.GetLeaveRequestByIdAsync(dto.LeaveRequestId);

            if (leave == null)
            {
                return ServiceResponseDto<LeaveRequestResponseDto>.Fail("Leave request not found");
            }

            if (leave.Status != LeaveStatus.Pending)
            {
                return ServiceResponseDto<LeaveRequestResponseDto>.Fail("This leave request has already been reviewed");
            }

            leave.Status = dto.IsApproved ? LeaveStatus.Approved:LeaveStatus.Rejected;

            leave.ManagerComments = dto.Comments;

            leave.ReviewedAt = DateTime.UtcNow;

            await _leaveRepository.SaveLeaveAsync();

            return ServiceResponseDto<LeaveRequestResponseDto>.Ok(
                _mapper.Map<LeaveRequestResponseDto>(leave),
                dto.IsApproved
                    ? "Leave approved successfully"
                    : "Leave rejected successfully");
        }

        public async Task<ServiceResponseDto<ICollection<LeaveRequestResponseDto>>> GetMyLeavesAsync(int employeeId)
        {
            var employee = _employeeRepository.GetById(employeeId);

            if (employee == null)
            {
                return ServiceResponseDto<ICollection<LeaveRequestResponseDto>>.Fail(
                    "Employee not found");
            }

            var leaves = await _leaveRepository.GetEmployeesLeaveRequestsAsync(employeeId);

            var result = _mapper.Map<ICollection<LeaveRequestResponseDto>>(leaves);

            return ServiceResponseDto<ICollection<LeaveRequestResponseDto>>.Ok(result,
                "Leave requests fetched successfully");
        }

        public async Task<ServiceResponseDto<ICollection<LeaveRequestResponseDto>>> GetTeamLeavesAsync(int managerId)
        {
            var manager = _employeeRepository.GetById(managerId);

            if (manager == null)
            {
                return ServiceResponseDto<ICollection<LeaveRequestResponseDto>>.Fail(
                    "Manager not found");
            }

            var leaves = await _leaveRepository.GetByManagerAsync(managerId);

            var result = _mapper.Map<ICollection<LeaveRequestResponseDto>>(leaves);

            return ServiceResponseDto<ICollection<LeaveRequestResponseDto>>.Ok(result,
                "Team leave requests fetched successfully");
        }
    }
}
