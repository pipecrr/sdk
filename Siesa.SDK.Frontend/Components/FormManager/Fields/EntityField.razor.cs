using Microsoft.AspNetCore.Components;
using Siesa.SDK.Frontend.Components.FormManager.Model.Fields;

namespace Siesa.SDK.Frontend.Components.FormManager.Fields 
{
    public partial class EntityField : FieldClass<dynamic>
    {
        [Parameter]
        public dynamic BaseModelObj { get; set; }
        
        public string RelatedBusiness { get; set; }
    }

}