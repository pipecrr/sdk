namespace Siesa.SDK.Shared.DTOS
{
    public class SDKFileUploadDTO{
        public string Url { get; set; }
        public string FileType { get; set; }
        public string FileName { get; set; }
        public byte[]? FileContent { get; set; }
    }
}