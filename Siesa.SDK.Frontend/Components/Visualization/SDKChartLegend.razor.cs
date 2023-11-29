using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Frontend.Services;
using System.Linq.Expressions;
using DevExpress.Blazor;
using Siesa.SDK.Frontend.Components;

namespace Siesa.SDK.Frontend.Components.Visualization;

public partial class SDKChartLegend 
{
    /// <summary>
    /// Specifies whether the legend is located outside or inside the chart’s plot.
    /// </summary>
    [Parameter] 
    public SDKChartRelativePosition RelativePositionLegend { get; set; } = SDKChartRelativePosition.Outside;

    /// <summary>
    /// Along with the VerticalAlignment property, specifies the legend’s position.
    /// </summary>
    [Parameter] 
    public SDKChartHorizontalAlignment HorizontalAlignmentLegend { get; set; } = SDKChartHorizontalAlignment.Center;
    /// <summary>
    /// Along with the HorizontalAlignment property, specifies the legend’s position.
    /// </summary>
    [Parameter] 
    public SDKChartVerticalEdge VerticalAlignmentLegend { get; set; } = SDKChartVerticalEdge.Bottom;
    /// <summary>
    /// Specifies how legend items are arranged, vertically (in a column) or horizontally (in a row).
    /// </summary>
    [Parameter] 
    public SDKChartOrientation OrientationLegend { get; set; } = SDKChartOrientation.Vertical;

    /// <summary>
    /// Specifies what series elements to highlight when a corresponding item in the legend is hovered over.
    /// </summary>

    [Parameter]
    public SDKChartLegendHoverMode HoverModeLegend { get; set; } = SDKChartLegendHoverMode.LegendMarkerAndSeriesWithPoints;

    /// <summary>
    /// Specifies the name of a CSS class applied to a chart legend.
    /// </summary>

    [Parameter] 
    public string CssClass { get; set; }  
    
    /// <summary>
    /// Specifies whether users can toggle series visibility
    /// </summary>
    [Parameter]
    public bool AllowToggleSeries { get; set; }
}
    
