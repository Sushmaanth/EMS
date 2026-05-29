using System;
using System.Collections.Generic;
using System.Text;

namespace Dtos
{
    public class DeleteDocumentResponseDto
    {
        public int DocumentId { get; set; }

        public string DocumentType { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
    }
}
