using System.Collections.Generic;

namespace Siesa.SDK.Shared.DTOS
{
    public class SDKFlexRequestSaveData
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string selected_class { get; set; }
        public List<object> visible_users { get; set; }
        public List<object> visible_roles { get; set; }
        public string module_path { get; set; }
        public List<SDKFlexColumn> select_list { get; set; }
        public List<SDKFlexFilters> filter_list { get; set; }
        public List<object> chart_list { get; set; }
        public Owner owner { get; set; }
        public OwnerTeam owner_team { get; set; }
        public List<SDKFlexColumn> group_list { get; set; }
        public int version { get; set; }
        public string metadata { get; set; }
        public string module { get; set; }
    }

    public class Owner
    {
        public string id { get; set; }
    }

    public class OwnerTeam
    {
    }
}