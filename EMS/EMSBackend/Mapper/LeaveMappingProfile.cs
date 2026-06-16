using AutoMapper;
using Dtos.LeaveRequestDto;
using Entities;

namespace EMSBackend.Mapper
{
    public class LeaveMappingProfile: Profile
    {
        public LeaveMappingProfile()
        {
            // ApplyLeaveDto -> LeaveRequest
            CreateMap<ApplyLeaveDto, LeaveRequest>()
                    .ForMember(dest => dest.Id, opt => opt.Ignore())
                    .ForMember(dest => dest.Employee, opt => opt.Ignore())
                    .ForMember(dest => dest.Manager, opt => opt.Ignore());

            // LeaveRequest -> LeaveRequestResponseDto
            CreateMap<LeaveRequest, LeaveRequestResponseDto>()
                    .ForMember(
                        dest => dest.EmployeeName,
                        opt => opt.MapFrom(src =>
                            src.Employee != null
                                ? src.Employee.Name
                                : string.Empty))

                    .ForMember(
                        dest => dest.LeaveType,
                        opt => opt.MapFrom(src =>
                            src.LeaveType.ToString()))

                    .ForMember(
                        dest => dest.Status,
                        opt => opt.MapFrom(src =>
                            src.Status.ToString()))

                    .ForMember(
                        dest => dest.TotalDays,
                        opt => opt.MapFrom(src =>
                            src.EndDate.DayNumber -
                            src.StartDate.DayNumber + 1));
        }
    }
}
