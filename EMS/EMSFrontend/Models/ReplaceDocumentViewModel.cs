using System.ComponentModel.DataAnnotations;

namespace EMSFrontend.Models
{
    public class ReplaceDocumentViewModel
    {
        [Required]
        public int DocumentId { get; set; }

        [Required(ErrorMessage = "Please select a file")]
        public IFormFile File { get; set; }
    }
}
