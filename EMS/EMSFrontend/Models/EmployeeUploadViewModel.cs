using Dtos;
using System.ComponentModel.DataAnnotations;

namespace EMSFrontend.Models
{
    public class EmployeeBulkUploadViewModel
    {
        [Required(ErrorMessage = "Please select a file.")]
        public IFormFile? File { get; set; }

        public EmployeeUploadExcelResponseDto? Result { get; set; }
    }
}
