using System;
using System.Collections.Generic;
using System.Text;

namespace Dtos
{
    public class ServiceResponseDto<T>
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public T? Data { get; set; }

        public Dictionary<string, List<string>> Errors { get; set; } = new();
    }
}
