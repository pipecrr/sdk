using System.Collections.Generic;
using Siesa.Global.Enums;

namespace Siesa.SDK.Shared.DTOS
{
    public class SDKResponseImportDTO
    {
        public enumSDKIntegrationStatus Status { get; set; }
        public string Message { get; set; }
        public dynamic Data { get; set; }
        public string Guid { get; set; }
    }
}