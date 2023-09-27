using System;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using Siesa.SDK.Entities;
using Microsoft.Extensions.DependencyInjection;
using Siesa.SDK.Shared.Services;
using System.Threading.Tasks;
namespace Siesa.SDK.Frontend.Components.Layout.AditionalField
{
    public partial class GroupAditionalFields{
        [Inject] public IResourceManager ResourceManager {get; set;}
        [Inject] public IAuthenticationService AuthenticationService { get; set; }
        [Parameter] public E00250_DynamicEntity DynamicEntity { get; set; } = new E00250_DynamicEntity();
        [Parameter] public int SizeField { get; set; }
        [Parameter] public int Type { get; set; }
        [Parameter] public bool EnableButtonNext { get; set; }
        [Parameter] public Action OnChangeTag { get; set; }
        List<SelectBarItemWrap<int>> DataVisualizationType = new List<SelectBarItemWrap<int>>();        

        protected override async Task OnInitializedAsync()
        {
            var TextFormulario = await ResourceManager.GetResource("Custom.Group.AditionalFields.VisualizationType.RadioForm", AuthenticationService);
            var TextTable = await ResourceManager.GetResource("Custom.Group.AditionalFields.VisualizationType.RadioTable", AuthenticationService);

            DataVisualizationType.Add(new SelectBarItemWrap<int>() {Key=1, Name=TextFormulario});
            DataVisualizationType.Add(new SelectBarItemWrap<int>() {Key=2, Name=TextTable});

            await base.OnInitializedAsync();
        }

        private string _OnChangeTag(string value){
            DynamicEntity.Tag = value;
            if(string.IsNullOrEmpty(DynamicEntity.Tag)){
                EnableButtonNext = false;
            }else{
                EnableButtonNext = true;
            }
            if(OnChangeTag != null){
                OnChangeTag();
            }

            return value;
        }
    }

}