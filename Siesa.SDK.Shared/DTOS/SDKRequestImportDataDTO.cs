using System.Collections.Generic;

namespace Siesa.SDK.Shared.DTOS
{
    public class SDKRequestImportDataDTO
    {
        public string BusinessName { get; set; }
        public List<Dictionary<string, object>> Data { get; set; }
        public Dictionary<string, string> ForeingsKeysData { get; set; } =  new();
        public string UrlResponse { get; set; }
    }

    public class SDKRequestImportDataDTO2
    {
        public string BusinessName { get; set; }
        public List<Dictionary<string, object>> Data { get; set; }
        public Dictionary<string, string> ForeingsKeysData { get; set; } =  new();
        public string UrlResponse { get; set; }
    }
}