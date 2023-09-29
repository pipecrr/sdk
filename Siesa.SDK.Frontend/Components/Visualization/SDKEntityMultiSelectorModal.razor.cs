
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Microsoft.AspNetCore.Components;
using Siesa.Global.Enums;
using Siesa.SDK.Entities;
using Siesa.SDK.Frontend.Components.Fields;
using Siesa.SDK.Frontend.Services;

namespace Siesa.SDK.Frontend.Components.Visualization
{
    public partial class SDKEntityMultiSelectorModal : ComponentBase
    {
        [Parameter] public dynamic Business {get; set;}
        [Parameter] public List<int> ItemsSelected {get; set;}
        [Parameter] public List<int> RowidRecordsRelated {get; set;}
        [Parameter] public SDKEntityMultiSelector SDKManyToManySelectorRef {get; set;}
        [Parameter] public Action<List<int>> OnAddAction {get; set;}
        [Parameter] public string ResourceTagButtonAdd {get; set;} = "Custom.SDKEntityMultiSelectorModal.AddItems";
        [Inject] public SDKNotificationService NotificationService {get; set;}
        private List<string> ConstantFilters { get; set; }

        protected override void OnInitialized()
        {
            ConstantFilters = new()
            {
                $"Status = {(int) enumStatusBaseMaster.Active}"
            };
            var notInFilter = RowidRecordsRelated.Select(x => $"Rowid != {x}");
            ConstantFilters.AddRange(notInFilter);
            ItemsSelected = new();
            
            base.OnInitialized();
        }

        private void OnSelectItems(IList<dynamic> items)
        {
            ItemsSelected = items.Select(x => (int) x.GetType().GetProperty("Rowid").GetValue(x)).ToList();
        }

        private void AddUsers()
        {
            if(!ItemsSelected.Any())
            {
                _ = NotificationService.ShowInfo("Custom.UserComponentModal.SelectUserMessage");
                return;
            }

            if (OnAddAction != null)
            {
                OnAddAction(ItemsSelected);
            }
            RowidRecordsRelated.AddRange(ItemsSelected);
            if(SDKManyToManySelectorRef != null)
                SDKManyToManySelectorRef.CloseModal();
        }
    }
}