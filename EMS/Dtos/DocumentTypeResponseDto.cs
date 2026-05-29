using System;
using System.Collections.Generic;
using System.Text;

namespace Dtos
{
    public class DocumentTypeResponseDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public bool IsMandatory { get; set; }

        public int DocumentCategoryId { get; set; }

        public string DocumentCategoryName { get; set; } = string.Empty;
    }
}
