using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Siesa.SDK.Entities;
using Siesa.SDK.Shared.Business;
using Siesa.SDK.Shared.Results;

namespace Siesa.SDK.Business
{

    public interface IBLBase<T> where T: BaseEntity
    {
        string BusinessName { get; set; }
        T BaseObj { get; set; }
        T Get(int id);
        Task<T> GetAsync(int id);
        SaveSimpleOperationResult ValidateAndSave();
        void Update();
        int Delete();
        LoadResult List(int page, int pageSize, string options);
    }

}