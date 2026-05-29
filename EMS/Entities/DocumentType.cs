

namespace Entities
{
    public class DocumentType
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsMandatory { get; set; }

        public int DocumentCategoryId { get; set; }

        public DocumentCategory DocumentCategory { get; set; }

        public ICollection<EmployeeDocument> EmployeeDocuments { get; set; }
    }
}
