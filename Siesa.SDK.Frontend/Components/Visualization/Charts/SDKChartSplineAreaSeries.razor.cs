using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq.Expressions;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Frontend.Components;


namespace Siesa.SDK.Frontend.Components.Visualization.Charts;
public partial class SDKChartSplineAreaSeries<TData, TArgument, TValue> : SDKComponent
{
    /// <summary>
    /// Gets or sets the data to be displayed in the series.
    /// </summary>
    [Parameter]
    public IEnumerable<TData> Data { get; set; }

    /// <summary>
    /// Gets or sets the expression defining the argument field for the series.
    /// </summary>
    [Parameter]
    public Expression<Func<TData, TArgument>> ArgumentField { get; set; }

    /// <summary>
    /// Gets or sets the settings for the series.
    /// </summary>
    [Parameter]
    public ChartSeriesSettings<TData, TValue, TArgument> Settings { get; set; }

    /// <summary>
    /// Gets or sets the expression defining the value field for the series.
    /// </summary>
    [Parameter]
    public Expression<Func<TData, TValue>> ValueField { get; set; }

    /// <summary>
    /// Gets or sets the axis associated with the series.
    /// </summary>
    [Parameter]
    public string Axis { get; set; }

    /// <summary>
    /// Gets or sets the hover mode for the series.
    /// </summary>
    [Parameter]
    public SDKChartContinuousSeriesHoverMode HoverMode { get; set; }

    /// <summary>
    /// Gets or sets the opacity of the series.
    /// </summary>
    [Parameter]
    public double Opacity { get; set; } = 0.5;

    /// <summary>
    /// Gets or sets the selection mode for the series.
    /// </summary>
    [Parameter]
    public SDKChartContinuousSeriesSelectionMode SelectionMode { get; set; }

    /// <summary>
    /// Gets or sets whether to break the series on empty points.
    /// </summary>
    [Parameter]
    public bool BreakOnEmptyPoints { get; set; }

    /// <summary>
    /// Gets or sets the color of the series.
    /// </summary>
    [Parameter]
    public Color Color { get; set; }

    /// <summary>
    /// Gets or sets the filter expression for the series.
    /// </summary>
    [Parameter]
    public Expression<Func<TData, bool>> Filter { get; set; }

    /// <summary>
    /// Gets or sets the pane associated with the series.
    /// </summary>
    [Parameter]
    public string Pane { get; set; }

    /// <summary>
    /// Gets or sets the name of the series.
    /// </summary>
    [Parameter]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the child content to be rendered within the series.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}