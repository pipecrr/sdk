using System.Collections.Generic;

namespace Siesa.SDK.Shared.DTOS
{
    public class SDKFlexField
    {
        public string name { get; set; }
        public string label { get; set; }
        public string type { get; set; }
        public List<SDKFlexFilterOption> filter_options { get; set; }
        public string module_name { get; set; }
        public string class_name { get; set; }
        public List<string> search_fields { get; set; }
        public List<object> results { get; set; }
        public SDKSelectFieldParams selectfield_params { get; set; }
    } 
}