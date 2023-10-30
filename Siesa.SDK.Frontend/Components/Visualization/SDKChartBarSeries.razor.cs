using System.Collections.Generic;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Frontend.Components;

namespace Siesa.SDK.Frontend.Components.Visualization;

public partial class SDKChartBarSeries<TData, TArgument, TValue> : SDKComponent
{
    [Parameter]
    public IEnumerable<TData> Data { get; set; }
    [Parameter]
    public IEnumerable<TArgument> Argument { get; set; }
    [Parameter]
    public IEnumerable<TValue> Value { get; set; }
    [Parameter]
    public double? BarPadding { get; set; } = null;
    [Parameter]
    public int? BarWidth { get; set; } = null;
    [Parameter]
    public ChartSeriesPointHoverMode HoverMode { get; set; }
    [Parameter]
    public int? MinBarHeight { get; set; } = null;
    [Parameter]
    public ChartSeriesPointSelectionMode SelectionMode { get; set; }
}