using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using Siesa.SDK.Entities;
using System.Threading.Tasks;
using Siesa.SDK.Shared.Services;

namespace Siesa.SDK.Frontend.Components.Layout.AditionalFields
{
    public partial class AditionalFields{
        [Parameter] public dynamic Business { get; set; }
        [Inject] public IBackendRouterService BackendRouterService { get; set; }
        [Inject] public IAuthenticationService AuthenticationService { get; set; }
        private string Title = "Custom.Group.AditionalFields.Title";
        private int Page = 0;        
        private E00250_DynamicEntity DynamicEntity = new E00250_DynamicEntity();
        private List<E00251_DynamicEntityColumn> DynamicEntityColumns = new List<E00251_DynamicEntityColumn>();
        private int SizeField = 0; 
        
        protected override async Task OnInitializedAsync(){
            E00251_DynamicEntityColumn DynamicEntityColumn = new E00251_DynamicEntityColumn();
            DynamicEntityColumns.Add(DynamicEntityColumn);
            base.OnInitialized();
        }
        public void Close(){
            SDKDialogService.Close();
        }

        public void Next(){
            Page = 1;
            Title = "Custom.Fields.AditionalFields.Title";
            StateHasChanged();
        }

        public void Back(){
            Page = 0;
            Title = "Custom.Group.AditionalFields.Title";
            StateHasChanged();
        }

        public async Task Save(){
            var BlFeacture = BackendRouterService.GetSDKBusinessModel("BLFeature", AuthenticationService);
            var resultFeature = await BlFeacture.Call("GetFeatureRowid", Business.BusinessName);
             if(resultFeature.Success){
                var RowidFeature = resultFeature.Data;
                DynamicEntity.RowidFeature = RowidFeature;
             }

            await Business.SaveGroupsDynamicEntity(DynamicEntity);
            SDKDialogService.Close();
        }

        public void DeleteButton(int index){
            DynamicEntityColumns.RemoveAt(index);
            StateHasChanged();
        }

        public void AddButton(){
            E00251_DynamicEntityColumn DynamicEntityColumn = new E00251_DynamicEntityColumn();
            DynamicEntityColumns.Add(DynamicEntityColumn);
            StateHasChanged();
        }
    }
}