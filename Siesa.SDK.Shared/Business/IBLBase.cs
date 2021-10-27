using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Siesa.SDK.Entities;

namespace Siesa.SDK.Business
{
    public interface IBLBase<T> where T: BaseEntity
    {
        string BusinessName { get; set; }
        T BaseObj { get; set; }
        T Get(int id);
        Task<T> GetAsync(int id);
        void Save();
        void Update();
        void Delete();
        IEnumerable<T> List(int page, int pageSize);

    }

}