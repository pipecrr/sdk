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
    /// <summary>
    /// Collection of data for the chart series.
    /// </summary>
    [Parameter]
    public IEnumerable<TData> Data { get; set; }

    /// <summary>
    /// Expression representing the argument field of the chart series.
    /// </summary>
    [Parameter]
    public Expression<Func<TData, TArgument>> ArgumentField { get; set; }

    /// <summary>
    /// Specific settings for the chart series.
    /// </summary>
    [Parameter]
    public ChartSeriesSettings<TData, TValue, TArgument> Settings { get; set; }

    /// <summary>
    /// Expression representing the value field of the chart series.
    /// </summary>
    [Parameter]
    public Expression<Func<TData, TValue>> ValueField { get; set; }

    /// <summary>
    /// Defines the axis associated with the chart series.
    /// </summary>
    [Parameter]
    public string Axis { get; set; }

    /// <summary>
    /// Mouse hover mode for the chart series.
    /// </summary>
    [Parameter]
    public SDKChartContinuousSeriesHoverMode HoverMode { get; set; }

    /// <summary>
    /// Selection mode for the chart series.
    /// </summary>
    [Parameter]
    public SDKChartContinuousSeriesSelectionMode SelectionMode { get; set; }

    /// <summary>
    /// Determines if it should break on empty points.
    /// </summary>
    [Parameter]
    public bool BreakOnEmptyPoints { get; set; }

    /// <summary>
    /// Color of the chart series.
    /// </summary>
    [Parameter]
    public Color Color { get; set; }

    /// <summary>
    /// Expression to filter the data for the chart series.
    /// </summary>
    [Parameter]
    public Expression<Func<TData, bool>> Filter { get; set; }

    /// <summary>
    /// Defines the pane associated with the chart series.
    /// </summary>
    [Parameter]
    public string Pane { get; set; }

    /// <summary>
    /// Name of the chart series.
    /// </summary>
    [Parameter]
    public string Name { get; set; }

    /// <summary>
    /// Additional content that can be embedded within the component.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}
