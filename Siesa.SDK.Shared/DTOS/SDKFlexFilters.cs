using System.Collections.Generic;

namespace Siesa.SDK.Shared.DTOS
{
    public class SDKFlexFilters
    {
        public string name { get; set; }
        public string label { get; set; }
        public string type { get; set; }
        public List<SDKFlexFilterOption> filter_options { get; set; }
        public object module_name { get; set; }
        public object class_name { get; set; }
        public object search_fields { get; set; }
        public object results { get; set; }
        public string path { get; set; }
        public string key_name { get; set; }
        public string selected_operator { get; set; }
        public string equal_from { get; set; }
        public string equal_to { get; set; }
    }
}