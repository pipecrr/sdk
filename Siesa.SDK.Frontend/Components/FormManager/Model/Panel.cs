using System;
using System.Collections.Generic;
using Siesa.SDK.Frontend.Components.FormManager.Model.Fields;

namespace Siesa.SDK.Frontend.Components.FormManager.Model
{
    public class Panel
    {
        public string Label { get; set; }

        public bool PreserveDOM { get; set; } = true;
        public string PanelId { get; set; } = Guid.NewGuid().ToString();
        public List<FieldOptions> Fields { get; set; }
    }
}
