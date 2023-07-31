using System;
using System.Collections.Generic;

namespace Siesa.SDK.Shared.DTOS 
{
    public class PortalDashboardRow
    {
        public string Height { get; set; }
        public List<PortalDashlet> Dashlets { get; set; }
    }

    public class PortalDashlet
    {
        public string ResourceTag { get; set; }
        public Type ComponentType { get; set; }
        public bool ShowTitle { get; set; } = true;
        public string CssClass { get; set; }
        public int Columns { get; set; } = 1;
        public int Rows { get; set; } = 1;
    }

    public class SDKPortalConfiguration
    {
        public List<PortalDashboardRow> DashboardRows { get; set; }
        public PortalDashlet MainDashlet { get; set; }
    }
}