using System;
using System.Collections.Generic;
using System.Text;

namespace Dtos
{
    public class EmployeeUploadContext
    {
        public int RowNumber { get; set; } = 2;

        public DateOnly Today { get; set; }

        public HashSet<string> UploadedEmployeeCodes { get; set; }
            = new(StringComparer.OrdinalIgnoreCase);

        public HashSet<string> UploadedEmails { get; set; }
            = new(StringComparer.OrdinalIgnoreCase);

        public HashSet<long> UploadedMobiles { get; set; }
            = new();

        public HashSet<string> ExistingEmployeeCodes { get; set; }
            = new(StringComparer.OrdinalIgnoreCase);

        public HashSet<string> ExistingEmails { get; set; }
            = new(StringComparer.OrdinalIgnoreCase);

        public HashSet<long> ExistingMobiles { get; set; }
            = new();

        public Dictionary<string, int> DepartmentDictionary { get; set; }
            = new(StringComparer.OrdinalIgnoreCase);

        public HashSet<string> AllowedGenders { get; set; }
            = new(
                new[] { "Male", "Female", "Other" },
                StringComparer.OrdinalIgnoreCase);
    }
}
