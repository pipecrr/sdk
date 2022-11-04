using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;


namespace Siesa.SDK.Frontend.Components.Fields
{
    public partial class SDKRadioButton<ItemType> : ComponentBase
    {

        [Parameter] public IEnumerable<SelectBarItemWrap<ItemType>> Data { get; set; }

        [Parameter] public ItemType Value { get; set; }

        [Parameter] public Action<ItemType> ValueChanged {get; set;}

        [Parameter] public bool Disabled { get; set; }

    }
}