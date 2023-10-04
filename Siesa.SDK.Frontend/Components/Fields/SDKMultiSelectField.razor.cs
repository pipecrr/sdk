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

namespace Siesa.SDK.Frontend.Components.Fields;

/// <summary>
/// Multi Select Field component
/// </summary>
/// <typeparam name="TItem">The type of the value.</typeparam>
public class SDKMultiSelectField<TItem> : RadzenDropDown<TItem>
{

    [Inject]
    private IResourceManager ResourceManager { get; set; }

    [Inject]
    private IAuthenticationService AuthenticationService { get; set; }

    [Parameter]
    public Type enumType { get; set; }

    private IEnumerable<SDKEnumWrapper<int>> _optionsEnums;

    protected override async Task OnInitializedAsync()
    {
        Multiple = true;
        
        if (enumType != null)
        {
            await GetEnumValues().ConfigureAwait(true);
        }
        await base.OnInitializedAsync().ConfigureAwait(true);
    }

    private async Task GetEnumValues()
    {
        if (enumType.IsEnum || (enumType.IsGenericType && enumType.GetGenericTypeDefinition() == typeof(Nullable<>) && enumType.GetGenericArguments()[0].IsEnum))
        {
            if (enumType.IsGenericType && enumType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                enumType = enumType.GetGenericArguments()[0];
            }

            Dictionary<byte, string> enumValues = await ResourceManager.GetEnumValues(enumType.Name, AuthenticationService.GetRoiwdCulture()).ConfigureAwait(true);

            if (enumValues == null || enumValues.Count == 0)
            {
                return;
            }
            _optionsEnums = new List<SDKEnumWrapper<int>>();


            foreach (var option in enumValues)
            {
                _optionsEnums = _optionsEnums.Append(new SDKEnumWrapper<int>
                {
                    DisplayText = option.Value,
                    Type = option.Key
                });
            }

            TextProperty = "DisplayText";
            ValueProperty = "Type";

            _optionsEnums = _optionsEnums.Distinct();
            
            Data = _optionsEnums;
        }
        StateHasChanged();
    }

    
}