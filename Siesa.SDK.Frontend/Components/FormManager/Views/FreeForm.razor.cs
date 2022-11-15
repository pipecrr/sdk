using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Frontend.Components.FormManager.Model;
using Siesa.SDK.Frontend.Components.FormManager.Model.Fields;
using Siesa.SDK.Frontend.Components.FormManager.ViewModels;

namespace Siesa.SDK.Frontend.Components.FormManager.Views
{
    public partial class FreeForm : FormView
    {
        [Parameter] public string Viewdef { get; set; }
        [Parameter] public string Title { get; set; }
        [Parameter] public bool SetTopBar { get; set; } = true;

        [Parameter] public bool ValidateForm { get; set; } = true;

        [Parameter] public EventCallback OnSubmit { get; set; }

        [Parameter] public EventCallback<FreeForm> OnReady { get; set; }
        
        protected override async Task OnInitializedAsync()
        {
            ViewdefName = Viewdef;
            if(string.IsNullOrEmpty(BusinessName) && BusinessObj != null)
            {
                BusinessName = BusinessObj.GetType().Name;
            }
            await base.OnInitializedAsync();

            if(OnReady.HasDelegate)
            {
                await OnReady.InvokeAsync(this);
            }
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

        public List<FieldOptions> GetFields()
        {
            List<FieldOptions> fields = new List<FieldOptions>();
            
            for (int i = 0; i < Panels.Count; i++)
            {
                for (int j = 0; j < Panels[i].Fields.Count; j++)
                {
                    fields.Add(Panels[i].Fields[j]);
                }
            }

            return fields;
        }

        private async Task HandleFreeFormValidSubmit()
        {
            if(OnSubmit.HasDelegate)
            {
                await OnSubmit.InvokeAsync();
            }
            else
            {
                await base.HandleValidSubmit();
            }
        }
        
    }
}
