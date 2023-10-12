using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using DevExpress.Blazor;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Siesa.SDK.Shared.Services;
using System.Globalization;

namespace Siesa.SDK.Frontend.Components.Fields
{
    /// <summary>
    /// Select Field component
    /// </summary>
    /// <typeparam name="ItemType">The type of the value.</typeparam>
    public partial class SDKSelectField<ItemType> : SDKComponent
    {
        private DxComboBox<SDKEnumWrapper<ItemType>, ItemType> _refField;

        /// <summary>
        /// Gets or sets the ValueExpression.
        /// </summary>
        [Parameter] public Expression<Func<ItemType>> ValueExpression { get; set; }
        
        /// <summary>
        /// Gets or sets the Value. Represents the Selected Value
        /// </summary>
        [Parameter] public ItemType Value { get; set; } = default!;

        /// <summary>
        /// Gets or sets the ValueChanged. Event when a new Value is selected
        /// </summary>
        [Parameter] public Action<ItemType> ValueChanged { get; set; } = (value) => { };

        /// <summary>
        /// Gets or sets the ChildContent. Represents the content of the component
        /// 
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Gets or sets the Options. Represents the list of options
        /// can be of type enum or SDKEnumWrapper
        /// If it is of type Enum, it is only necessary to specify the type in ItemType
        /// </summary>
        [Parameter] public IEnumerable<SDKEnumWrapper<ItemType>> Options { get; set; }

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

        private IEnumerable<SDKEnumWrapper<ItemType>> _options { get; set; }
        private Type enumType { get; set; }

        private string _textProperty { get; set; } = "DisplayText";

        private string _valueProperty { get; set; } = "Type";


        /// <summary>
        /// Constructor Component
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            await GetEnumValues().ConfigureAwait(true);
            await base.OnInitializedAsync().ConfigureAwait(true);
        }
        private void onKeyDown(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                StateHasChanged();
            }
        }


        /// <summary>
        /// Gets the automation identifier.
        /// </summary>
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
            enumType = typeof(ItemType);

            if (enumType.IsEnum || (enumType.IsGenericType && enumType.GetGenericTypeDefinition() == typeof(Nullable<>) && enumType.GetGenericArguments()[0].IsEnum))
            {

                if (enumType.IsGenericType && enumType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    enumType = enumType.GetGenericArguments()[0];
                }

                Dictionary<byte, string> enumValues = await ResourceManager.GetEnumValues(enumType.Name, AuthenticationService.GetRowidCulture()).ConfigureAwait(true);

                if (enumValues == null || enumValues.Count == 0)
                {
                    return;
                }
                _options = Enumerable.Empty<SDKEnumWrapper<ItemType>>();

                if (IsSearch)
                {
                    var SelectAllName = await ResourceManager.GetResource("Custom.Enum.SelectAll", AuthenticationService).ConfigureAwait(true);
                    _options = _options.Append(new SDKEnumWrapper<ItemType>
                    {
                        Type = (ItemType)Enum.Parse(enumType, (enumValues.Select(x => x.Key).Max() + 1).ToString(CultureInfo.InvariantCulture)),
                        DisplayText = SelectAllName
                    });

                    Value = _options.Select(x => x.Type).First();
                }

                foreach (var option in enumValues)
                {
                    _options = _options.Append(new SDKEnumWrapper<ItemType>
                    {
                        Type = (ItemType)Enum.Parse(enumType, option.Key.ToString(CultureInfo.InvariantCulture)),
                        DisplayText = option.Value
                    });
                }
            }
            else
            {
                _options = Options;
            }

            _options = _options.Distinct();

            StateHasChanged();
        }

    }

}