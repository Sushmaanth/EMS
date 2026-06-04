
namespace Dtos
{
    public class EmployeeExcelUploadDto
    {
        public string EmployeeCode { get; set; }

        public string Name { get; set; }

        public string Gender { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public string EmailId { get; set; }

        public long Mobile { get; set; }

        public decimal Salary { get; set; }

        public DateOnly DateOfJoining { get; set; }

        public string DepartmentName { get; set; }
    }
}
