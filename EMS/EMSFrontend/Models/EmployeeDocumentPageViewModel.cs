namespace EMSFrontend.Models
{
    public class EmployeeDocumentPageViewModel
    {
        public List<DocumentCategoryViewModel> Categories { get; set; }= new();

        public List<DocumentTypeViewModel> DocumentTypes { get; set; }= new();
    }
}
