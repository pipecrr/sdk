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
/// SDKMultiSelectField is a wrapper of the RadzenDropDown component.
/// </summary>
/// See the definition more parameters in: <see cref="https://blazor.radzen.com/dropdown-multiple"/>,
/// You can also enter the GitHub repository
/// <typeparam name="TValue">The type of the value.</typeparam>
public partial class SDKMultiSelectField<TValue> : RadzenDropDown<TValue>
{
    [Inject]
    private IResourceManager ResourceManager { get; set; }

    [Inject]
    private IAuthenticationService AuthenticationService { get; set; }

    [Parameter]
    public bool IsEnum { get; set; }

    [Parameter]
    public bool Badges { get; set; }

    private List<SDKEnumWrapper<TValue>> _optionsEnums = new();
    protected override async Task OnInitializedAsync()
    {
        Multiple = true;
        Chips = Badges;

        if (IsEnum)
        {
            await GetEnumValues().ConfigureAwait(true);
        }
        await base.OnInitializedAsync().ConfigureAwait(true);
    }

    private async Task GetEnumValues()
    {
        Type EnumType = typeof(TValue);
        if (EnumType.IsEnum || (EnumType.IsGenericType && EnumType.GetGenericTypeDefinition() == typeof(Nullable<>) && EnumType.GetGenericArguments()[0].IsEnum))
        {
            if (EnumType.IsGenericType && EnumType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                EnumType = EnumType.GetGenericArguments()[0];
            }

            Dictionary<byte, string> enumValues = await ResourceManager.GetEnumValues(EnumType.Name, AuthenticationService.GetRowidCulture()).ConfigureAwait(true);

            if (enumValues == null || enumValues.Count == 0)
            {
                return;
            }

            foreach (var option in enumValues)
            {
                _optionsEnums.Add(new SDKEnumWrapper<TValue> 
                { 
                    DisplayText = option.Value, 
                    Type = (TValue)Enum.ToObject(EnumType, option.Key) 
                });
            }

            TextProperty = "DisplayText";
            ValueProperty = "Type";

            _optionsEnums = _optionsEnums.Distinct().ToList();

            Data = _optionsEnums;

        }
        StateHasChanged();
    }

    protected override async Task OnSelectItem(object item, bool isFromKey = false)
    {
       await base.OnSelectItem(item, isFromKey).ConfigureAwait(true);

       SelectedItemChanged?.Invoke(item);
    }
}