using System.Collections.Generic;

namespace Siesa.SDK.Shared.DTOS
{
    public class SDKUploadProgressArgsDTO
    {
        public long Loaded { get; set; }
        public long Total { get; set; }
        public int Progress { get; set; }
        public IEnumerable<SDKFileInfoDTO> Files { get; set; }
        public bool Cancel { get; set; }
    }
}