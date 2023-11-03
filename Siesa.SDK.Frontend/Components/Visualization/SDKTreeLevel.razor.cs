using System;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;

namespace Siesa.SDK.Frontend.Components.Visualization;
public partial class SDKTreeLevel : SDKComponent
{
    [Parameter]
    public string TextProperty { get; set; }
    [Parameter]
    public string ChildrenProperty { get; set; }
    [Parameter]
    public Func<object, bool> HasChildren { get; set; } = value => true;
    [Parameter]
    public Func<object, bool> Expanded { get; set; } = value => false;
    [Parameter]
    public Func<object, bool> Selected { get; set; } = value => false;
    [Parameter]
    public Func<object, string> Text { get; set; }
    [Parameter]
    public RenderFragment<RadzenTreeItem> Template { get; set; }
}