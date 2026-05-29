

using Dtos.Repository.Abstraction;
using Entities;
using Entities.Data;
using Microsoft.EntityFrameworkCore;

namespace Dtos.Repository.Implementation
{
    public class DocumentRepository:IDocumentRepository
    {
        private readonly AppDbContext _context;

        public DocumentRepository(AppDbContext context)
        {
            _context = context;
        }

        public DocumentType? GetById(int id)
        {
            return _context.DocumentTypes
                   .Include(dt => dt.DocumentCategory)
                   .FirstOrDefault(dt => dt.Id == id);
        }

        public IEnumerable<DocumentType>GetByCategory(int categoryId)
        {
            return _context.DocumentTypes
                .Include(dt => dt.DocumentCategory)
                .Where(dt =>
                    dt.DocumentCategoryId == categoryId)
                .ToList();
        }
    }
}
