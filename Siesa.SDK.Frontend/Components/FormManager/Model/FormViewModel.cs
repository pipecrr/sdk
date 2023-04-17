using System;
using System.Collections.Generic;
using Siesa.SDK.Frontend.Components.FormManager.Model.Fields;

namespace Siesa.SDK.Frontend.Components.FormManager.Model
{
    public class FormViewModel
    {
        public List<Button> Buttons { get; set; } = new List<Button>();
        public List<Panel> Panels { get; set; } = new List<Panel>();

        public List<Relationship> Relationships { get; set; } = new List<Relationship>(); //Used for relationships in detailview

        public List<string> ExtraFields { get; set; } = new List<string>(); 
    }
}
