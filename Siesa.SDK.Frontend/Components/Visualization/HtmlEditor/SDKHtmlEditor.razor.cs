using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Radzen;
using Siesa.SDK.Shared.DTOS;
using Siesa.SDK.Frontend.Components;


namespace Siesa.SDK.Frontend.Components.Visualization.HtmlEditor;

public partial class SDKHtmlEditor : SDKComponent
{
    /// <summary>
    /// Gets or sets the current value of the editor.
    /// </summary>
    [Parameter]
    public string Value { get; set; }
    /// <summary>
    /// Specifies whether to show the toolbar. Set it to false to hide the toolbar. Default value is true.
    /// </summary>
    [Parameter]
    public bool ShowToolbar { get; set; } = true;

    /// <summary>
    /// Gets or sets the mode of the editor.
    /// </summary>
    [Parameter]
    public SDKHtmlEditorMode Mode { get; set; } = SDKHtmlEditorMode.Design;

    [Parameter]
    public bool Disabled {get; set;}

    /// <summary>
    /// Gets or sets the child content.
    /// </summary>
    /// <value>The child content.</value>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }


    /// <summary>
    /// Specifies custom headers that will be submit during uploads.
    /// </summary>
    [Parameter]
    public IDictionary<string, string> UploadHeaders { get; set; }

    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    /// <value>The input.</value>
    [Parameter]
    public EventCallback<string> Input { get; set; }

    /// <summary>
    /// A callback that will be invoked when the user pastes content in the editor. Commonly used to filter unwanted HTML.
    /// </summary>

    [Parameter]
    public EventCallback<SDKHtmlEditorPasteEventArgs> Paste { get; set; }

    /// <summary>
    /// A callback that will be invoked when there is an error during upload.
    ///</summary>
    [Parameter]
    public EventCallback<SDKUploadErrorEventArgs> UploadError { get; set; }


    [Parameter]
    public EventCallback<SDKHtmlEditorExecuteEventArgs> Execute { get; set; }

    /// <summary>
    /// Specifies the URL to which RadzenHtmlEditor will submit files.
    /// </summary>
    [Parameter]
    public string UploadUrl { get; set; }

    [Parameter]
    public EventCallback<string> ValueChanged { get; set; }

    private async Task _onUploadError(UploadErrorEventArgs args)
    {
        if (UploadError.HasDelegate)
        {
            SDKUploadErrorEventArgs sDKUploadErrorEventArgs = new();
            sDKUploadErrorEventArgs.Message = args.Message;

            await UploadError.InvokeAsync(sDKUploadErrorEventArgs).ConfigureAwait(true);
        }
    }
    private async Task _onPaste(HtmlEditorPasteEventArgs args)
    {
        if (Paste.HasDelegate)
        {
            SDKHtmlEditorPasteEventArgs sDKHtmlEditorPasteEventArgs = new();
            sDKHtmlEditorPasteEventArgs.Html = args.Html;

            await Paste.InvokeAsync(sDKHtmlEditorPasteEventArgs).ConfigureAwait(true);
        }
    }
    private async Task _onExecute(HtmlEditorExecuteEventArgs args)
    {

        if (Execute.HasDelegate)
        {
            SDKHtmlEditorExecuteEventArgs sDKHtmlEditorExecuteEventArgs = new();

            sDKHtmlEditorExecuteEventArgs.CommandName = args.CommandName;
            sDKHtmlEditorExecuteEventArgs.Editor = this;

            await Execute.InvokeAsync(sDKHtmlEditorExecuteEventArgs).ConfigureAwait(true);
        }
    }

}

