using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Siesa.SDK.Frontend.Components.FormManager.ViewModels;
using Siesa.Global.Enums;
using Siesa.SDK.Entities;
using Siesa.SDK.Frontend.Components.Fields;
using Siesa.SDK.Frontend.Components.FormManager.Model.Fields;

namespace Siesa.SDK.Frontend.Components.FormManager.Views
{
    public partial class CreateView : FormView
    {
        [Parameter]
        public bool SetTopBar { get; set; } = true;
        [Parameter]
        public string BLNameParentAttatchment { get; set; }
        private string UniqueId;
        private dynamic _refGrid;

        protected override async Task OnInitializedAsync()
        {
            UniqueId = Guid.NewGuid().ToString();
            DefaultViewdefName = String.IsNullOrEmpty(DefaultViewdefName) ? "create" : DefaultViewdefName;
            await BusinessObj.InstanceDynamicEntities(BusinessName);
            
            await base.OnInitializedAsync();
        }

        protected override async Task CheckPermissions()
        {
            if(IsSubpanel && BusinessName.Equals("BLAttachmentDetail"))
            {
                try
                {
                    CanCreate = await FeaturePermissionService.CheckUserActionPermission(BLNameParentAttatchment, enumSDKActions.UploadAttachment, AuthenticationService);
                }
                catch (System.Exception)
                {
                }
            }else
            {
                await base.CheckPermissions();
            }
            if(!CanCreate)
            {
                NotificationService.ShowError("Custom.Generic.Unauthorized");
                ErrorMsg = "Custom.Generic.Unauthorized";
                ErrorList.Add("Custom.Generic.Unauthorized");
                if(!IsSubpanel){
                    // NavigationService.NavigateTo("/", replace:true);
                }
            }
        }

        private string GetItemUniqueId(FieldOptions fld, bool isChild = false)
        {
            return $"{UniqueId}-{fld.Name}-{isChild}";
        }

        private void ClickAddRow()
        {
            Type typeChild = BusinessObj.GetTypeRelated();
            dynamic obj = Activator.CreateInstance(typeChild);
            if(BusinessObj.RelatedBaseObjects == null)
            {
                dynamic ListChildObj = typeof(List<>).MakeGenericType(new Type[] { typeChild });
                BusinessObj.RelatedBaseObjects = Activator.CreateInstance(ListChildObj);
            }
            BusinessObj.RelatedBaseObjects.Add(obj);
            _refGrid.Reload();
        }
    }
}
