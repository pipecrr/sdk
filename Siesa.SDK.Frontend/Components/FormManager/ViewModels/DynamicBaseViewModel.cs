using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;
using Siesa.SDK.Business;
using Siesa.SDK.Frontend.Utils;
using Microsoft.Extensions.DependencyInjection;
using Siesa.SDK.Shared.Services;
using System.Collections.Generic;
using Siesa.SDK.Shared.Utilities;
using Siesa.Global.Enums;
using System.Reflection;
using Siesa.SDK.Shared.DataAnnotations;
using System.Linq;
using Siesa.SDK.Shared.DTOS;
namespace Siesa.SDK.Frontend.Components.FormManager.ViewModels
{
    public abstract class DynamicBaseViewModel: ComponentBase
    {
        [Parameter]
        public string BusinessName { get; set; }
        [Parameter]
        public string BLNameParentAttatchment { get; set; }
        [Parameter]
        public string BusinessObjId { get; set; }

        [Parameter] 
        public bool ShowTitle {get; set;} = true;
        [Parameter] 
        public bool ShowButtons {get; set;} = true;
        [Parameter] 
        public bool ShowCancelButton {get; set;} = true;
        [Parameter] 
        public bool ShowSaveButton {get; set;} = true;
        [Parameter] 
        public bool ShowDeleteButton {get; set;} = true;

        [Parameter]
        public Action<object> OnSave {get; set;} = null;

        [Parameter]
        public Action OnCancel {get; set;} = null;
        
        [Parameter] 
        public bool IsSubpanel { get; set; }

        [Parameter]
        public bool HideRelationshipContainer { get; set; }

        [Inject]
        public IServiceProvider ServiceProvider { get; set; }

        [Inject]
        public IAuthenticationService AuthenticationService {get; set;}

        [Inject]
        private IBackendRouterService BackendRouterService {get; set;}

        [Inject] 
        public IFeaturePermissionService FeaturePermissionService { get; set; }

        //[Inject]
        //public SGFState SGFState { get; set; }

        public string ErrorMsg { get; set; }

        public List<ModelMessagesDTO> ErrorList { get; set; } = new();

        public Type businessType;

        public dynamic BusinessObj { get; set; }

        public SDKBusinessModel BusinessModel { get; set; }

        protected IDictionary<string, object> parameters = new Dictionary<string, object>();


        public DynamicViewType ViewType { get; set; }

        protected bool CanAccess { get; set; }

        protected virtual async Task CheckAccessPermission(bool disableAccessValidation = false)
        {
            if(!BusinessName.Equals("BLAttachmentDetail"))
            {
                CanAccess = await FeaturePermissionService.CheckUserActionPermission(BusinessName, enumSDKActions.Access, AuthenticationService);
            }else
            {
                CanAccess = await FeaturePermissionService.CheckUserActionPermission(BLNameParentAttatchment, enumSDKActions.AccessAttachment, AuthenticationService);
            }

            if(!disableAccessValidation && !CanAccess)
            {
                this.ErrorMsg = "Custom.Generic.Unauthorized";
                
                ErrorList.Add(new ModelMessagesDTO()
                {
                    Message = "Custom.Generic.Unauthorized"
                });
            }

            StateHasChanged();
        }
        
        protected virtual async Task InitGenericView(string bName=null)
        {
            SDKBusinessModel businessModel;
            if (bName == null) {
                bName = this.BusinessName;
            }
            businessModel = BackendRouterService.GetSDKBusinessModel(bName, AuthenticationService);
           
            try
            {
                businessType = Utilities.SearchType(businessModel.Namespace + "." + businessModel.Name);

                if (businessType is null)
                {
                    ErrorMsg = $"Business not found in Front: {bName}";
                    ErrorList.Add(new ModelMessagesDTO()
                    {
                        Message = "Custom.Generic.FrontendBusinessNotFound"
                    });
                    return;
                }
                
                BusinessObj = ActivatorUtilities.CreateInstance(ServiceProvider, businessType);
                BusinessModel = businessModel;
                BusinessObj.BusinessName = bName;
            }
            catch (System.Exception ex)
            {
                string stringError = $"{ex.Message} {ex.StackTrace}";
                ErrorMsg = ex.ToString();

                ErrorList.Add(new ModelMessagesDTO()
                {
                    Message = "Custom.Generic.BackendBusinessNotFound",
                    StackTrace = stringError
                });
            }
            StateHasChanged();
        }

        protected override async Task OnInitializedAsync()
        {
            if(!string.IsNullOrEmpty(BusinessName)) //TODO: Check if this is necessary
            {
                await CheckAccessPermission().ConfigureAwait(true);
            } 
            
            await base.OnInitializedAsync().ConfigureAwait(true);

            SetParameters(BusinessObj, BusinessName);
            if(BusinessObj != null){
                long rowid;
                try
                {
                    rowid = Convert.ToInt64(BusinessObjId);
                }
                catch (System.Exception)
                {
                    rowid = 0;
                }
                BusinessObj.OnReady(ViewType, rowid);
            }
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            SetParameters(BusinessObj, BusinessName);
        }

        protected virtual void SetParameters(dynamic businessObj, string businessName){
            parameters.Clear();
            parameters.Add("BusinessObj", businessObj);
            parameters.Add("BusinessName", businessName);
            parameters.Add("IsSubpanel", IsSubpanel);            
            if (IsSubpanel)
            {
                parameters.Add("SetTopBar", false);                
            }
        }
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            try
            {
                
                if (parameters.TryGetValue<string>(nameof(BusinessName), out var value))
                {
                    if (value != null && value != BusinessName)
                    {
                        BusinessObj = null;
                        businessType = null;
                        BusinessModel = null;
                        ErrorMsg = "";
                        ErrorList = new ();

                        //await base.SetParametersAsync(parameters);

                        await InitGenericView(value);
                    }

                }
            }
            catch (Exception ex)
            {
                string stringError = $"{ex.Message} {ex.StackTrace}";
                if(!ErrorList.Any(x => x.StackTrace.Equals(stringError, StringComparison.Ordinal)))
                {
                    ErrorList.Add(new ModelMessagesDTO()
                    {
                        Message = "Custom.Generic.FrontendBusinessNotFound",
                        StackTrace = stringError
                    });
                }
            }
            await base.SetParametersAsync(parameters).ConfigureAwait(true);
        }
    }
}