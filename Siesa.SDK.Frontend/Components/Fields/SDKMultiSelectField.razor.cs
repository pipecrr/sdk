using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Linq;
using System.Globalization;
using Radzen.Blazor;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Frontend.Services;
using System.Collections;
using System.Reflection;

namespace Siesa.SDK.Frontend.Components.Fields;

/// <summary>
/// Multi Select Field component
/// </summary>
/// <typeparam name="TItem">The type of the value.</typeparam>
public partial class SDKMultiSelectField<TItem> : SDKComponent
{
    /// <summary>
    /// Gets or sets the ValueExpression.
    /// </summary>
    [Parameter] public Expression<Func<TItem>> ValueExpression { get; set; }

    /// <summary>
    /// Gets or sets the Value. Represents the Selected Value
    /// </summary>
    [Parameter] public TItem Value { get; set; } = default!;

    /// <summary>
    /// Gets or sets the ValueChanged. Event when a new Value is selected
    /// </summary>
    [Parameter] public Action<TItem> ValueChanged { get; set; } = (value) => { };

    /// <summary>
    /// Gets or sets the ChildContent. Represents the content of the component
    /// 
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the Options. Represents the list of options
    /// </summary>
    [Parameter] public IEnumerable Options { get; set; }

    /// <summary>
    /// Gets or sets the Placeholder.
    /// </summary>
    [Parameter] public string Placeholder { get; set; }

    /// <summary>
    /// Gets or sets the Disabled.
    /// </summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets the ReadOnly.
    /// </summary>
    [Parameter] public bool ReadOnly { get; set; }

    /// <summary>
    /// Gets or sets the Required.
    /// </summary>

    [Parameter] public bool Required { get; set; }

    /// <summary>
    /// Gets or sets the CssClass.
    /// </summary>
    /// 
    [Parameter] public string CssClass { get; set; }

    /// <summary>
    /// Gets or sets the FieldName.
    /// </summary>
    [Parameter] public string FieldName { get; set; }

    /// <summary>
    /// Gets or sets the OnFocusOut.
    /// </summary>
    [Parameter] public Action OnFocusOut { get; set; } = () => { };

    /// <summary>
    /// Gets or sets IsSearch.
    /// Is true when the component is to be painted in a lookup view.
    ///false for any of the other views
    /// </summary>
    [Parameter] public bool IsSearch { get; set; }

    /// <summary>
    /// Gets or sets the multiple selector, dafault true
    /// </summary>
    [Parameter] public bool Multiple { get; set; } = true;

    /// <summary>
    /// Gets or sets the AllowClear, dafault true
    /// </summary>
    [Parameter] public bool AllowClear { get; set; } = true;

    /// <summary>
    /// Gets or sets whether selected values are displayed as badges, dafault false
    /// </summary>
    [Parameter] public bool Badges { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of selected values displayed in the field, dafault 4
    /// </summary>
    [Parameter] public int MaxSelectedLabels { get; set; }

    /// <summary>
    /// Gets or sets the TextProperty. Represents the property of the object that will be displayed in the dropdown
    /// </summary>
    [Parameter] public string TextProperty { get; set; }

    /// <summary>
    /// Gets or sets the ValueProperty. Represents the property of the object that will be used as the value of the dropdown
    /// </summary>
    [Parameter] public string ValueProperty { get; set; }

    /// <summary>
    /// Gets or sets the EnumType. Represents the type of the enum
    /// </summary>
    [Parameter]
    public Type EnumType { get; set; }

    private IEnumerable<dynamic> _optionsEnums = new List<dynamic>();
    private IEnumerable<SDKEnumWrapper<Enum>> _options = new List<SDKEnumWrapper<Enum>>();


    protected override async Task OnInitializedAsync()
    {   
        if (EnumType != null)
        {
            await GetEnumValues().ConfigureAwait(true);
        }
        await base.OnInitializedAsync().ConfigureAwait(true);
    }

    protected override string GetAutomationId()
    {
        if (string.IsNullOrEmpty(AutomationId))
        {
            AutomationId = FieldName;
        }
        return base.GetAutomationId();
    }

    private void _OnFocusOut()
    {
        if (OnFocusOut != null)
        {
            OnFocusOut();
        }
    }
    private async Task GetEnumValues()
    {
        if (EnumType.IsEnum || (EnumType.IsGenericType && EnumType.GetGenericTypeDefinition() == typeof(Nullable<>) && EnumType.GetGenericArguments()[0].IsEnum))
        {
            if (EnumType.IsGenericType && EnumType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                EnumType = EnumType.GetGenericArguments()[0];
            }

            //_optionsEnums = Enum.GetValues(EnumType).Cast<Enum>();

            Dictionary<byte, string> enumValues = await ResourceManager.GetEnumValues(EnumType.Name, AuthenticationService.GetRoiwdCulture()).ConfigureAwait(true);

            if (enumValues == null || enumValues.Count == 0)
            {
                return;
            }


            foreach (var option in enumValues)
            {
                // Type sdkEnumWrapperType = typeof(SDKEnumWrapper<>).MakeGenericType(EnumType);
                // dynamic sdkEnumWrapperInstance = Activator.CreateInstance(sdkEnumWrapperType);

                //  sdkEnumWrapperInstance.DisplayText = option.Value;

                //  PropertyInfo propertyInfo = sdkEnumWrapperType.GetProperty("Type");
                //  propertyInfo.SetValue(sdkEnumWrapperInstance, Enum.ToObject(EnumType, option.Key));

                // _optionsEnums = _optionsEnums.Concat(new[] { sdkEnumWrapperInstance });
                 

                _options = _options.Append(new SDKEnumWrapper<Enum>
                {
                    Type= (Enum)Enum.ToObject(EnumType, option.Key),
                    DisplayText = option.Value
                });
            }

            TextProperty = "DisplayText";
            ValueProperty = "Type";

            _options = _options.Distinct();
            
            Options = _options;
        }
        StateHasChanged();
    }

    
}