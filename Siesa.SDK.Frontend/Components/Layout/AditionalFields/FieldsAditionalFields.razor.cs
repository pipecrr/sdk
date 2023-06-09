using System;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using Siesa.SDK.Entities;

namespace Siesa.SDK.Frontend.Components.Layout.AditionalFields
{
    public partial class FieldsAditionalFields{
        [Parameter] public E00251_DynamicEntityColumn DynamicEntityColumn { get; set; } = new E00251_DynamicEntityColumn();
        [Parameter] public Action<int> OnDeleteField { get; set; }
        [Parameter] public Action OnAddField { get; set; }
        [Parameter] public int Index { get; set; }
        [Parameter] public bool ShowAddButton { get; set; }
        private int Type { get; set; }
        private int TypeVisualization { get; set; }
        List<SelectBarItemWrap<int>> DataType = new List<SelectBarItemWrap<int>>()
        {
            new SelectBarItemWrap<int>() {Key=1, Name="Numerico"},
            new SelectBarItemWrap<int>() {Key=2, Name="Decimal"},
            new SelectBarItemWrap<int>() {Key=2, Name="Fecha"},
            new SelectBarItemWrap<int>() {Key=3, Name="Texto"},
            new SelectBarItemWrap<int>() {Key=2, Name="Entidad"}
        };
        List<SelectBarItemWrap<int>> DataTypeVisualization = new List<SelectBarItemWrap<int>>()
        {
            new SelectBarItemWrap<int>() {Key=1, Name="Default"},
            new SelectBarItemWrap<int>() {Key=2, Name="Boton-group"}
        };

        public void DeleteField(){            
            if(OnDeleteField != null)
                OnDeleteField(Index);
        }

        public void AddField(){
            if(OnAddField != null)
                OnAddField();            
        }
    }

}