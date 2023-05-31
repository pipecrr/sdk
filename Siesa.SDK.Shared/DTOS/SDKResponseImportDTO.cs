using System.Collections.Generic;

namespace Siesa.SDK.Shared.DTOS
{
    public class SDKResponseImportDTO
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }
    }
}