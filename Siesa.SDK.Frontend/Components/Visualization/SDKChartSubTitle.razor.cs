using System.Collections.Generic;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Frontend.Components;

namespace Siesa.SDK.Frontend.Components.Visualization;

public partial class SDKChartSubTitle : SDKComponent
{
[Parameter]
public string CssClass { get; set; }

[Parameter]
public string Text { get; set; }
}