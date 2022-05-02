using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Frontend.Components.FormManager.ViewModels;

namespace Siesa.SDK.Frontend.Components.FormManager.Views
{
    public partial class ExplorerView : FormView
    {
        [Parameter] public string Viewdef { get; set; }
        [Parameter] public string Title { get; set; }

        protected override async Task OnInitializedAsync()
        {
            ViewdefName = Viewdef;
            await base.OnInitializedAsync();
        }

        protected override void InitView(string bName = null)
        {
            base.InitView(bName);
            if(Panels != null && Panels.Count > 0){
                for (int j = 0; j < Panels[0].Fields.Count; j++)
                {
                    var currentField = Panels[0].Fields[j];

                    if(currentField.CustomAttributes  == null){
                        currentField.CustomAttributes = new();
                    }
                    currentField.CustomAttributes.Add("sdk-change", "Refresh()");

                    Panels[0].Fields[j] = currentField;
                    
                }
            }

        }
        
    }
}
