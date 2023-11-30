using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using Siesa.SDK.Frontend.Components;

namespace Siesa.SDK.Frontend.Components.Visualization;
/// <summary>
/// Represents a tree item component for data visualization within a tree.
/// </summary>
public partial class SDKTreeItem : SDKComponent
{
    /// <summary>
    /// Gets or sets the child content of the tree item, allowing customization of its appearance.
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Gets or sets a template for rendering the tree item.
    /// </summary>
    [Parameter]
    public RenderFragment<RadzenTreeItem> Template { get; set; }

    /// <summary>
    /// Gets or sets the text content of the tree item.
    /// </summary>
    [Parameter]
    public string TextItem { get; set; }

    /// <summary>
    /// Gets or sets a flag indicating whether the tree item is initially expanded.
    /// </summary>
    [Parameter]
    public bool Expanded { get; set; }

    /// <summary>
    /// Gets or sets the value associated with the tree item.
    /// </summary>
    [Parameter]
    public object Value { get; set; }
}
