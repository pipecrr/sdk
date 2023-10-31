using System.Collections.Generic;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Frontend.Components;

namespace Siesa.SDK.Frontend.Components.Visualization.Charts;

public partial class SDKChartTitle: SDKComponent
{
    [Parameter]
    public string CssClass { get; set; }
    [Parameter]
    public HorizontalAlignment HorizontalAlignment { get; set; }
    [Parameter]
    public string Text { get; set; }
    [Parameter]
    public VerticalEdge VerticalAlignment { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

}