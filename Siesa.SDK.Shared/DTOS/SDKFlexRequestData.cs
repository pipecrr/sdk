using System.Collections.Generic;

namespace Siesa.SDK.Shared.DTOS
{
    public class SDKFlexRequestData
    {
        public string module_path { get; set; }
        public string selected_class { get; set; }
        public List<SDKFlexFilters> filters { get; set; }
        public List<SDKFlexColumn> columns { get; set; }
        public List<SDKFlexColumn> groups { get; set; }
        public string filter_search { get; set; }
        public int? skip { get; set; }
        public int? take { get; set; }
    }
}