using System;
using System.Collections.Generic;
using Siesa.SDK.Entities;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Shared.Validators;
namespace Siesa.SDK.Business
{
    public class BLFrontendDocment<TParent, TChild> : BLFrontendSimple<TParent,BLBaseValidator<TParent>> where TParent : class,IBaseSDK
    {
        public BLFrontendDocment(IAuthenticationService authenticationService) : base(authenticationService)
        {

        }

        public List<TChild> ChildObj { get; set; } = new();

        public Type GetTypeChild()
        {
            return typeof(TChild);
        }
    }
}
