using System;
using System.Collections.Generic;


namespace Siesa.SDK.Frontend.Components.FormManager.Model.Fields
{
    public class FieldOptions
    {
        public string Name { get; set; }
        public string Placeholder { get; set; } = "";
        public string Label { get; set; }
        public Boolean PureField { get; set; } = false;
        public string Id { get; set; }
        public string Value { get; set; }
        public string Msg { get; set; }

        public bool Disabled { get; set; } = false;
        public string CssClass { get; set; }
        public string FieldType { get; set; }

        public string ViewContext { get; set; }

        public List<string> EscucharA { get; set; }

        public Dictionary<string, object> CustomAtributes { get; set; }
        public int TextFieldCols { get; set; } = 20;
        public int TextFieldRows { get; set; } = 6;

        //Para Listas
        public IEnumerable<ListOption> Options { get; set; }

        public Dictionary<string, int> ColSize { get; set; } = new Dictionary<string, int>()
        {
            {"MD", 4},
            {"SM", 6},
            {"XS", 12},

        };
            
            //"col-md-3 col-sm-6 col-xs-12";
    }

   
}
