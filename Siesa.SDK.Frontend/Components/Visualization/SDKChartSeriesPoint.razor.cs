using System.Collections.Generic;
using System.Drawing;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;

namespace Siesa.SDK.Frontend.Components.Visualization;

public partial class SDKChartSeriesPoint : SDKComponent
{
    [Parameter]
    public Color Color { get; set; }
    [Parameter]
    public ChartSeriesPointHoverMode HoverMode { get; set; }
    [Parameter]
    public ChartSeriesPointSelectionMode SelectionMode { get; set; }
    [Parameter]
    public int Size { get; set; } = 12;
    [Parameter]
    public ChartPointSymbol Symbol { get; set; }
    [Parameter]
    public bool Visible { get; set; }

}