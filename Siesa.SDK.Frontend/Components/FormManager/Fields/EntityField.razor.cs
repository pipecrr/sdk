using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
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

        protected override async Task OnInitializedAsync()
        {
            base.OnInitializedAsync();
            string OnReadyStr = (string)FieldOpt.CustomAttributes?.Where(x => x.Key == "sdk-ready").FirstOrDefault().Value;

            if (!string.IsNullOrEmpty(OnReadyStr))
            {                
                var ejec = await Evaluator.EvaluateCode(OnReadyStr, BaseModelObj, OnReadyStr, true, useRoslyn: true); //revisar
                OnReady = (Action<List<dynamic>>)ejec;
            }
        }
    }
}