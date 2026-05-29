using Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dtos.Repository.Abstraction
{
    public interface IDocumentRepository
    {
        DocumentType? GetById(int id);

        IEnumerable<DocumentType>GetByCategory(int categoryId);

        IEnumerable<DocumentCategory> GetAll();

        EmployeeDocument? GetEmployeeDocument(int employeeId, int documentTypeId);

        public EmployeeDocument? GetDocumentById(int id);

        void DeleteDocument(EmployeeDocument document);

        Task SaveChangesAsync();
    }
}
