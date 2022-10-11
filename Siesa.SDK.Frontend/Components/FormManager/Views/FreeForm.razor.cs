using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Frontend.Components.FormManager.Model;
using Siesa.SDK.Frontend.Components.FormManager.ViewModels;

namespace Siesa.SDK.Frontend.Components.FormManager.Views
{
    public partial class FreeForm : FormView
    {
        [Parameter] public string Viewdef { get; set; }
        [Parameter] public string Title { get; set; }
        [Parameter] public bool SetTopBar { get; set; } = true;

        protected override async Task OnInitializedAsync()
        {
            ViewdefName = Viewdef;
            if(string.IsNullOrEmpty(BusinessName) && BusinessObj != null)
            {
                BusinessName = BusinessObj.GetType().Name;
            }
            await base.OnInitializedAsync();
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            try
            {
                
                if (parameters.TryGetValue<string>(nameof(Viewdef), out var value))
                {
                    if (value != null && value != Viewdef)
                    {
                        ViewdefName = value;
                        StateHasChanged();
                    }
                }
            }
            catch (Exception e)
            {

            }

            await base.SetParametersAsync(parameters);
        }
        
    }
}
