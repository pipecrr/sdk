using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Siesa.SDK.Entities;
using Siesa.SDK.Protos;
using Siesa.SDK.Shared.Business;

namespace Siesa.SDK.Shared.Business
{
    public delegate IQueryable<T> QueryFilterDelegate<T>(IQueryable<T> query) where T: IBaseSDK;

    public interface IBLBase<T> where T: IBaseSDK
    {
        string BusinessName { get; set; }
        T BaseObj { get; set; }
        T Get(Int64 rowid, List<string> extraFields = null);
        Task<T> GetAsync(Int64 rowid, List<string> extraFields = null);
        ValidateAndSaveBusinessObjResponse ValidateAndSave(bool ignorePermissions = false);
        void Update();
        DeleteBusinessObjResponse Delete();
        Siesa.SDK.Shared.Business.LoadResult GetData(int? skip, int? take, string filter, string orderBy, QueryFilterDelegate<T> queryFilter, bool includeCount = false);
    }

}