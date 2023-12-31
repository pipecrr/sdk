﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Siesa.SDK.Frontend.Components.FormManager.Model.Fields;
using Siesa.SDK.Frontend.Services;

namespace Siesa.SDK.Frontend.Components.FormManager.Model
{
    public class SubViewdef {
        public string Business { get; set; }
        public string ParentField { get; set; }
        public string Viewdef { get; set; }
        public List<Panel> Panels = new();

        [JsonConstructor]
        public SubViewdef(string? Business, string? ParentField, string? Viewdef) { 
            this.Business = Business;
            this.ParentField = ParentField;
            this.Viewdef = Viewdef;

            var metadata = BackendRouterService.Instance.GetViewdef(Business, Viewdef);
            if (metadata != "" && metadata != null)
            {
                //replace all the ocurrences of "BaseObj" with the parent field
                var sub_metadata = metadata.Replace("BaseObj", ParentField);
                
                try
                {
                    var formViewModel = JsonConvert.DeserializeObject<FormViewModel>(sub_metadata);
                    Panels = formViewModel.Panels;
                }
                catch (System.Exception)
                {
                    Panels = JsonConvert.DeserializeObject<List<Panel>>(sub_metadata);
                }
            }
        }
    }
}
