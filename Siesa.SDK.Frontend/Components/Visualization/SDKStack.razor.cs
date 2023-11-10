using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using Siesa.SDK.Frontend.Components;

namespace Siesa.SDK.Frontend.Components.Visualization;

public partial class SDKStack : SDKComponent
{
    [Parameter]
    public SDKFlexWrap SDKWrap { get; set; } = SDKFlexWrap.NoWrap;
    
    [Parameter]
    public SDKOrientation SDKOrientation { get; set; } = SDKOrientation.Vertical;

    [Parameter]
    public SDKAlignItems SDKAlignItems { get; set; } = SDKAlignItems.Stretch;

    [Parameter]
    public SDKJustifyContent SDKJustifyContent { get; set; }

    [Parameter]
    public string Gap { get; set; }

    [Parameter]
    public bool Reverse { get; set; }

    [Parameter]
    public string Style { get; set; } 

    [Parameter]
    public RenderFragment ChildContent { get; set; }   
}