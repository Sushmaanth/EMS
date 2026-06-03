namespace Dtos
{
    public class DocumentTypeDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public bool IsMandatory { get; set; }

        public int? DocumentId { get; set; }

        public string? FileName { get; set; }

        public string? BlobUrl { get; set; }

        public bool IsUploaded { get; set; }
    }
}