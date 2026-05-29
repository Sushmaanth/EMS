

namespace Entities
{
    public class EmployeeDocument
    {
        public int Id { get; set; }

        public string OriginalFileName { get; set; }

        public string StoredFileName { get; set; }

        public string BlobUrl { get; set; }

        public DateTime UploadedDate { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public int DocumentTypeId { get; set; }

        public DocumentType DocumentType { get; set; }
    }
}
