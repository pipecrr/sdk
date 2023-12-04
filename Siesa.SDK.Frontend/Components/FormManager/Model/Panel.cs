using System;
using System.Collections.Generic;
using GrapeCity.ActiveReports.SectionReportModel;
using Siesa.SDK.Frontend.Components.FormManager.Model.Fields;

namespace Siesa.SDK.Frontend.Components.FormManager.Model
{
    public class Panel
    {
        public string Name { get; set; }
        public string ResourceTag { get; set; }
        public string IconClass { get; set; } = "fa-circle-info";

        public bool PreserveDOM { get; set; } = true;
        public string PanelId { get; set; } = Guid.NewGuid().ToString();
        public List<FieldOptions> Fields { get; set; } = new List<FieldOptions>();
        public SubViewdef SubViewdef { get; set; }
        public Dictionary<string, int> ColSize { get; set; } = new Dictionary<string, int>()
        {
            {"MD", 4},
            {"SM", 6},
            {"XS", 12},
        };
        public int RowidGroupDynamicEntity { get; set; }
        /// <summary>
        /// Gets or sets the Hidden property of the panel.
        /// </summary>
        public bool Hidden { get; set; }
        /// <summary>
        /// Gets or sets the list of additional attributes to be included in the panel.
        /// </summary>
        public Dictionary<string, object> CustomAttributes { get; set; } = new Dictionary<string, object>();
    }
}
