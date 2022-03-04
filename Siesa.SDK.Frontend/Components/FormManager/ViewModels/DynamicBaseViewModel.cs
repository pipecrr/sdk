using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;
using Siesa.SDK.Business;
using Siesa.SDK.Frontend.Utils;
using Microsoft.Extensions.DependencyInjection;
using Siesa.SDK.Shared.Services;

namespace Siesa.SDK.Frontend.Components.FormManager.ViewModels
{
    public abstract class DynamicBaseViewModel: ComponentBase
    {
        [Parameter]
        public string BusinessName { get; set; }

        [Parameter]
        public string BusinessObjId { get; set; }

        [Inject]
        public IServiceProvider ServiceProvider { get; set; }

        [Inject]
        private IAuthenticationService AuthenticationService {get; set;}

        //[Inject]
        //public SGFState SGFState { get; set; }

        public string ErrorMsg { get; set; }

        public Type businessType;

        public dynamic BusinessObj { get; set; }

        public BusinessFrontendModel BusinessModel { get; set; }

        protected void InitGenericView(string bName=null)
        {
            BusinessFrontendModel businessModel;
            if (bName == null) {
                bName = this.BusinessName;
            }

            BusinessManagerFrontend.Instance.Businesses.TryGetValue(bName, out businessModel);
            if (businessModel != null)
            {
                try
                {
                    businessType = Utils.Utils.SearchType(businessModel.Namespace + "." + businessModel.Name); 
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

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            //InitGenericView();
        }

        public RenderFragment CreateDynamicComponent() => builder =>
        {
            var viewType = typeof(Views.CreateView);
            builder.OpenComponent(0, viewType);
            builder.AddAttribute(1, "BusinessObj", BusinessObj);
            builder.AddAttribute(2, "BusinessName", BusinessName);
            builder.CloseComponent();
        };

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