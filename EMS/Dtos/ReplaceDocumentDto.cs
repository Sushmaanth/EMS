using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dtos
{
    public class ReplaceDocumentDto
    {
        public int DocumentId { get; set; }

        public IFormFile File { get; set; }
    }
}
