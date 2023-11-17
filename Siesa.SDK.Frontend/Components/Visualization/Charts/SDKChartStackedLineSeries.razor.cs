using System;
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Shared.DTOS;
using System.Threading.Tasks;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Frontend.Services;
using Siesa.SDK.Entities;
using System.Collections.Generic;
using System.Linq;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using DevExpress.Blazor;
using System.Drawing;
using System.Linq.Expressions;

namespace Siesa.SDK.Frontend.Components.Visualization.Charts;

public partial class SDKChartStackedLineSeries<TData, TArgument, TValue> : SDKComponent
{
    [Parameter]
    public IEnumerable<TData> Data { get; set; }
    [Parameter]
    public Expression<Func<TData, TArgument>> ArgumentField { get; set; }
    [Parameter]
    public ChartSeriesSettings<TData, TValue, TArgument> Settings { get; set; }
    [Parameter]
    public Expression<Func<TData, TValue>> ValueField { get; set; }
    [Parameter]
    public string Axis { get; set; }
    [Parameter]
    public ChartContinuousSeriesHoverMode HoverMode { get; set; }
    [Parameter]
    public double Opacity { get; set; } = 0.5;
    [Parameter]
    public ChartContinuousSeriesSelectionMode SelectionMode { get; set; }
    [Parameter]
    public bool BreakOnEmptyPoints { get; set; }
    [Parameter]
    public Color Color { get; set; }
    [Parameter]
    public Expression<Func<TData, bool>> Filter { get; set; }
    [Parameter]
    public string Pane { get; set; }
    [Parameter]
    public int Width { get; set; }= 2;
    [Parameter]
    public string Name { get; set; }
    [Parameter]
    public bool Visible { get; set; }

}