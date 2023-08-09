using System;
using System.Collections.Generic;
using Siesa.SDK.Entities;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Shared.Validators;

namespace Siesa.SDK.Business
{
    public class BLBackendDocment<TParent, TChild> : BLBackendSimple<TParent,BLBaseValidator<TParent>> where TParent : class,IBaseSDK
    {
        public BLBackendDocment(IAuthenticationService authenticationService) : base(authenticationService)
        {
            
        }
        public List<TChild> RelatedBaseObjects { get; set; } = new ();
        public Type GetTypeRelated()
        {
            return typeof(TChild);
        }
    }
}
