using System.Collections.Generic;
using System.Drawing;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;

namespace Siesa.SDK.Frontend.Components.Visualization.Charts;

public partial class SDKChartTooltip : SDKComponent
{
    [Parameter]
    public RenderFragment<ChartTooltipData> ChildContent { get; set; }
    [Parameter]
    public bool Enabled { get; set; }
    [Parameter]
    public RelativePosition Position { get; set; }
}