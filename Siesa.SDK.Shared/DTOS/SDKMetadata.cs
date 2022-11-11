using System.Collections.Generic;

namespace Siesa.SDK.Shared.DTOS
{
    public class SDKMetadata{
        public List<SDKTreeDatum> treeData { get; set; }
        public SDKCheckedKeys checkedKeys { get; set; }
        public List<string> expandedKeys { get; set; }
        public List<SDKMetaFieldList> field_list { get; set; }
    }

    public class SDKTreeDatum
    {
        public string module { get; set; }
        public string class_path { get; set; }
        public string value { get; set; }
        public string label { get; set; }
        public string class_name { get; set; }
        public string target_mod { get; set; }
        public string path { get; set; }
        public string breadcrumbs_label { get; set; }
        public List<SDKMetadataChild> children { get; set; }
        public List<SDKMetaFieldList> field_list { get; set; }
    }

    public class SDKMetadataChild
    {
        public string class_name { get; set; }
        public string class_path { get; set; }
        public string fk_name { get; set; }
        public string label { get; set; }
        public string path { get; set; }
    }

    public class SDKCheckedKeys
    {
        public List<string> check { get; set; }
        public List<object> halfChecked { get; set; }
    }

    public class SDKMetaFieldList
    {
        public string name { get; set; }
        public string label { get; set; }
        public string type { get; set; }
        public List<SDKFlexFilterOption> filter_options { get; set; }
        public string module_name { get; set; }
        public object class_name { get; set; }
        public List<string> search_fields { get; set; }
        public object results { get; set; }
        public SDKSelectfieldParams selectfield_params { get; set; }
        public string path { get; set; }
        public string key_name { get; set; }
        public List<SDKMetaOption> options { get; set; }
    }

    public class SDKSelectfieldParams
    {
        public string list_name { get; set; }
    }

    public class SDKMetaOption
    {
        public string name { get; set; }
        public string label { get; set; }
        public string type { get; set; }
        public List<SDKFlexFilterOption> filter_options { get; set; }
        public string module_name { get; set; }
        public object class_name { get; set; }
        public List<string> search_fields { get; set; }
        public object results { get; set; }
        public SDKSelectfieldParams selectfield_params { get; set; }
        public string path { get; set; }
        public string key_name { get; set; }
    }
}