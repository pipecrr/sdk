using System.Collections.Generic;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Frontend.Components;

namespace Siesa.SDK.Frontend.Components.Visualization;

public partial class SDKChart<TData> : SDKComponent
{

    [Parameter]
    public IEnumerable<TData> Data { get; set; }
    [Parameter]
    public bool AutoHidePointMarkers { get; set; } = true;

    [Parameter]
    public double BarGroupPadding { get; set; } = 0.3;

    [Parameter]
    public int? BarGroupWidth { get; set; } = null;

    [Parameter]
    public ChartLabelOverlap LabelOverlap { get; set; }

    [Parameter]
    public ChartSelectionMode PointSelectionMode { get; set; }

    [Parameter]
    public bool Rotated { get; set; }

    [Parameter]
    public ChartSelectionMode SeriesSelectionMode { get; set; }

    [Parameter]
    public bool SynchronizeAxes { get; set; }
}