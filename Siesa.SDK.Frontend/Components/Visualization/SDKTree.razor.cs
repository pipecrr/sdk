using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Radzen;
using Siesa.SDK.Frontend.Components;

/// <summary>
/// Represents a tree component for data visualization.
/// </summary>
public partial class SDKTree : SDKComponent
{
    /// <summary>
    /// Gets or sets an event callback for handling tree node selection changes.
    /// </summary>
    [Parameter]
    public EventCallback<TreeEventArgs> OnChange { get; set; }

    /// <summary>
    /// Gets or sets an event callback for handling tree node expansion.
    /// </summary>
    [Parameter]
    public EventCallback<TreeExpandEventArgs> EventExpand { get; set; }

    /// <summary>
    /// Gets or sets an event callback for handling tree node collapse.
    /// </summary>
    [Parameter]
    public EventCallback<TreeEventArgs> EventCollapse { get; set; }

    /// <summary>
    /// Gets or sets the child content of the tree, allowing customization of the tree nodes.
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Gets or sets a flag indicating whether only one tree node can be expanded at a time.
    /// </summary>
    [Parameter]
    public bool SingleExpand { get; set; }

    /// <summary>
    /// Gets or sets the data source for the tree.
    /// </summary>
    [Parameter]
    public IEnumerable Data { get; set; }

    /// <summary>
    /// Gets or sets the selected value in the tree.
    /// </summary>
    [Parameter]
    public object Value { get; set; }

    /// <summary>
    /// Gets or sets an event callback for handling changes in the selected value.
    /// </summary>
    [Parameter]
    public EventCallback<object> ValueChanged { get; set; }

    /// <summary>
    /// Gets or sets a flag indicating whether checkboxes are allowed for tree nodes.
    /// </summary>
    [Parameter]
    public bool AllowCheckBoxes { get; set; }

    /// <summary>
    /// Gets or sets a flag indicating whether child nodes can have checkboxes.
    /// </summary>
    [Parameter]
    public bool AllowCheckChildren { get; set; } = true;

    /// <summary>
    /// Gets or sets a flag indicating whether parent nodes can have checkboxes.
    /// </summary>
    [Parameter]
    public bool AllowCheckParents { get; set; } = true;

    /// <summary>
    /// Gets or sets the checked values for the tree nodes.
    /// </summary>
    [Parameter]
    public IEnumerable<object> CheckedValues { get; set; } = Enumerable.Empty<object>();

    /// <summary>
    /// Gets or sets an event callback for handling changes in the checked values.
    /// </summary>
    [Parameter]
    public EventCallback<IEnumerable<object>> CheckedValuesChanged { get; set; }

    /// <summary>
    /// Gets or sets the custom styling for the tree component.
    /// </summary>
    [Parameter]
    public string Style { get; set; }
}
