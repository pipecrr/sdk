using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using Siesa.SDK.Entities;
using System.Threading.Tasks;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Frontend.Services;

namespace Siesa.SDK.Frontend.Components.Layout.AditionalField
{
    public partial class AditionalFields{
        [Inject] public IBackendRouterService BackendRouterService { get; set; }
        [Inject] public IAuthenticationService AuthenticationService { get; set; }
        [Inject] public SDKGlobalLoaderService GlobalLoaderService { get; set; }
        [Parameter] public dynamic Business { get; set; }
        [Parameter] public E00250_DynamicEntity DynamicEntity { get; set; } = new E00250_DynamicEntity();
        [Parameter] public bool IsEdit { get; set; }
        [Parameter] public bool IsCreate { get; set; }
        private string Title = "Custom.Group.AditionalFields.Title";
        private int Page;
        private List<E00251_DynamicEntityColumn> DynamicEntityColumns = new List<E00251_DynamicEntityColumn>();
        private int SizeField;
        private int TypeGroupDynamicEntity = 1;
        private bool EnableButtonNext;
        private bool EnableButtonSave;
        protected override async Task OnInitializedAsync(){
            if(IsEdit){
                EnableButtonSave = true;
            }
            if(DynamicEntity != null && DynamicEntity.Rowid != 0 && IsEdit){
                var responseGroupsDynamicEntity = await Business.Backend.Call("GetColumnsDynamicEntity", DynamicEntity.Rowid);
                if(responseGroupsDynamicEntity.Success){
                    DynamicEntityColumns = responseGroupsDynamicEntity.Data;
                }
            }else{
                E00251_DynamicEntityColumn DynamicEntityColumn = new E00251_DynamicEntityColumn();
                DynamicEntityColumns.Add(DynamicEntityColumn);
            }
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
            GlobalLoaderService.Show();
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
            var RowidGroupsDynamicEntity = DynamicEntity.Rowid;
            if(!IsEdit){
                var responseGroupsDynamicEntity = await Business.Backend.Call("SaveGroupsDynamicEntity", DynamicEntity);
                if(responseGroupsDynamicEntity.Success){
                    RowidGroupsDynamicEntity = responseGroupsDynamicEntity.Data;
                }
            }

            foreach(var DynamicEntityColumn in DynamicEntityColumns){
                DynamicEntityColumn.RowidDynamicEntity = RowidGroupsDynamicEntity;
                DynamicEntityColumn.Id = $"CED_{DynamicEntityColumn.Tag}";
                
                await Business.Backend.Call("SaveDynamicEntityColumn", DynamicEntityColumn);                
            }

            GlobalLoaderService.Hide();
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