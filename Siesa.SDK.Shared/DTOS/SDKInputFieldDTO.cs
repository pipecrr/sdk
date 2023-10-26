using Microsoft.AspNetCore.Components.Forms;

namespace Siesa.SDK.Shared.DTOS
{
    public class SDKInputFieldDTO{
        public string Url { get; set; }
        public IBrowserFile File { get; set; }
        public int RowidAttachmentDetail { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        

    }
}