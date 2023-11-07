using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using Siesa.SDK.Frontend.Components;

namespace Siesa.SDK.Frontend.Components.Visualization;
public partial class SDKDataList : SDKComponent
{
    [Parameter]
    public bool WrapItems { get; set; }
    [Parameter]
    public bool AllowVirtualization { get; set; }
    [Parameter]
    public bool IsLoading { get; set; }
}