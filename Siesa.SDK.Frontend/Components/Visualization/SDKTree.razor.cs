using Microsoft.AspNetCore.Components;
using Radzen;

namespace Siesa.SDK.Frontend.Components.Visualization;
public partial class SDKTree : SDKComponent
{
    [Parameter]
    public EventCallback<TreeEventArgs> Change { get; set; }
    [Parameter]
    public EventCallback<TreeExpandEventArgs> Expand { get; set; }
    [Parameter]
    public EventCallback<TreeEventArgs> Collapse { get; set; }
    [Parameter]
    public RenderFragment ChildContent { get; set; }
    [Parameter]
    public bool SingleExpand { get; set; }

    [Parameter]
    public string Style {get; set;}

    // [Parameter]
    // public object Value { get; set; }
    // [Parameter]
    // public EventCallback<object> ValueChanged { get; set; }
    // [Parameter] 
    // public string Style { get; set; }

}