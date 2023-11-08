using System;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using Siesa.SDK.Frontend.Components;

/// <summary>
/// Represents a tree level component for organizing hierarchical data within a tree.
/// </summary>
public partial class SDKTreeLevel : SDKComponent
{
    /// <summary>
    /// Gets or sets the property used for retrieving text content from tree items.
    /// </summary>
    [Parameter]
    public string TextProperty { get; set; }

    /// <summary>
    /// Gets or sets the property used for accessing children of tree items.
    /// </summary>
    [Parameter]
    public string ChildrenProperty { get; set; }

    /// <summary>
    /// Gets or sets a function to determine if a tree item has children. Default is true for all items.
    /// </summary>
    [Parameter]
    public Func<object, bool> HasChildren { get; set; } = value => true;

    /// <summary>
    /// Gets or sets a function to determine if a tree item should be initially expanded. Default is false for all items.
    /// </summary>
    [Parameter]
    public Func<object, bool> Expanded { get; set; } = value => false;

    /// <summary>
    /// Gets or sets a function to determine if a tree item is selected. Default is false for all items.
    /// </summary>
    [Parameter]
    public Func<object, bool> Selected { get; set; } = value => false;

    /// <summary>
    /// Gets or sets a function to retrieve text content from tree items for rendering.
    /// </summary>
    [Parameter]
    public Func<object, string> Text { get; set; }

    /// <summary>
    /// Gets or sets a template for rendering tree items within this level.
    /// </summary>
    [Parameter]
    public RenderFragment<RadzenTreeItem> Template { get; set; }
}
