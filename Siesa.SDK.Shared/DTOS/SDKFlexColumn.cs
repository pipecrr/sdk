using System.Collections.Generic;

namespace Siesa.SDK.Shared.DTOS
{
    public class SDKFlexColumn
    {
        public string name { get; set; }
        public string label { get; set; }
        public string type { get; set; }
        public List<SDKFlexFilterOption> filter_options { get; set; }
        public object module_name { get; set; }
        public object class_name { get; set; }
        public object search_fields { get; set; }
        public string sortType { get; set; }
        public object results { get; set; }
        public string path { get; set; }
        public string key_name { get; set; }
        public string id_enum { get; set; }
        public string formula { get; set; }
        public bool customFn { get; set; }
        public bool is_dynamic_field { get; set; }
        public string aggregate { get; set; }
        public bool hide { get; set; }
        public bool is_sensitive_data { get; set; }
        public SDKSelectFieldParams selectfield_params { get; set; }
    }
}