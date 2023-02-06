using Microsoft.AspNetCore.Components;
using System;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Frontend.Application;
using System.Threading.Tasks;

namespace Siesa.SDK.Frontend.Components
{
    public abstract class SDKComponent : ComponentBase
    {
        [Parameter]
        public string AutomationId { get; set; }
        [Parameter] 
        public string ResourceTag { get; set; }
        [Parameter]
        public Int64? RowidResource { get; set; }
        [Parameter]
        public Int64 RowidCulture { get; set;} = 1;
        [Inject]
        public IAuthenticationService AuthenticationService { get; set; }
        [Inject]
        public IResourceManager ResourceManager { get; set; }

        protected virtual string GetAutomationId()
        {
            var componentType = this.GetType().Name;
            //split by ` and take the first part
            var componentTypeSplit = componentType.Split('`')[0];
            return $"{componentTypeSplit}_{AutomationId}";
        }

        protected async Task<string> GetText(){
            if((RowidResource == null || RowidResource == 0) && ResourceTag != null){
                if (AuthenticationService != null && AuthenticationService.GetRoiwdCulture() != 0)
                {
                    return await ResourceManager.GetResource(ResourceTag, AuthenticationService);
                }else
                {
                    return await ResourceManager.GetResource(ResourceTag, RowidCulture);
                }
            }
            return await ResourceManager.GetResource(Convert.ToInt64(RowidResource), AuthenticationService);
        }


    }
}