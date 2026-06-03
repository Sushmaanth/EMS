namespace EMSFrontend.Models
{
    public class DashboardViewModel
    {
        public string EmployeeName { get; set; }

        public string? DepartmentName { get; set; }

        public DateOnly DateOfJoining { get; set; }

        public int TotalMandatoryDocuments { get; set; }

        public int UploadedDocuments { get; set; }

        public int MissingDocuments { get; set; }

        public decimal CompletionPercentage { get; set; }

        public List<DocumentStatusViewModel> RequiredDocuments { get; set; }
    }
}
