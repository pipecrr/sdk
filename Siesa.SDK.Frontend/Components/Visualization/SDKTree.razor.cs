using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace Siesa.SDK.Frontend.Components.Visualization;
public partial class SDKTree : SDKComponent
{
    [Parameter]
    public EventCallback<TreeEventArgs> OnChange { get; set; }
    [Parameter]
    public EventCallback<TreeExpandEventArgs> EventExpand { get; set; }
    [Parameter]
    public EventCallback<TreeEventArgs> EventCollapse { get; set; }
    [Parameter]
    public RenderFragment ChildContent { get; set; }
    [Parameter]
    public bool SingleExpand { get; set; }
    [Parameter]
    public IEnumerable Data { get; set; }
    [Parameter]
    public object Value { get; set; }
    [Parameter]
    public EventCallback<object> ValueChanged { get; set; }
    [Parameter]
    public bool AllowCheckBoxes { get; set; }
    [Parameter]
    public bool AllowCheckChildren { get; set; } = true;
    [Parameter]
    public bool AllowCheckParents { get; set; } = true;
    [Parameter]
    public IEnumerable<object> CheckedValues { get; set; } = Enumerable.Empty<object>();
    [Parameter]
    public EventCallback<IEnumerable<object>> CheckedValuesChanged { get; set; }
    [Parameter]
    public string Style { get; set; }


}