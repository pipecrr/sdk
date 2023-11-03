using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using Siesa.SDK.Frontend.Components;

namespace Siesa.SDK.Frontend.Components.Visualization;

public partial class SDKTextBox : SDKComponent
{
    [Parameter]
    public bool ReadOnly { get; set; }

    [Parameter]
    public bool AutoComplete { get; set; } = true;
    [Parameter]
    public AutoCompleteType AutoCompleteType { get; set; } = AutoCompleteType.On;
    [Parameter]
    public long? MaxLength { get; set; }
    [Parameter]
    public bool Trim { get; set; }
}