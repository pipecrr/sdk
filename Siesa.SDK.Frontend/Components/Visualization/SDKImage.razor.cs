using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using Siesa.SDK.Frontend.Components;

namespace Siesa.SDK.Frontend.Components.Visualization;
public partial class SDKImage : SDKComponent
{

    [Parameter]
    public string Path { get; set; }
    public string Style {get ; set;}

}