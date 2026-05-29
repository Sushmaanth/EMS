

namespace Entities
{
    public class DocumentCategory
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<DocumentType> DocumentTypes { get; set; }
    }
}
