using System;
using System.Collections.Generic;
using Siesa.SDK.Frontend.Components.FormManager.Model.Fields;

namespace Siesa.SDK.Frontend.Components.FormManager.Model
{
    public class Panel
    {
        public string Name { get; set; }
        public string ResourceTag { get; set; }

        public bool PreserveDOM { get; set; } = true;
        public string PanelId { get; set; } = Guid.NewGuid().ToString();
        public List<FieldOptions> Fields { get; set; } = new List<FieldOptions>();
        public SubViewdef SubViewdef { get; set; }

        public Dictionary<string, int> ColSize { get; set; } = new Dictionary<string, int>()
        {
            {"XS", 12},
            {"SM", 6},
            {"MD", 6},
            {"LG", 4},
            {"XL", 3}
        };
    }
}
