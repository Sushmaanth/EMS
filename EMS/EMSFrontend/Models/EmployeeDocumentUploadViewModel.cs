using System.ComponentModel.DataAnnotations;

namespace EMSFrontend.Models
{
    public class EmployeeDocumentUploadViewModel
    {
        [Required]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Document Type is required")]
        public int DocumentTypeId { get; set; }

        [Required(ErrorMessage = "Please select a file")]
        public IFormFile File { get; set; }
    }
}
