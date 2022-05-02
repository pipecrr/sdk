using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Siesa.SDK.Entities;
using Siesa.SDK.Protos;
using Siesa.SDK.Shared.Business;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Shared.Validators;

namespace Siesa.SDK.Business
{
    public class BLBackendExplorer<T> : BLBackendSimple<T,BLBaseValidator<T>> where T : class,IBaseSDK
    {
        public BLBackendExplorer(IAuthenticationService authenticationService) : base(authenticationService)
        {
        }
    }
}
