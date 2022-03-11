using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Siesa.SDK.Entities;
using Siesa.SDK.Protos;
using Siesa.SDK.Shared.Business;

namespace Siesa.SDK.Shared.Business
{
    public delegate IQueryable<T> QueryFilterDelegate<T>(IQueryable<T> query) where T : BaseEntity;

    public interface IBLBase<T> where T: BaseEntity
    {
        string BusinessName { get; set; }
        T BaseObj { get; set; }
        T Get(int id);
        Task<T> GetAsync(int id);
        ValidateAndSaveBusinessObjResponse ValidateAndSave();
        void Update();
        int Delete();
        Siesa.SDK.Shared.Business.LoadResult GetData(int? skip, int? take, string filter, string orderBy, QueryFilterDelegate<T> queryFilter);
    }

}