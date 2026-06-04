using System;
using System.Collections.Generic;
using System.Text;

namespace Dtos
{
    public class UploadEmployeeExcelErrorDto
    {
        public int RowNumber { get; set; }

        public string ErrorMessage { get; set; }
    }
}
