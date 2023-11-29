using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using Siesa.SDK.Frontend.Components;

namespace Siesa.SDK.Frontend.Components.Visualization;
/// <summary>
/// Represents a fieldset component, possibly collapsible.
/// </summary>
public partial class SDKFieldset : SDKComponent
{
    /// <summary>
    /// Gets or sets a flag indicating whether collapsing functionality is allowed for the fieldset.
    /// </summary>
    [Parameter]
    public bool AllowCollapse { get; set; }

    /// <summary>
    /// Gets or sets a flag indicating whether the fieldset is initially collapsed.
    /// </summary>
    [Parameter]
    public bool Collapsed { get; set; }

    /// <summary>
    /// Gets or sets the title for expanding the fieldset.
    /// </summary>
    [Parameter]
    public string ExpandTitle { get; set; }

    /// <summary>
    /// Gets or sets the title for collapsing the fieldset.
    /// </summary>
    [Parameter]
    public string CollapseTitle { get; set; }

    /// <summary>
    /// Gets or sets the ARIA label for expanding the fieldset.
    /// </summary>
    [Parameter]
    public string ExpandAriaLabel { get; set; }

    /// <summary>
    /// Gets or sets the ARIA label for collapsing the fieldset.
    /// </summary>
    [Parameter]
    public string CollapseAriaLabel { get; set; }

    /// <summary>
    /// Gets or sets the inline style for the fieldset.
    /// </summary>
    [Parameter]
    public string Style { get; set; }

    /// <summary>
    /// Gets or sets the icon for the fieldset.
    /// </summary>
    [Parameter]
    public string Icon { get; set; }

    /// <summary>
    /// Gets or sets the color of the icon for the fieldset.
    /// </summary>
    [Parameter]
    public string IconColor { get; set; }

    /// <summary>
    /// Gets or sets a template for the header of the fieldset.
    /// </summary>
    [Parameter]
    public RenderFragment HeaderTemplate { get; set; }

    /// <summary>
    /// Gets or sets the child content to be placed within the fieldset.
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Gets or sets a template for the summary of the fieldset.
    /// </summary>
    [Parameter]
    public RenderFragment SummaryTemplate { get; set; } = null;

    /// <summary>
    /// Gets or sets an event callback for the expand action of the fieldset.
    /// </summary>
    [Parameter]
    public EventCallback Expand { get; set; }

    /// <summary>
    /// Gets or sets an event callback for the collapse action of the fieldset.
    /// </summary>
    [Parameter]
    public EventCallback Collapse { get; set; }
}
