using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;
using Siesa.SDK.Business;
using Siesa.SDK.Frontend.Utils;
using Microsoft.Extensions.DependencyInjection;
using Siesa.SDK.Shared.Services;
using System.Collections.Generic;
using Siesa.SDK.Shared.Utilities;
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

        [Inject]
        public IServiceProvider ServiceProvider { get; set; }

        [Inject]
        private IAuthenticationService AuthenticationService {get; set;}

        [Inject]
        private IBackendRouterService BackendRouterService {get; set;}

        //[Inject]
        //public SGFState SGFState { get; set; }

        public string ErrorMsg { get; set; }

        public Type businessType;

        public dynamic BusinessObj { get; set; }

        public SDKBusinessModel BusinessModel { get; set; }

        protected IDictionary<string, object> parameters = new Dictionary<string, object>();

        [Parameter] 
        public bool IsSubpanel { get; set; }

        public DynamicViewType ViewType { get; set; }

        protected void InitGenericView(string bName=null)
        {
            SDKBusinessModel businessModel;
            if (bName == null) {
                bName = this.BusinessName;
            }
            businessModel = BackendRouterService.GetSDKBusinessModel(bName, AuthenticationService);
            if (businessModel != null)
            {
                try
                {
                    businessType = Utilities.SearchType(businessModel.Namespace + "." + businessModel.Name); 
                    BusinessObj = ActivatorUtilities.CreateInstance(ServiceProvider, businessType);
                    BusinessModel = businessModel;
                    BusinessObj.BusinessName = bName;
                }
                catch (System.Exception e)
                {
                    Console.WriteLine("Error BaseViewModel" + e.ToString());
                    ErrorMsg = e.ToString();
                }
            }
            else
            {
                this.ErrorMsg = "404 Not Found.";
            }
            StateHasChanged();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
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


                        InitGenericView(value);
                    }

                }
            }
            catch (Exception e)
            {

            }

            await base.SetParametersAsync(parameters);
        }
    }
}