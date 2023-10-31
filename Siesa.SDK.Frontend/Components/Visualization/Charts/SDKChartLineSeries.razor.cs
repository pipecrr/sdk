using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using System.Drawing;

namespace Siesa.SDK.Frontend.Components.Visualization.Charts;


public partial class SDKChartLineSeries<TData, TArgument, TValue> : SDKComponent
{
    [Parameter]
    public IEnumerable<TData> Data { get; set; }
    [Parameter]
    public string Name { get; set; }
    [Parameter]
    public Expression<Func<TData, bool>> Filter { get; set; }
    [Parameter]
    public Expression<Func<TData, TArgument>> ArgumentField { get; set; }
    [Parameter]
    public Expression<Func<TData, TValue>> ValueField { get; set; }

    [Parameter]
    public ChartDashStyle DashStyle { get; set; }
    [Parameter]
    public ChartContinuousSeriesHoverMode HoverMode { get; set; }
    [Parameter]
    public ChartContinuousSeriesSelectionMode SelectionMode { get; set; }
    [Parameter]
    public int Width { get; set; } = 2;

    [Parameter] public RenderFragment? ChildContent { get; set; }
}




