using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

using Siesa.SDK.Frontend.Components;

namespace Siesa.SDK.Frontend.Components.Visualization;
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


    /// <summary>
    /// Gets or sets the child content.
    /// </summary>
    /// <value>The child content.</value>
    [Parameter]
    public RenderFragment ChildContent { get; set; }


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
    /// <example>
    /// <code>
    /// &lt;RadzenHtmlEditor @bind-Value=@html Paste=@OnPaste /&gt;
    /// @code {
    ///   string html = "@lt;strong&gt;Hello&lt;/strong&gt; world!";
    ///   void OnPaste(HtmlEditorPasteEventArgs args)
    ///   {
    ///     // Set args.Html to filter unwanted tags.
    ///     args.Html = args.Html.Replace("&lt;br&gt;", "");
    ///   }
    /// </code>
    /// </example>
    [Parameter]
    public EventCallback<HtmlEditorPasteEventArgs> Paste { get; set; }

    /// <summary>
    /// A callback that will be invoked when there is an error during upload.
    /// </summary>
    [Parameter]
    public EventCallback<UploadErrorEventArgs> UploadError { get; set; }

    /// <summary>
    /// A callback that will be invoked when the user executes a command of the editor (e.g. by clicking one of the tools).
    /// </summary>
    /// <example>
    /// <code>
    /// &lt;RadzenHtmlEditor Execute=@OnExecute&gt;
    ///   &lt;RadzenHtmlEditorCustomTool CommandName="InsertToday" Icon="today" Title="Insert today" /&gt;
    /// &lt;/RadzenHtmlEditor&gt;
    /// @code {
    ///   string html = "@lt;strong&gt;Hello&lt;/strong&gt; world!";
    ///   async Task OnExecute(HtmlEditorExecuteEventArgs args)
    ///   {
    ///     if (args.CommandName == "InsertToday")
    ///     {
    ///       await args.Editor.ExecuteCommandAsync(HtmlEditorCommands.InsertHtml, DateTime.Today.ToLongDateString());
    ///     }
    ///  }
    /// </code>
    /// </example>
    [Parameter]
    public EventCallback<HtmlEditorExecuteEventArgs> Execute { get; set; }

    /// <summary>
    /// Specifies the URL to which RadzenHtmlEditor will submit files.
    /// </summary>
    [Parameter]
    public string UploadUrl { get; set; }

    [Parameter]
    public EventCallback<string> ChangeValue { get; set; }

}