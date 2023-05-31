using System.Collections.Generic;

namespace Siesa.SDK.Shared.DTOS
{
    public class SDKRequestImportDataDTO
    {
        public string BusinessName { get; set; }
        public List<Dictionary<string, object>> Data { get; set; }
        public string UrlResponse { get; set; }
    }
}