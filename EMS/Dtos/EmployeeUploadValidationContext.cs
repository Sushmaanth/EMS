using System;
using System.Collections.Generic;
using System.Text;

namespace Dtos
{
    public class EmployeeUploadValidationContext
    {
        public HashSet<string> ExistingEmployeeCodes { get; set; }

        public HashSet<string> ExistingEmails { get; set; }

        public HashSet<long> ExistingMobiles { get; set; }

        public HashSet<string> UploadedEmployeeCodes { get; set; }

        public HashSet<string> UploadedEmails { get; set; }

        public HashSet<long> UploadedMobiles { get; set; }

        public Dictionary<string, int> Departments { get; set; }
    }
}
