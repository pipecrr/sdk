using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Siesa.SDK.Entities;
using Siesa.SDK.Protos;
using Siesa.SDK.Shared.Business;

namespace Siesa.SDK.Business
{

    public interface IBLBase<T> where T: BaseEntity
    {
        string BusinessName { get; set; }
        T BaseObj { get; set; }
        T Get(int id);
        Task<T> GetAsync(int id);
        ValidateAndSaveBusinessObjResponse ValidateAndSave();
        void Update();
        int Delete();
        Siesa.SDK.Shared.Business.LoadResult List(int page, int pageSize, string options);
    }

}