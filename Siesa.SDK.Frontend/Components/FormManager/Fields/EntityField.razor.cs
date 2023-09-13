using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Siesa.SDK.Frontend.Components.FormManager.Model.Fields;
using Siesa.SDK.Frontend.Components.FormManager.ViewModels;
using Siesa.SDK.Frontend.Utils;

namespace Siesa.SDK.Frontend.Components.FormManager.Fields
{
    public partial class EntityField : FieldClass<dynamic>
    {
        [Parameter]
        public dynamic BaseModelObj { get; set; }
        public string RelatedBusiness { get; set; }
        public Action<List<dynamic>> OnReady { get; set; }
        private Dictionary<string, string> _relatedFilters = new Dictionary<string, string>();
        private List<List<object>> _filters = new List<List<object>>(); 

        protected override async Task OnInitializedAsync()
        {
            base.OnInitializedAsync();
            string OnReadyStr = (string)FieldOpt.CustomAttributes?.Where(x => x.Key == "sdk-ready").FirstOrDefault().Value;

            if (!string.IsNullOrEmpty(OnReadyStr))
            {
                var ejec = await Evaluator.EvaluateCode(OnReadyStr, BaseModelObj);
                if(ejec != null){
                    OnReady = (Action<List<dynamic>>)ejec;
                }
            }
            if(FieldOpt.RelatedFilters != null){
                foreach(var filter in FieldOpt.RelatedFilters){
                    var dynamicValue = await Evaluator.EvaluateCode(filter.Value, BaseModelObj);
                    _relatedFilters.Add(filter.Key, dynamicValue.ToString());
                }
            }
            
            if(FieldOpt.Filters != null){
                EvaluateFilters();
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            if(FieldOpt.RelatedFilters != null)
            {
                _relatedFilters.Clear();
                foreach(var filter in FieldOpt.RelatedFilters){
                    var dynamicValue = await Evaluator.EvaluateCode(filter.Value, BaseModelObj);
                    _relatedFilters.Add(filter.Key, dynamicValue.ToString());
                }
            }

            if(FieldOpt.Filters != null){
                EvaluateFilters();
            }   
        }

        private void EvaluateFilters()
        {
            foreach(var filter in FieldOpt.Filters){
                List<object> filtersInside = new List<object>();
                foreach(var item in filter)
                {
                    var properties = JsonConvert.DeserializeObject<dynamic>(item.ToString()).Properties();
                    bool nullvalue = true;
                    foreach (var property in properties)
                    {
                        var name = property.Name;
                        var codeValue = property.Value.ToString();
                        var dynamicValue = Evaluator.EvaluateCode(codeValue, BaseModelObj);
                        try
                        {
                            item.GetType().GetProperty(name)?.SetValue(item, dynamicValue);
                            nullvalue = false;
                        }catch(NullReferenceException ex){
                            Console.WriteLine(ex.Message);
                            nullvalue = true;
                        }
                    }
                    if(!nullvalue)
                        filtersInside.Add(item);
                }
                if(filtersInside.Count > 0)
                    _filters.Add(filtersInside);
            }
        }

        public void OnChange(){
            try{
                if(BindValue == null){
                    this.SetValue(FieldOpt.EntityRowidField, null);
                }
            }catch(Exception ex){
                Console.WriteLine(ex.Message);
            }
        }
    }
}