using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using Siesa.SDK.Frontend.Components;

namespace Siesa.SDK.Frontend.Components.Visualization;
public partial class SDKFieldset : SDKComponent
{
    [Parameter]
    public bool AllowCollapse { get; set; }
    [Parameter]
    public bool Collapsed { get; set; }
    [Parameter]
    public string ExpandTitle { get; set; }
    [Parameter]
    public string CollapseTitle { get; set; }
    [Parameter]
    public string ExpandAriaLabel { get; set; }
    [Parameter]
    public string CollapseAriaLabel { get; set; }
     [Parameter]
     public string Style { get; set; }
    [Parameter]
    public string Icon { get; set; }
    [Parameter]
    public string IconColor { get; set; }
    [Parameter]
    public string Text { get; set; } = "";
    [Parameter]
    public RenderFragment HeaderTemplate { get; set; }
    [Parameter]
    public RenderFragment ChildContent { get; set; }
    [Parameter]
    public RenderFragment SummaryTemplate { get; set; } = null;
    [Parameter]
    public EventCallback Expand { get; set; }
    [Parameter]
    public EventCallback Collapse { get; set; }

}