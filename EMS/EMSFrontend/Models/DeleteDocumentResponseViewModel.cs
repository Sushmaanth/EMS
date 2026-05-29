namespace EMSFrontend.Models
{
    public class DeleteDocumentResponseViewModel
    {
        public int DocumentId { get; set; }

        public string DocumentType { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
    }
}
