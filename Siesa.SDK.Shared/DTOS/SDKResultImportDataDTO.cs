using System.Collections.Generic;

namespace Siesa.SDK.Shared.DTOS
{
    public class SDKResultImportDataDTO
    {
        public List<dynamic> Success { get; set; }
        public List<dynamic> Errors { get; set; }
    }
}