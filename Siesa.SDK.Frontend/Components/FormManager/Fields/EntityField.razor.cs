using Microsoft.AspNetCore.Components;
using Siesa.SDK.Frontend.Components.FormManager.Model.Fields;
using Siesa.SDK.Frontend.Components.FormManager.ViewModels;

namespace Siesa.SDK.Frontend.Components.FormManager.Fields
{
    public partial class EntityField : FieldClass<dynamic>
    {
        [Parameter]
        public dynamic BaseModelObj { get; set; }
        [Parameter]
        public DynamicViewType ViewContext { get; set; }
        public string RelatedBusiness { get; set; }
        public bool AutoValueInUnique { get; set; } = false;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if(ViewContext == DynamicViewType.Create){
                AutoValueInUnique = FieldOpt.AutoValueInUnique;
            }
        }
    }

}