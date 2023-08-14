using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public List<TChild> ChildObjs { get; set; } = new();
        public List<TChild> ChildObjsDeleted { get; set; } = new();
        public List<dynamic> ChildRowidsUpdated { get; set; } = new();
        public List<string> ExtrDetailFields { get; set; } = new();

        public Type GetTypeChild()
        {
            return typeof(TChild);
        }        

        public async Task InitializeChilds()
        {
            var response = await Backend.Call("GetChilds",BaseObj.GetRowid(), ExtrDetailFields).ConfigureAwait(true);
            if (response.Success)
            {
                ChildObjs = response.Data;
            }
        }

        public void SdkOnChangeCell(dynamic row)
        {

            if (row.Rowid > 0 && !ChildRowidsUpdated.Contains(row.Rowid))
            {
                ChildRowidsUpdated.Add(row.Rowid);
            }
        }
    }
}
