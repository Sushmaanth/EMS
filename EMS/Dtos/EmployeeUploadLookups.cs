using System;
using System.Collections.Generic;
using System.Text;

namespace Dtos
{
    public class EmployeeUploadLookups
    {
        public Dictionary<string, int> DepartmentDictionary { get; set; }

        public HashSet<string> ExistingEmployeeCodes { get; set; }

        public HashSet<string> ExistingEmails { get; set; }

        public HashSet<long> ExistingMobiles { get; set; }
    }
}
