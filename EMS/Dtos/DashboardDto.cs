using System;
using System.Collections.Generic;
using System.Text;

namespace Dtos
{
    public class DashboardDto
    {
        public string EmployeeName { get; set; }

        public string DepartmentName { get; set; }

        public DateOnly DateOfJoining { get; set; }

        public int TotalMandatoryDocuments { get; set; }

        public int UploadedDocuments { get; set; }

        public int MissingDocuments { get; set; }

        public decimal CompletionPercentage { get; set; }

        public List<DocumentStatusDto> RequiredDocuments { get; set; }
    }
}
