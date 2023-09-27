namespace Siesa.SDK.Shared.DTOS
{
    public class SDKFileFieldDTO{
        public string Url { get; set; }
        public string FileType { get; set; }
        public string FileName { get; set; }
        public byte[] FileByte { get; set; }
        public string FileBase64 { get; set; }
        public int Rowid { get; set; }
    }
}