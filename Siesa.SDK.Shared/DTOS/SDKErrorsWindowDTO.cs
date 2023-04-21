using System.Collections.Generic;

namespace Siesa.SDK.Shared.DTOS
{
    public class SDKErrorsWindowDTO
    {
        public string Field { get; set; }
        public List<string> Errors { get; set; }
    }
}