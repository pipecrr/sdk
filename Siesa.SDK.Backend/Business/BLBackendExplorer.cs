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

namespace Siesa.SDK.Business
{
    public class BLBackendExplorer<T> : BLBackendSimple<T,BLBaseValidator<T>> where T : class,IBaseSDK
    {
        public BLBackendExplorer(IAuthenticationService authenticationService) : base(authenticationService)
        {
        }

        [SDKExposedMethod]
        public ActionResult<long> GetNextRowId(long Rowid)
        {
            using (SDKContext context = CreateDbContext())
            {

                var nextRowid = context.Set<T>().ToList().FirstOrDefault(x => x.GetRowid() > Rowid);

                if (nextRowid != null)
                {
                    return new ActionResult<long>() {Success=true, Data = nextRowid.GetRowid()};
                }
                else
                {
                    return new BadRequestResult<long> { Success = false, Errors = new List<string>() { "Rowid Not Found" } };
                }  
            }

        }

        [SDKExposedMethod]
        public ActionResult<long> GetPrevioustRowId(long Rowid)
        {
            using (SDKContext context = CreateDbContext())
            {

                var nextRowid = context.Set<T>().ToList().Where(x => x.GetRowid() < Rowid).OrderByDescending(x => x.GetRowid()).FirstOrDefault();

                if (nextRowid != null)
                {
                    return new ActionResult<long>() {Success=true, Data = nextRowid.GetRowid()};
                }
                else
                {
                    return new BadRequestResult<long> { Success = false, Errors = new List<string>() { "Rowid Not Found" } };
                }  
            }

        }

        [SDKExposedMethod]
        public ActionResult<long> GetFirstRowid(long Rowid)
        {
            using (SDKContext context = CreateDbContext())
            {

                var nextRowid = context.Set<T>().ToList().FirstOrDefault(x => x.GetRowid() < Rowid);

                if (nextRowid != null)
                {
                    return new ActionResult<long>() {Success=true, Data = nextRowid.GetRowid()};
                }
                else
                {
                    return new BadRequestResult<long> { Success = false, Errors = new List<string>() { "Rowid Not Found" } };
                }  
            }
        }

        [SDKExposedMethod]
        public ActionResult<long> GetLastRowid(long Rowid)
        {
            using (SDKContext context = CreateDbContext())
            {

                var nextRowid = context.Set<T>().ToList().OrderByDescending(x => x.GetRowid()).FirstOrDefault();

                if (nextRowid != null)
                {
                    return new ActionResult<long>() {Success=true, Data = nextRowid.GetRowid()};
                }
                else
                {
                    return new BadRequestResult<long> { Success = false, Errors = new List<string>() { "Rowid Not Found" } };
                }  
            }

        }
    }
}
