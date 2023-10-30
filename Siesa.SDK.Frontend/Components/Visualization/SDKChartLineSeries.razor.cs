using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using System.Drawing;

namespace Siesa.SDK.Frontend.Components.Visualization;

public partial class SDKChartLineSeries<TData, TArgument, TValue> : SDKComponent
{
    [Parameter]
    public IEnumerable<TData> Data { get; set; }
    [Parameter]
    public IEnumerable<TArgument> Argument { get; set; }
    [Parameter]
    public IEnumerable<TValue> Value { get; set; }
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


    // //Parameters DXCharLabelPoint

    // [Parameter]
    // public Color Color { get; set; }
    // [Parameter]
    // public ChartSeriesPointHoverMode HoverModeLabelPoint { get; set; }
    // [Parameter]
    // public ChartSeriesPointSelectionMode SelectionModeLabelPoint { get; set; }
    // [Parameter]
    // public int Size { get; set; } = 12;
    // [Parameter]
    // public ChartPointSymbol Symbol { get; set; }
    // [Parameter]
    // public bool VisibleLabelPoint { get; set; }

    // //DXChartSeriesLabel

    // [Parameter]
    // public ChartElementFormat ArgumentFormat { get; set; }
    // [Parameter]
    // public Color BackgroundColor { get; set; }
    // [Parameter]
    // public ChartElementFormat Format { get; set; }
    // [Parameter]
    // public string FormatPattern { get; set; } = null;
    // [Parameter]
    // public RelativePosition Position { get; set; }
    // [Parameter]
    // public ChartElementFormat ValueFormat { get; set; }
    // [Parameter]
    // public bool VisibleSeriesLabel { get; set; }
}




