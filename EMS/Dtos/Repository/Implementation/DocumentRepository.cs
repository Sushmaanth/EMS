

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

        public IEnumerable<DocumentCategory> GetAll()
        {
            return _context.DocumentCategories.ToList();
        }

        public EmployeeDocument? GetEmployeeDocument(int employeeId, int documentTypeId)
        {
            return _context.EmployeeDocuments
               .FirstOrDefault(ed =>
                   ed.EmployeeId == employeeId &&
                   ed.DocumentTypeId == documentTypeId);
        }

        public EmployeeDocument? GetDocumentById(int id)
        {
            return _context.EmployeeDocuments
                .Include(d => d.DocumentType)
                .ThenInclude(dt => dt.DocumentCategory)
                .Include(d => d.Employee)
                .FirstOrDefault(x => x.Id == id);
        }

        public void DeleteDocument(EmployeeDocument document)
        {
            _context.EmployeeDocuments.Remove(document);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<ICollection<DocumentType>> GetDocumentTypesByCategoryAsync(int categoryId, int employeeId)
        {
            return await _context.DocumentTypes.Include(
                dt => dt.EmployeeDocuments.Where(
                    ed => ed.EmployeeId == employeeId))
                .Where(dt => dt.DocumentCategoryId == categoryId)
                .ToListAsync();
        }
    }
}
