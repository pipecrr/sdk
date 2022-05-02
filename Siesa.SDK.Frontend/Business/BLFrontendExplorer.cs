using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Siesa.SDK.Entities;
using Siesa.SDK.Frontend;
using Siesa.SDK.Frontend.Components.FormManager.Model;
using Siesa.SDK.Protos;
using Siesa.SDK.Shared.Business;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Shared.Validators;
using Siesa.SDK.Frontend.Components.FormManager;
using Siesa.SDK.Frontend.Components.FormManager.Views;

namespace Siesa.SDK.Business
{
    public class BLFrontendExplorer<T> : BLFrontendSimple<T,BLBaseValidator<T>> where T : class,IBaseSDK
    {
        public BLFrontendExplorer(IAuthenticationService authenticationService) : base(authenticationService)
        {
        }

        public async Task GetNextRowid()
        {
            if(BaseObj != null){
                var currentRowid = BaseObj.GetRowid();
                var nextRowid = currentRowid + 1;
                await InitializeBusiness(nextRowid);
            }
        }

        public async Task GetPreviousRowid()
        {
            if(BaseObj != null && BaseObj.GetRowid() > 1){
                var currentRowid = BaseObj.GetRowid();
                var previousRowid = currentRowid - 1;
                await InitializeBusiness(previousRowid);
            }
        }

        public async Task GetFirstRowid()
        {
            if(BaseObj != null){
                var firstRowid = 1;
                await InitializeBusiness(firstRowid);
            }
        }

        public async Task GetLastRowid()
        {
            // if(BaseObj != null){
            //     var lastRowid = await GetLastRowidAsync();
            //     await InitializeBusiness(lastRowid);
            // }
        }

        public RenderFragment explorer()
        {
            return (builder) =>
            {
                builder.OpenComponent<ExplorerView>(0);
                builder.AddAttribute(1, "Viewdef", "explorer");
                builder.AddAttribute(2, "BusinessObj", this);
                builder.AddAttribute(3, "BusinessName", this.BusinessName);
                builder.AddAttribute(4, "Title", "Explorador");
                builder.CloseComponent();
            };
        }
    }
}
