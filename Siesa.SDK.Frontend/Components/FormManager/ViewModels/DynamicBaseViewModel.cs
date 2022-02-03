using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;
using Siesa.SDK.Business;
using Siesa.SDK.Frontend.Utils;


namespace Siesa.SDK.Frontend.Components.FormManager.ViewModels
{
    public abstract class DynamicBaseViewModel: ComponentBase
    {
        [Parameter]
        public string BusinessName { get; set; }

        [Parameter]
        public string BusinessObjId { get; set; }

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
                    BusinessObj = Activator.CreateInstance(businessType);
                    BusinessModel = businessModel;
                    BusinessObj.BusinessName = BusinessName;
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
            InitGenericView();
        }

        public RenderFragment CreateDynamicComponent() => builder =>
        {
            var viewType = typeof(Views.CreateView);
            builder.OpenComponent(0, viewType);
            builder.AddAttribute(1, "BusinessObj", BusinessObj);
            builder.AddAttribute(2, "BusinessName", BusinessName);
            builder.CloseComponent();
        };

        protected override void OnParametersSet(){
            InitGenericView();
        }
    }
}