using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using Siesa.SDK.Frontend.Components;

namespace Siesa.SDK.Frontend.Components.Visualization;

/// <summary>
/// Represents a stack component for managing layout properties using flexbox.
/// </summary>
public partial class SDKStack : SDKComponent
{
    /// <summary>
    /// Gets or sets the flex wrap mode for the stack.
    /// </summary>
    [Parameter]
    public SDKFlexWrap SDKWrap { get; set; } = SDKFlexWrap.NoWrap;
    
    /// <summary>
    /// Gets or sets the orientation of the stack.
    /// </summary>
    [Parameter]
    public SDKOrientation SDKOrientation { get; set; } = SDKOrientation.Vertical;

    /// <summary>
    /// Gets or sets the alignment of items within the stack along the cross axis.
    /// </summary>
    [Parameter]
    public SDKAlignItems SDKAlignItems { get; set; } = SDKAlignItems.Stretch;

    /// <summary>
    /// Gets or sets the alignment of items within the stack along the main axis.
    /// </summary>
    [Parameter]
    public SDKJustifyContent SDKJustifyContent { get; set; }

    /// <summary>
    /// Gets or sets the gap between items in the stack.
    /// </summary>
    [Parameter]
    public string Gap { get; set; }

    /// <summary>
    /// Gets or sets whether to reverse the order of items in the stack.
    /// </summary>
    [Parameter]
    public bool Reverse { get; set; }

    /// <summary>
    /// Gets or sets the inline style for the stack.
    /// </summary>
    [Parameter]
    public string Style { get; set; } 

    /// <summary>
    /// Gets or sets the child content to be placed within the stack.
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }   
}