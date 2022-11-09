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
            //var pbr = context.Set<T>().Select(x => x.GetType().GetProperty("Rowid").GetValue(x,null)).AsQueryable();
            //var nextRowid = query.OrderBy(x =>    x.GetRowid()).Select(x => x.GetRowid()).FirstOrDefault();
            //.Where(x => (long)x > Rowid).ToList().FirstOrDefault();
            var result = this.GetData(0,1,$"Rowid > {Rowid}",null,null); 
            if (result is not null)
            {
                long nextRowid = result.Data.First().Rowid;
                return new ActionResult<long>() {Success=true, Data = nextRowid};
            }
            else
            {
                return new BadRequestResult<long> { Success = false, Errors = new List<string>() { "Rowid Not Found" } };
            }  
        }

        [SDKExposedMethod]
        public ActionResult<long> GetPrevioustRowId(long Rowid)
        {
           //var result = this.GetData(null,null,filter:$"Rowid < {Rowid}" ,orderBy:"Rowid desc",null); 
           var result = this.GetData(0,1,$"Rowid < {Rowid}","Rowid desc",null);            
            if (result is not null)
            {
                //long nextRowid = result.Data.OrderByDescending(x=> x.Rowid).First().Rowid;
                long nextRowid = result.Data.LastOrDefault().Rowid;
                return new ActionResult<long>() {Success=true, Data = nextRowid};
            }
            else
            {
                return new BadRequestResult<long> { Success = false, Errors = new List<string>() { "Rowid Not Found" } };
            }  
        }

        [SDKExposedMethod]
        public ActionResult<long> GetFirstRowid(long Rowid)
        {
            var result = this.GetData(0,1,$"Rowid < {Rowid}",null,null); 

            if (result is not null)
            {
                long nextRowid = result.Data.First().Rowid;
                return new ActionResult<long>() {Success=true, Data = nextRowid};
            }
            else
            {
                return new BadRequestResult<long> { Success = false, Errors = new List<string>() { "Rowid Not Found" } };
            } 
        }

        [SDKExposedMethod]
        public ActionResult<long> GetLastRowid(long Rowid)
        {
            var result = this.GetData(0,1,$"Rowid > {Rowid}","Rowid desc",null); 

            if (result is not null)
            {
                long nextRowid = result.Data.First().Rowid;
                return new ActionResult<long>() {Success=true, Data = nextRowid};
            }
            else
            {
                return new BadRequestResult<long> { Success = false, Errors = new List<string>() { "Rowid Not Found" } };
            }  

        }
    }
}
