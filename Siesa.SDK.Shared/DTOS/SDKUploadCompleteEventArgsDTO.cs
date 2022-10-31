using System.Text.Json;

namespace Siesa.SDK.Shared.DTOS
{
    public class SDKUploadCompleteEventArgsDTO
    {
        public JsonDocument JsonResponse { get; set; }
        public string RawResponse { get; set; }
        public bool Cancelled { get; set; }
    }
}