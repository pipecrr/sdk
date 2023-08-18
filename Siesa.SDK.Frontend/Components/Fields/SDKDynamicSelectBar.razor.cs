using Siesa.SDK.Shared.DTOS;
using System;
using System.Linq;
using System.Collections.Generic;
using Siesa.SDK.Frontend.Components.Visualization;
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Frontend.Components;

namespace Siesa.SDK.Frontend.Components.Fields
{
    public partial class SDKDynamicSelectBar : SDKComponent
    {
        [Parameter] public List<DynamicSelectBarDetailDTO> Items { get; set; }
        [Parameter] public Action ValueChanged { get; set; }
        [Parameter] public bool Disabled { get; set; }
        private string _id = $"{Guid.NewGuid()}";

        private string GetActiveCss(DynamicSelectBarDetailDTO Item) => !Item.On ? string.Empty : "rz-state-active";
        private string GetStyle(DynamicSelectBarDetailDTO Item) => $"color: #{Item.IconColor};";

        private void OnChange(DynamicSelectBarDetailDTO Item)
        {
            if (Item.On || Disabled) return;

            Items.First(x => x.On).On = false;

            Item.On = true;

            ValueChanged?.Invoke();

            StateHasChanged();
        }


    }

}