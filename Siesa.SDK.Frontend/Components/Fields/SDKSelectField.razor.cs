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
    }

}