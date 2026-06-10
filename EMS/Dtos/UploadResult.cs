using Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dtos
{
    public class UploadResult
    {
        public List<Employee> ValidEmployees { get; set; } = new();

        public List<UploadEmployeeExcelErrorDto> Errors { get; set; } = new();
    }
}
