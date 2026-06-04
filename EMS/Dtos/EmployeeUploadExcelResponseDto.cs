using System;
using System.Collections.Generic;
using System.Text;

namespace Dtos
{
    public class EmployeeUploadExcelResponseDto
    {
        public int TotalRecords { get; set; }

        public int SuccessRecords { get; set; }

        public int FailedRecords { get; set; }

        public List<UploadEmployeeExcelErrorDto> Errors { get; set; }= new();
    }
}
