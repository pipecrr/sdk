using System.Collections.Generic;

namespace Siesa.SDK.Shared.DTOS
{
    public class SDKUploadChangeEventArgsDTO
    {
        public IEnumerable<SDKFileInfoDTO> Files { get; set; }
    }
}