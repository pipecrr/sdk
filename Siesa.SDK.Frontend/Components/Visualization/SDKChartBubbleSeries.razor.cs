using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Frontend.Components;

namespace Siesa.SDK.Frontend.Components.Visualization;

public partial class SDKChartBubbleSeries<TData, TArgument, TValue, TSize> : SDKComponent
{
    [Parameter]
    public IEnumerable<TData> Data { get; set; }
    [Parameter]
    public IEnumerable<TArgument> Argument { get; set; }
    [Parameter]
    public IEnumerable<TValue> Value { get; set; }
    [Parameter]
    public IEnumerable<TSize> Size { get; set; }
    [Parameter]
    public ChartSeriesPointHoverMode HoverMode { get; set; }
    [Parameter]
    public double Opacity { get; set; } = 0.5;
    [Parameter]
    public ChartSeriesPointSelectionMode SelectionMode { get; set; }
    [Parameter]
    public Expression<Func<TData, TSize>> SizeField { get; set; }
}