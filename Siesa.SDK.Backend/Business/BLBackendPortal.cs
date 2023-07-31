using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Siesa.SDK.Backend.Access;
using Siesa.SDK.Entities;
using Siesa.SDK.Protos;
using Siesa.SDK.Shared.Business;
using Siesa.SDK.Shared.DataAnnotations;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Shared.Validators;
using System.Linq.Dynamic.Core;

namespace Siesa.SDK.Business
{
    public class BLBackendPortal<T> : BLBackendSimple<T,BLBaseValidator<T>> where T : class,IBaseSDK
    {
        public BLBackendPortal(IAuthenticationService authenticationService) : base(authenticationService)
        {
        }
    }
}
