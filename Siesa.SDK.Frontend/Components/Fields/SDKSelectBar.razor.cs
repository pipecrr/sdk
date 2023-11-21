using Microsoft.AspNetCore.Components;
using Siesa.SDK.Frontend.Components.FormManager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace Siesa.SDK.Frontend.Components.Fields;

    /// <summary>
/// Represents a partial class for a SDKSelectBar.
/// </summary>
/// <typeparam name="ItemType">The type of items in the  SDKSelectBar.</typeparam>
public partial class SDKSelectBar<ItemType> : SDKComponent
{
    /// <summary>
    /// Gets or sets the data for the SDKSelectBar.
    /// </summary>
    [Parameter] public IEnumerable<SelectBarItemWrap<ItemType>> Data { get; set; }

    /// <summary>
    /// Gets or sets the currently selected value in the SDKSelectBar.
    /// </summary>
    [Parameter] public ItemType Value { get; set; }

    /// <summary>
    /// Gets or sets the action to be executed when the value in the SDKSelectBar changes.
    /// </summary>
    [Parameter] public Action<ItemType> ValueChanged { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the SDKSelectBar is disabled.
    /// </summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets the expression representing the value in the SDKSelectBar.
    /// </summary>
    [Parameter] public Expression<Func<ItemType>> ValueExpression { get; set; }

    /// <summary>
    /// Gets or sets the field name associated with the SDKSelectBar.
    /// </summary>
    [Parameter] public string FieldName { get; set; }

    /// <summary>
    /// Gets or sets the resource tag for denial messages associated with the SDKSelectBar.
    /// </summary>
    [Parameter] public string DenialResourceTag { get; set; }

    /// <summary>
    /// Gets or sets the resource tag for affirmation messages associated with the SDKSelectBar.
    /// </summary>
    [Parameter] public string AffirmationResourceTag { get; set; }

    /// <summary>
    /// Gets or sets the render fragment for customizing the appearance of items in the SDKSelectBar.
    /// </summary>
    [Parameter]
    public RenderFragment Items { get; set; }

    [Inject] 
    private UtilsManager UtilManager {get; set;}

    protected override string GetAutomationId()
    {
        if(string.IsNullOrEmpty(AutomationId))
        {
            if (!string.IsNullOrEmpty(FieldName))
            {
                    AutomationId = FieldName;
            }
        }
        return base.GetAutomationId();
    }

    private async Task GetResources()
    {
        if(Data != null && Data.Any())
        {
            foreach(var item in Data)
            {
                var resourceTag  = await UtilManager.GetResource(item.Name).ConfigureAwait(true);

                if(!string.IsNullOrEmpty(resourceTag))
                {
                    item.Name = resourceTag;
                }
            }
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync().ConfigureAwait(true);
        await GetResources().ConfigureAwait(true);
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync().ConfigureAwait(true);
        await GetResources().ConfigureAwait(true);
    }

    }

