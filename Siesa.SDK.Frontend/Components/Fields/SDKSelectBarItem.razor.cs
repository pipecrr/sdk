using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using System.Threading.Tasks;

namespace Siesa.SDK.Frontend.Components.Fields;

public partial class SDKSelectBarItem : SDKComponent
{
    /// <summary>
    /// Gets or sets the template.
    /// </summary>
    /// <value>The template.</value>
    [Parameter]
    public RenderFragment<RadzenSelectBarItem> Template { get; set; }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>The value.</value>
    [Parameter]
    public object Value { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="RadzenSelectBarItem"/> is disabled.
    /// </summary>
    /// <value><c>true</c> if disabled; otherwise, <c>false</c>.</value>
    [Parameter]
    public bool Disabled { get; set; }

    protected override async Task OnInitializedAsync()
    {
        ResourceTag = await GetText().ConfigureAwait(true);
        await base.OnInitializedAsync().ConfigureAwait(true);
    }

    protected override string GetAutomationId()
    {
        if(string.IsNullOrEmpty(AutomationId))
        {
            if (!string.IsNullOrEmpty(ResourceTag))
            {
                    AutomationId = ResourceTag;
            }
        }
        return base.GetAutomationId();
    }


}