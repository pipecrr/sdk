using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq.Expressions;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Frontend.Components;


namespace Siesa.SDK.Frontend.Components.Visualization.Charts;
public partial class SDKChartFullStackedAreaSeries<TData, TArgument, TValue> : SDKComponent
{
    /// <summary>
    /// Data to be displayed in the series.
    /// </summary>
    [Parameter]
    public IEnumerable<TData> Data { get; set; }

    /// <summary>
    /// Defines the argument field for the series.
    /// </summary>
    [Parameter]
    public Expression<Func<TData, TArgument>> ArgumentField { get; set; }

    /// <summary>
    /// Settings for the series.
    /// </summary>
    [Parameter]
    public ChartSeriesSettings<TData, TValue, TArgument> Settings { get; set; }

    /// <summary>
    /// Defines the value field for the series.
    /// </summary>
    [Parameter]
    public Expression<Func<TData, TValue>> ValueField { get; set; }

    /// <summary>
    /// Associated axis for the series.
    /// </summary>
    [Parameter]
    public string Axis { get; set; }

    /// <summary>
    /// Hover mode for the series.
    /// </summary>
    [Parameter]
    public SDKChartContinuousSeriesHoverMode HoverMode { get; set; }

    /// <summary>
    /// Opacity of the series. Default is 0.5.
    /// </summary>
    [Parameter]
    public double Opacity { get; set; } = 0.5;

    /// <summary>
    /// Selection mode for the series.
    /// </summary>
    [Parameter]
    public SDKChartContinuousSeriesSelectionMode SelectionMode { get; set; }

    /// <summary>
    /// Whether to break on empty points.
    /// </summary>
    [Parameter]
    public bool BreakOnEmptyPoints { get; set; }

    /// <summary>
    /// Color of the series.
    /// </summary>
    [Parameter]
    public Color Color { get; set; }

    /// <summary>
    /// Filtering expression for the series.
    /// </summary>
    [Parameter]
    public Expression<Func<TData, bool>> Filter { get; set; }

    /// <summary>
    /// Associated pane for the series.
    /// </summary>
    [Parameter]
    public string Pane { get; set; }

    /// <summary>
    /// Name of the series.
    /// </summary>
    [Parameter]
    public string Name { get; set; }

    /// <summary>
    /// Content to be rendered within the series.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}