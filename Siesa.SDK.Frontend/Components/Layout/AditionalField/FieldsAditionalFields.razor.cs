using System;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using Siesa.SDK.Entities;
using Siesa.SDK.Shared.Services;
using System.Threading.Tasks;
using Siesa.Global.Enums;
namespace Siesa.SDK.Frontend.Components.Layout.AditionalField
{
    public partial class FieldsAditionalFields{
        [Inject] public IResourceManager ResourceManager {get; set;}
        [Inject] public IAuthenticationService AuthenticationService { get; set; }
        [Parameter] public E00251_DynamicEntityColumn DynamicEntityColumn { get; set; } = new E00251_DynamicEntityColumn();
        [Parameter] public Action<int> OnDeleteField { get; set; }
        [Parameter] public Action OnAddField { get; set; }
        [Parameter] public int Index { get; set; }
        [Parameter] public bool ShowAddButton { get; set; }
        [Parameter] public Action OnChangeTag { get; set; }
        private int Type { get; set; } = 1;
        private bool HasMinMax;
        private string _resourceTagSize; 
        List<SelectBarItemWrap<int>> DataType = new List<SelectBarItemWrap<int>>();
        
        List<SelectBarItemWrap<int>> DataTypeVisualization = new List<SelectBarItemWrap<int>>();

        protected override async Task OnInitializedAsync()
        {
            if(!(DynamicEntityColumn.Rowid !=0 && DynamicEntityColumn.DataType != enumDynamicEntityDataType.Number)){
                DynamicEntityColumn.DataType = enumDynamicEntityDataType.Number;
                _resourceTagSize = "Custom.Fields.AditionalFields.SizeName";
            }else if(DynamicEntityColumn.DataType != enumDynamicEntityDataType.Text){
                _resourceTagSize = "Custom.Fields.AditionalFields.SizeTextName";
            }
            SetType();
            var TextNumeric = await ResourceManager.GetResource("Custom.Fields.AditionalFields.Type.RadioNumeric", AuthenticationService).ConfigureAwait(false);
            var TextText = await ResourceManager.GetResource("Custom.Fields.AditionalFields.Type.RadioText", AuthenticationService).ConfigureAwait(false);
            var TextDate = await ResourceManager.GetResource("Custom.Fields.AditionalFields.Type.RadioDate", AuthenticationService).ConfigureAwait(false);
            var TextHour = await ResourceManager.GetResource("Custom.Fields.AditionalFields.Type.RadioHour", AuthenticationService).ConfigureAwait(false);
            var TextBool = await ResourceManager.GetResource("Custom.Fields.AditionalFields.Type.RadioBool", AuthenticationService).ConfigureAwait(false);
            var InternalMaster = await ResourceManager.GetResource("Custom.Fields.AditionalFields.Type.RadioInternalMaster", AuthenticationService).ConfigureAwait(false);
            var GenericMaster = await ResourceManager.GetResource("Custom.Fields.AditionalFields.Type.RadioGenericMaster", AuthenticationService).ConfigureAwait(false);

            DataType.Add(new SelectBarItemWrap<int>() {Key=1, Name=TextNumeric});
            DataType.Add(new SelectBarItemWrap<int>() {Key=2, Name=TextText});
            DataType.Add(new SelectBarItemWrap<int>() {Key=3, Name=TextDate});
            DataType.Add(new SelectBarItemWrap<int>() {Key=4, Name=TextHour});
            DataType.Add(new SelectBarItemWrap<int>() {Key=5, Name=TextBool});
            DataType.Add(new SelectBarItemWrap<int>() {Key=6, Name=InternalMaster});
            DataType.Add(new SelectBarItemWrap<int>() {Key=7, Name=GenericMaster});

            base.OnInitialized();            
        }

        public void SetType(){
            switch (DynamicEntityColumn.DataType){
                case enumDynamicEntityDataType.Number:
                    Type = 1;
                    break;
                case enumDynamicEntityDataType.Text:
                    Type = 2;
                    break;
                case enumDynamicEntityDataType.Date:
                    Type = 3;
                    break;                
                default:
                    Type = 2;
                    break;
            }
        }

        public void DeleteField(){
            if(OnDeleteField != null)
                OnDeleteField(Index);
        }

        public void AddField(){
            if(OnAddField != null)
                OnAddField();
        }

        public int Onchange(int item){
            Type = item;
            switch (Type){
                case 1:
                    DynamicEntityColumn.DataType = enumDynamicEntityDataType.Number;
                    _resourceTagSize = "Custom.Fields.AditionalFields.SizeName";
                    break;
                case 2:
                    DynamicEntityColumn.DataType = enumDynamicEntityDataType.Text;
                    _resourceTagSize = "Custom.Fields.AditionalFields.SizeTextName";
                    break;
                case 3:
                    DynamicEntityColumn.DataType = enumDynamicEntityDataType.Date;
                    break;
                default:
                    DynamicEntityColumn.DataType = enumDynamicEntityDataType.Text;
                    break;
            }

            return item;
        }

        private string _OnChangeTag(string value){
            DynamicEntityColumn.Tag = value;
            if(OnChangeTag != null){
                OnChangeTag();
            }

            return value;            
        }
    }

}