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
using Siesa.SDK.Frontend.Services;

namespace Siesa.SDK.Business
{
    public class BLFrontendExplorer<T> : BLFrontendSimple<T,BLBaseValidator<T>> where T : class,IBaseSDK
    {
        public BLFrontendExplorer(IAuthenticationService authenticationService) : base(authenticationService)
        {
        }

        public async Task GetNextRowid()
        {
            if(BaseObj != null)
            {
                try
                {
                    var currentRowid = BaseObj.GetRowid();
                    var response = await Backend.Call("GetNextRowId", currentRowid);
                    
                    if (response.Success)
                    {
                            long nextRowid = response.Data;
                            await InitializeBusiness((int)nextRowid);
                    }
                    else
                    {
                        return ;
                    }
                }
                catch (Exception e)
                {
                }
                
            }
        }

        public async Task GetPreviousRowid()
        {
            if(BaseObj != null){
                try
                {
                    var currentRowid = BaseObj.GetRowid();
                    var response = await Backend.Call("GetPrevioustRowId", currentRowid);
                    
                    if (response.Success)
                    {
                            long nextRowid = response.Data;
                            await InitializeBusiness((int)nextRowid);
                    }
                    else
                    {
                        return ;
                    }
                }
                catch (Exception e)
                {
                }
              
            }
        }

        public async Task GetFirstRowid()
        {
            if(BaseObj != null){
                try
                {
                    var currentRowid = BaseObj.GetRowid();
                    var response = await Backend.Call("GetFirstRowid", currentRowid);
                    
                    if (response.Success)
                    {
                            long nextRowid = response.Data;
                            await InitializeBusiness((int)nextRowid);
                    }
                    else
                    {
                        return ;
                    }
                }
                catch (Exception e)
                {
                }
            }
        }

        public async Task GetLastRowid()
        {
            if(BaseObj != null){
                try
                {
                    var currentRowid = BaseObj.GetRowid();
                    var response = await Backend.Call("GetLastRowid", currentRowid);
                    
                    if (response.Success)
                    {
                            long nextRowid = response.Data;
                            await InitializeBusiness((int)nextRowid);
                    }
                    else
                    {
                        return ;
                    }
                }
                catch (Exception e)
                {
                }
            }
        }

        public RenderFragment explorer()
        {
            return (builder) =>
            {
                builder.OpenComponent<ExplorerView>(0);
                builder.AddAttribute(1, "Viewdef", "explorer");
                builder.AddAttribute(2, "BusinessObj", this);
                builder.AddAttribute(3, "BusinessName", this.BusinessName);
                builder.AddAttribute(4, "Title", $"{BusinessName}.Name");
                builder.CloseComponent();
            };
        }
    }
}
