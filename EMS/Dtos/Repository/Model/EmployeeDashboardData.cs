using Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dtos.Repository.Model
{
    public class EmployeeDashboardData
    {
        public Employee Employee { get; set; }

        public List<DocumentType> MandatoryDocumentTypes { get; set; }

        public List<EmployeeDocument> UploadedDocuments { get; set; }
    }
}
