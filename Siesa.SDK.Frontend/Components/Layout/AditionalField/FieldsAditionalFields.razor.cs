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
        private bool ReadOnlyMinMax;
        List<SelectBarItemWrap<int>> DataType = new List<SelectBarItemWrap<int>>();
        
        List<SelectBarItemWrap<int>> DataTypeVisualization = new List<SelectBarItemWrap<int>>();

        protected override async Task OnInitializedAsync()
        {   
            if(DynamicEntityColumn.Rowid !=0 && DynamicEntityColumn.DataType != enumDynamicEntityDataType.Number){
                ReadOnlyMinMax = true;
            }else{
                ReadOnlyMinMax = false;
                DynamicEntityColumn.DataType = enumDynamicEntityDataType.Number;
            }
            SetType();
            var TextNumeric = await ResourceManager.GetResource("Custom.Fields.AditionalFields.Type.RadioNumeric", AuthenticationService);
            var TextDecimal = await ResourceManager.GetResource("Custom.Fields.AditionalFields.Type.RadioDecimal", AuthenticationService);
            var TextDate = await ResourceManager.GetResource("Custom.Fields.AditionalFields.Type.RadioDate", AuthenticationService);
            var TextText = await ResourceManager.GetResource("Custom.Fields.AditionalFields.Type.RadioText", AuthenticationService);

            DataType.Add(new SelectBarItemWrap<int>() {Key=1, Name=TextNumeric});
            DataType.Add(new SelectBarItemWrap<int>() {Key=2, Name=TextDecimal});
            DataType.Add(new SelectBarItemWrap<int>() {Key=3, Name=TextDate});
            DataType.Add(new SelectBarItemWrap<int>() {Key=4, Name=TextText});

            var TextDefault = await ResourceManager.GetResource("Custom.Fields.AditionalFields.TypeVisualization.RadioDefault", AuthenticationService);
            var TextButtonGroup = await ResourceManager.GetResource("Custom.Fields.AditionalFields.TypeVisualization.RadioButtonGroup", AuthenticationService);

            DataTypeVisualization.Add(new SelectBarItemWrap<int>() {Key=1, Name=TextDefault});
            DataTypeVisualization.Add(new SelectBarItemWrap<int>() {Key=2, Name=TextButtonGroup});

            base.OnInitialized();            
        }

        public void SetType(){
            switch (DynamicEntityColumn.DataType){
                case enumDynamicEntityDataType.Number:
                    Type = 1;
                    break;
                case enumDynamicEntityDataType.Date:
                    Type = 3;
                    break;
                case enumDynamicEntityDataType.Text:
                    Type = 4;
                    break;
                default:
                    Type = 4;
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
            ReadOnlyMinMax = true;
            switch (Type){
                case 1:
                case 2:
                    DynamicEntityColumn.DataType = enumDynamicEntityDataType.Number;
                    ReadOnlyMinMax = false;
                    break;
                case 3:
                    DynamicEntityColumn.DataType = enumDynamicEntityDataType.Date;
                    break;
                case 4:
                    DynamicEntityColumn.DataType = enumDynamicEntityDataType.Text;
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