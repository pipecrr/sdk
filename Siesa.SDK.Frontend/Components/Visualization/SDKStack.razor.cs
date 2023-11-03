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
    public FlexWrap Wrap { get; set; } = FlexWrap.NoWrap;
    [Parameter]
    public Orientation Orientation { get; set; } = Orientation.Vertical;
    [Parameter]
    public string Gap { get; set; }
    [Parameter]
    public bool Reverse { get; set; }    
}