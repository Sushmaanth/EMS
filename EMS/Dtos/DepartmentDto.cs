using System.ComponentModel.DataAnnotations;

namespace Dtos
{
    public class DepartmentDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="Department Name is required")]
        [StringLength(200,ErrorMessage ="Department Name cannot exceed more than 200 characters")]
        public string DepartmentName { get; set; }
    }
}