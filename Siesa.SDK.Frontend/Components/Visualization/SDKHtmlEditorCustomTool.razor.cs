using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using Siesa.SDK.Frontend.Components;

namespace Siesa.SDK.Frontend.Components.Visualization;
public partial class SDKHtmlEditor : SDKComponent
{
    /// <summary>
    /// Determines if the tools is visible.
    /// </summary>
    [Parameter]
    public bool Visible { get; set; } = true;

    /// <summary>
    /// Specifies the icon of the tool. Set to <c>"settings"</c> by default.
    /// </summary>
    [Parameter]
    public string Icon { get; set; } = "settings";

    /// <summary>
    /// Gets or sets the icon color.
    /// </summary>
    /// <value>The icon color.</value>
    [Parameter]
    public string IconColor { get; set; }

    /// <summary>
    /// Specifies the modes that this tool will be enabled in.
    /// </summary>
    [Parameter]
    public HtmlEditorMode EnabledModes { get; set; } = HtmlEditorMode.Design;

    /// <summary>
    /// The template of the tool. Use to render a custom tool.
    /// </summary>
    [Parameter]
    public RenderFragment<RadzenHtmlEditor> Template { get; set; }

    /// <summary>
    /// Specifies whether the tool is selected.
    /// </summary>
    [Parameter]
    public bool Selected { get; set; }

    /// <summary>
    /// Specifies whether the tool is disabled.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// Specifies the name of the command. It is available as <see cref="HtmlEditorExecuteEventArgs.CommandName" /> when
    /// <see cref="RadzenHtmlEditor.Execute" /> is raised.
    [Parameter]
    public string CommandName { get; set; }

    /// <summary>
    /// The RadzenHtmlEditor component which this tool is part of.
    /// </summary>
    [CascadingParameter]
    public RadzenHtmlEditor Editor { get; set; }

    /// <summary>
    /// Specifies the title (tooltip) displayed when the user hovers the tool.
    /// </summary>
    [Parameter]
    public string Title { get; set; }

}