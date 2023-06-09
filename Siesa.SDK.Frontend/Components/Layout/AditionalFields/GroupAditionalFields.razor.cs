using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using Siesa.SDK.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Siesa.SDK.Frontend.Components.Layout.AditionalFields
{
    public partial class GroupAditionalFields{
        [Parameter] public E00250_DynamicEntity DynamicEntity { get; set; } = new E00250_DynamicEntity();
        [Parameter] public int SizeField { get; set; }
        private int Type { get; set; }
        List<SelectBarItemWrap<int>> DataVisualizationType = new List<SelectBarItemWrap<int>>()
        {
            new SelectBarItemWrap<int>() {Key=1, Name="Formulario"},
            new SelectBarItemWrap<int>() {Key=2, Name="Tabla"}
        };
    }

}