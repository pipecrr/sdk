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
                var nextRowid = context.Set<T>().AsEnumerable().Select(x => x.GetType().GetProperty("Rowid").GetValue(x,null))
                                    .Where(x => Convert.ToInt64(x) > Rowid).FirstOrDefault();

                if (nextRowid is not null)
                {
                    long nextRowidValue = Convert.ToInt64(nextRowid);
                    return new ActionResult<long>() {Success=true, Data = nextRowidValue};
                }
                else
                {
                    return new BadRequestResult<long> { Success = false, Errors = new List<string>() { "Rowid Not Found" } };
                }  
            }
            /*var result = this.GetData(0,1,$"Rowid > {Rowid}",null,null); 
                if (nextRowid is not null)
                {
                    long nextRowid = result.Data.First().Rowid;
                    return new ActionResult<long>() {Success=true, Data = nextRowid};
                }
                else
                {
                    return new BadRequestResult<long> { Success = false, Errors = new List<string>() { "Rowid Not Found" } };
            }*/ 
        }

        [SDKExposedMethod]
        public ActionResult<long> GetPrevioustRowId(long Rowid)
        {
             using (SDKContext context = CreateDbContext())
            {
                var nextRowid = context.Set<T>().AsEnumerable().Select(x => x.GetType().GetProperty("Rowid").GetValue(x,null))
                                        .Where(x => Convert.ToInt64(x) < Rowid).OrderByDescending(x => x).FirstOrDefault();

                if (nextRowid is not null)
                {
                    long nextRowidValue = Convert.ToInt64(nextRowid);
                    return new ActionResult<long>() {Success=true, Data = nextRowidValue};
                }
                else
                {
                    return new BadRequestResult<long> { Success = false, Errors = new List<string>() { "Rowid Not Found" } };
                }
            }  

        //    var result = this.GetData(0,1,$"Rowid < {Rowid}","Rowid desc",null);            
        //     if (result is not null)
        //     {
        //         long nextRowid = result.Data.LastOrDefault().Rowid;
        //         return new ActionResult<long>() {Success=true, Data = nextRowid};
        //     }
        //     else
        //     {
        //         return new BadRequestResult<long> { Success = false, Errors = new List<string>() { "Rowid Not Found" } };
        //     }  
        }

        [SDKExposedMethod]
        public ActionResult<long> GetFirstRowid(long Rowid)
        {
            using (SDKContext context = CreateDbContext())
            {
                var nextRowid = context.Set<T>().AsEnumerable().Select(x => x.GetType().GetProperty("Rowid").GetValue(x,null))
                                        .Where(x => Convert.ToInt64(x) < Rowid).FirstOrDefault();

                if (nextRowid is not null)
                {
                    long nextRowidValue = Convert.ToInt64(nextRowid);
                    return new ActionResult<long>() {Success=true, Data = nextRowidValue};
                }
                else
                {
                    return new BadRequestResult<long> { Success = false, Errors = new List<string>() { "Rowid Not Found" } };
                }
            }  

            // var result = this.GetData(0,1,$"Rowid < {Rowid}",null,null); 

            // if (result is not null)
            // {
            //     long nextRowid = result.Data.First().Rowid;
            //     return new ActionResult<long>() {Success=true, Data = nextRowid};
            // }
            // else
            // {
            //     return new BadRequestResult<long> { Success = false, Errors = new List<string>() { "Rowid Not Found" } };
            // } 
        }

        [SDKExposedMethod]
        public ActionResult<long> GetLastRowid(long Rowid)
        {
            using (SDKContext context = CreateDbContext())
            {
                var nextRowid = context.Set<T>().AsEnumerable().Select(x => x.GetType().GetProperty("Rowid").GetValue(x,null))
                                        .OrderByDescending(x => x).FirstOrDefault();

                if (nextRowid is not null)
                {
                    long nextRowidValue = Convert.ToInt64(nextRowid);
                    return new ActionResult<long>() {Success=true, Data = nextRowidValue};
                }
                else
                {
                    return new BadRequestResult<long> { Success = false, Errors = new List<string>() { "Rowid Not Found" } };
                }
            }  
            // var result = this.GetData(0,1,$"Rowid > {Rowid}","Rowid desc",null); 

            // if (result is not null)
            // {
            //     long nextRowid = result.Data.First().Rowid;
            //     return new ActionResult<long>() {Success=true, Data = nextRowid};
            // }
            // else
            // {
            //     return new BadRequestResult<long> { Success = false, Errors = new List<string>() { "Rowid Not Found" } };
            // }  

        }
    }
}
