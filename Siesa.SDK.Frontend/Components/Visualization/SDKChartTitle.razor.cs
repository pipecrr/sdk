using System.Collections.Generic;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Frontend.Components;

namespace Siesa.SDK.Frontend.Components.Visualization;

public partial class SDKChartTitle<TData> : SDKComponent
{
    [Parameter]
    public IEnumerable<TData> Data { get; set; }
    [Parameter]
    public string CssClass { get; set; }
    [Parameter]
    public HorizontalAlignment HorizontalAlignment { get; set; }
    [Parameter]
    public string Text { get; set; }
    [Parameter]
    public VerticalEdge VerticalAlignment { get; set; }

}