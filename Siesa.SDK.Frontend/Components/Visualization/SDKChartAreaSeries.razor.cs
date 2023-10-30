using System.Collections.Generic;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Frontend.Components;

namespace Siesa.SDK.Frontend.Components.Visualization;

public partial class SDKChartAreaSeries<TData, TArgument, TValue> : SDKComponent
{
    [Parameter]
    public IEnumerable<TData> Data { get; set; }
    [Parameter]
    public IEnumerable<TArgument> Argument { get; set; }
    [Parameter]
    public IEnumerable<TValue> Value { get; set; }
    [Parameter]
    public ChartContinuousSeriesHoverMode HoverMode { get; set; }
    [Parameter]
    public double Opacity { get; set; } = 0.5;
    [Parameter]
    public ChartContinuousSeriesSelectionMode SelectionMode { get; set; }
}