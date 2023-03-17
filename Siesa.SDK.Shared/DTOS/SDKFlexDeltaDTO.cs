using System.Collections.Generic;

namespace Siesa.SDK.Shared.DTOS
{
    public class SDKFlexDeltaDTO
    {
        public List<SDKFlexDeltaColumnDTO> Columns { get; set; }
        public List<SDKFlexFilters> Filters { get; set; }
        public List<SDKFlexColumn> Groups { get; set; }
    }
}