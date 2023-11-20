using Microsoft.AspNetCore.Components;
using Siesa.SDK.Frontend.Components.FormManager.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace Siesa.SDK.Frontend.Components.Fields
{
    public partial class SDKSelectBar<ItemType> : SDKComponent
    {

        [Parameter] public IEnumerable<SelectBarItemWrap<ItemType>> Data { get; set; }

        [Parameter] public ItemType Value { get; set; }

        [Parameter] public Action<ItemType> ValueChanged {get; set;}

        [Parameter] public bool Disabled { get; set; }
        [Parameter] public Expression<Func<ItemType>> ValueExpression { get; set; }
        [Parameter] public string FieldName { get; set; }

        [Parameter] public string DenialResourceTag {get; set;}
        [Parameter] public string AffirmationResourceTag {get; set;}

        [Inject] 
        private UtilsManager UtilManager {get; set;}

        private string DenialText {get; set;}
        private string AffirmationText {get; set;}

         protected override string GetAutomationId()
        {
            if(string.IsNullOrEmpty(AutomationId))
            {
                if (!string.IsNullOrEmpty(FieldName))
                {
                        AutomationId = FieldName;
                }
            }
            return base.GetAutomationId();
        }

        private async Task GetResources()
        {
            if(typeof(ItemType) == typeof(bool))
            {   
                DenialResourceTag = string.IsNullOrEmpty(DenialResourceTag) ? "Custom.Selectbar.Boolean.No" : DenialResourceTag;
                AffirmationResourceTag = string.IsNullOrEmpty(AffirmationResourceTag) ? "Custom.Selectbar.Boolean.Yes" : AffirmationResourceTag;

                DenialText = await UtilManager.GetResource(DenialResourceTag);
                AffirmationText = await UtilManager.GetResource(AffirmationResourceTag);
                StateHasChanged();
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await GetResources();
        }

        protected override async Task OnParametersSetAsync()
        {
            base.OnParametersSet();
            await GetResources();
        }

    }
}
