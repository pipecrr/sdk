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
    [Parameter]
    public bool ShowToolbar { get; set; } = true;
    [Parameter]
    public HtmlEditorMode Mode { get; set; } = HtmlEditorMode.Design;
    [Parameter]
    public RenderFragment ChildContent { get; set; }
    [Parameter]
    public IDictionary<string, string> UploadHeaders { get; set; }
    [Parameter]
    public EventCallback<string> Input { get; set; }
    [Parameter]
    public EventCallback<HtmlEditorPasteEventArgs> Paste { get; set; }
    [Parameter]
    public EventCallback<UploadErrorEventArgs> UploadError { get; set; }
    [Parameter]
    public EventCallback<HtmlEditorExecuteEventArgs> Execute { get; set; }
    [Parameter]
    public string UploadUrl { get; set; }

}