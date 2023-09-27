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
using Siesa.SDK.Shared.DTOS;
namespace Siesa.SDK.Business
{
    public class BLFrontendPortal<T> : BLFrontendSimple<T,BLBaseValidator<T>> where T : class,IBaseSDK
    {
        public BLFrontendPortal(IAuthenticationService authenticationService) : base(authenticationService)
        {
        }

        public virtual SDKPortalConfiguration GetPortalConfiguration()
        {
            throw new NotImplementedException();
        }
    }
}
