using Microsoft.AspNetCore.Components;
using Siesa.SDK.Frontend.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Siesa.SDK.Frontend.Components.Visualization
{
    public partial class SDKFixedButtonManyToMany : ComponentBase
    {
        [Parameter] public string ResourceTag { get; set; }
        [Parameter] public Action OnCustomClick {get; set;}
        [Parameter] public bool ShowButton {get; set;}
        [Inject] public SDKNotificationService NotificationService {get; set;}

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync().ConfigureAwait(true);
        }

        public void Refresh()
        {
            StateHasChanged();
        }

        private void OnClick()
        {
            if(OnCustomClick is null)
            {
                return;
            }
            OnCustomClick();
        }
    }
}