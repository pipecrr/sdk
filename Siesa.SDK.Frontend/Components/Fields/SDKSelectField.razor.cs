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

namespace Siesa.SDK.Frontend.Components.Fields
{
    public partial class SDKSelectField<ItemType> : SDKComponent
    {
        private DxComboBox<SDKEnumWrapper<ItemType>, ItemType> _refField;
        [Parameter] public Expression<Func<ItemType>> ValueExpression { get; set; }
        [Parameter] public ItemType Value { get; set; } = default!;
        [Parameter] public Action<ItemType> ValueChanged { get; set; } = (value) => { };
        [Parameter] public RenderFragment? ChildContent { get; set; }

        [Parameter] public IEnumerable<SDKEnumWrapper<ItemType>> Options { get; set; }
        [Parameter] public string Placeholder { get; set; }
        [Parameter] public bool Disabled { get; set; }
        [Parameter] public bool ReadOnly { get; set; }
        [Parameter] public bool Required { get; set; }
        [Parameter] public string CssClass { get; set; }
        [Parameter] public string TextProperty { get; set; }
        [Parameter] public string ValueProperty { get; set; }
        [Parameter] public string FieldName { get; set; }
        [Parameter] public Action OnFocusOut { get; set; } = () => { };
        [Parameter] public bool IsSearch { get; set; }
        private IEnumerable<SDKEnumWrapper<ItemType>> _options {get; set;} 
        private Type enumType { get; set; }



        /*protected override async Task OnParametersSetAsync()
        {
            await GetEnumValues().ConfigureAwait(true);
            await base.OnParametersSetAsync().ConfigureAwait(true);
        }   */

        protected override async Task OnInitializedAsync()
        {
            await GetEnumValues().ConfigureAwait(true);
            await base.OnInitializedAsync().ConfigureAwait(true);
        }     

        public void RefreshCurrentText()
        {


        }

        private void onKeyDown(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                StateHasChanged();
            }
        }

        private async Task<LoadResult> GetData(DataSourceLoadOptionsBase options, CancellationToken cancellationToken)
        {
            return new LoadResult
            {
                data = Options,
                totalCount = Options.Count()
            };
        }

        protected override string GetAutomationId()
        {
            if(string.IsNullOrEmpty(AutomationId))
            {
                AutomationId = FieldName;
            }
            return base.GetAutomationId();
        }

        private async Task _OnFocusOut()
        {
            if(OnFocusOut != null){
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

                Dictionary<byte, string> enumValues = await ResourceManager.GetEnumValues(enumType.Name, AuthenticationService.GetRoiwdCulture()).ConfigureAwait(true);

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
                        Type = (ItemType)Enum.Parse(enumType, (enumValues.Select(x => x.Key).Max() + 1).ToString()),
                        DisplayText = SelectAllName
                    });

                    Value = _options.Select(x => x.Type).First();
                }

                foreach (var option in enumValues)
                {
                    _options = _options.Append(new SDKEnumWrapper<ItemType>
                    {
                        Type = (ItemType)Enum.Parse(enumType, option.Key.ToString()),
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