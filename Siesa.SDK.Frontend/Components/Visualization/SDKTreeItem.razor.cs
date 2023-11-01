using Microsoft.AspNetCore.Components;
using Radzen.Blazor;

namespace Siesa.SDK.Frontend.Components.Visualization;
public partial class SDKTreeItem : SDKComponent
{

    [Parameter]
    public RenderFragment ChildContent { get; set; }
    [Parameter]
    public RenderFragment<RadzenTreeItem> Template { get; set; }
    [Parameter]
    public string Text { get; set; }
    [Parameter]
    public bool Expanded { get; set; }
    [Parameter]
    public object Value { get; set; }


}