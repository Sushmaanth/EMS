namespace EMSFrontend.Models
{
    public class EmployeeDocumentResponseViewModel
    {
        public int DocumentId { get; set; }
        public string DocumentCategory { get; set; } = string.Empty;

        public string DocumentType { get; set; } = string.Empty;

        public string OriginalFileName { get; set; } = string.Empty;

        public string StoredFileName { get; set; } = string.Empty;

        public string BlobUrl { get; set; } = string.Empty;

        public DateTime UploadedDate { get; set; }
    }
}
