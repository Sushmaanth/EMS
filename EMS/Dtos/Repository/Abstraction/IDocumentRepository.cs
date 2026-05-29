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
    }
}
