using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq.Expressions;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Frontend.Components;


namespace Siesa.SDK.Frontend.Components.Visualization.Charts;

public partial class SDKChartSplineSeries<TData, TArgument, TValue> : SDKComponent
{
[Parameter]
public Expression<Func<TData, TArgument>> ArgumentField { get; set; }
[Parameter]
public string Axis { get; set; }
[Parameter]
public bool BreakOnEmptyPoints { get; set; }
[Parameter]
public Color Color { get; set; }
[Parameter]
public ChartDashStyle DashStyle { get; set; }
[Parameter]
public IEnumerable<TData> Data { get; set; }
[Parameter]
public Expression<Func<TData, bool>> Filter { get; set; }
[Parameter]
public ChartContinuousSeriesHoverMode HoverMode { get; set; }
[Parameter]
public string Name { get; set; }
[Parameter]
public string Pane { get; set; }
[Parameter]
public ChartContinuousSeriesSelectionMode SelectionMode { get; set; }
[Parameter]
public ChartSeriesSettings<TData, TValue, TArgument> Settings { get; set; }
[Parameter]
public Func<IEnumerable<TValue>, TValue> SummaryMethod { get; set; }
[Parameter]
public Expression<Func<TData, TValue>> ValueField { get; set; }
[Parameter]
public bool Visible { get; set; }
[Parameter]
public int Width { get; set; }=2;
[Parameter] 
public RenderFragment? ChildContent { get; set; }
}