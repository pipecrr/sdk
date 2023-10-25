using System.Drawing;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;

namespace Siesa.SDK.Frontend.Components.Visualization;

public partial class SDKChartSeriesLabel : SDKComponent
{
    
[Parameter]
public ChartElementFormat ArgumentFormat { get; set; }
[Parameter]
public Color BackgroundColor { get; set; }
[Parameter]
public ChartElementFormat Format { get; set; }
[Parameter]
public string FormatPattern { get; set; }  = null;
[Parameter]
public RelativePosition Position { get; set; }
[Parameter]
public ChartElementFormat ValueFormat { get; set; }
[Parameter]
public bool Visible { get; set; }
}