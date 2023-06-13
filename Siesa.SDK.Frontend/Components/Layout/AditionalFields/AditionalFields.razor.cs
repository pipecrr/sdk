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
        [Parameter] public E00250_DynamicEntity DynamicEntity { get; set; } = new E00250_DynamicEntity();
        private string Title = "Custom.Group.AditionalFields.Title";
        private int Page = 0;        
        private List<E00251_DynamicEntityColumn> DynamicEntityColumns = new List<E00251_DynamicEntityColumn>();
        private int SizeField = 0; 
        private int TypeGroupDynamicEntity = 1;
        private bool EnableButtonNext = false;
        private bool EnableButtonSave = false;
        protected override async Task OnInitializedAsync(){
            E00251_DynamicEntityColumn DynamicEntityColumn = new E00251_DynamicEntityColumn();
            DynamicEntityColumns.Add(DynamicEntityColumn);
            base.OnInitialized();
        }
        
        public void Close(){
            SDKDialogService.Close(false);
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
            if(TypeGroupDynamicEntity == 1){
                DynamicEntity.IsMultiRecord = false;
            }else{
                DynamicEntity.IsMultiRecord = true;
            }

            DynamicEntity.Id = $"ED_{DynamicEntity.Tag}";

            var responseGroupsDynamicEntity = await Business.Backend.Call("SaveGroupsDynamicEntity", DynamicEntity);
            if(responseGroupsDynamicEntity.Success){
                var RowidGroupsDynamicEntity = responseGroupsDynamicEntity.Data;
                foreach(var DynamicEntityColumn in DynamicEntityColumns){
                    DynamicEntityColumn.RowidDynamicEntity = RowidGroupsDynamicEntity;
                    DynamicEntityColumn.Id = $"CED_{DynamicEntityColumn.Tag}";
                    
                    var responseGroupsDynamicEntityColumn = await Business.Backend.Call("SaveDynamicEntityColumn", DynamicEntityColumn);
                }
            }
            SDKDialogService.Close(true);
        }

        public void DeleteButton(int index){
            DynamicEntityColumns.RemoveAt(index);
            OnChangeTagField();
        }
        public void AddButton(){
            E00251_DynamicEntityColumn DynamicEntityColumn = new E00251_DynamicEntityColumn();
            DynamicEntityColumns.Add(DynamicEntityColumn);
            EnableButtonSave = false;
            StateHasChanged();
        }

        public void OnChangeTag(){
            if(string.IsNullOrEmpty(DynamicEntity.Tag)){
                EnableButtonNext = false;
            }else{
                EnableButtonNext = true;
            }
            StateHasChanged();
        }

        public void OnChangeTagField(){
            foreach(var DynamicEntityColumn in DynamicEntityColumns){
                if(string.IsNullOrEmpty(DynamicEntityColumn.Tag)){
                    EnableButtonSave = false;
                    break;
                }else{
                    EnableButtonSave = true;
                }
            }
            StateHasChanged();
        }
    }
}