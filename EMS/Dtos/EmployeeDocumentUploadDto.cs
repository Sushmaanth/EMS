using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Dtos
{
    public class EmployeeDocumentUploadDto
    {
        [Required(ErrorMessage = "Employee Id is required")]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Document Type is required")]
        public int DocumentTypeId { get; set; }

        [Required(ErrorMessage = "Please upload the file")]
        public IFormFile File { get; set; }
    }
}
