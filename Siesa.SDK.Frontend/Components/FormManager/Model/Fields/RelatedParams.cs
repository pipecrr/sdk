using System.Collections.Generic;

namespace Siesa.SDK.Frontend.Components.FormManager.Model.Fields
{
    public class RelatedParams
    {
        public List<string> ExtraFields { get; set; } = new List<string>();
        public int FieldTemplate { get; set; } = 1;
        public string FieldPhoto { get; set; }
        public bool AutoValueInUnique { get; set; }
        public bool ShowExtraFields { get; set; }
    }
}