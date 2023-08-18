using Siesa.SDK.Shared.DTOS;
using System;
using System.Linq;
using System.Collections.Generic;
using Siesa.SDK.Frontend.Components.Visualization;
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Frontend.Components;

namespace Siesa.SDK.Frontend.Components.Fields
{
    /// <summary>
    /// Represents a dynamic select bar component that allows choosing from a list of options.
    /// </summary>
    public partial class SDKDynamicSelectBar : SDKComponent
    {
        /// <summary>
        /// Gets or sets the list of items for the dynamic select bar.
        /// </summary>
        [Parameter]
        public List<DynamicSelectBarDetailDTO> Items { get; set; }

        /// <summary>
        /// Gets or sets the action to be invoked when the value changes.
        /// </summary>
        [Parameter]
        public Action ValueChanged { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the dynamic select bar is disabled.
        /// </summary>
        [Parameter]
        public bool Disabled { get; set; }
        private string _id = $"{Guid.NewGuid()}";

        private static string GetActiveCss(DynamicSelectBarDetailDTO Item) => !Item.On ? string.Empty : "rz-state-active";
        private static string GetStyle(DynamicSelectBarDetailDTO Item) => $"color: #{Item.IconColor};";

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