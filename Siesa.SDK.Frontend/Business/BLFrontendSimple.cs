using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Siesa.SDK.Entities;

namespace Siesa.SDK.Business
{
    public class BLFrontendSimple<T> : IBLBase<T> where T : BaseEntity
    {
        public string BusinessName { get; set; }
        public T BaseObj { get; set; }
        public BLFrontendSimple()
        {
            BaseObj = Activator.CreateInstance<T>();
        }

        public virtual T Get(int id)
        {
            return GetAsync(id).GetAwaiter().GetResult();
        }

        public async virtual Task<T> GetAsync(int id)
        {
            var businness = Frontend.BusinessManager.Instance.GetBusiness(BusinessName);
            var message = await businness.Get(id);
            var result = JsonConvert.DeserializeObject<T>(message);
            return result;
        }

        public async virtual Task InitializeBusiness(int id)
        {
            BaseObj = await GetAsync(id);
        }

        public virtual void Save()
        {
            var businness = Frontend.BusinessManager.Instance.GetBusiness(BusinessName);
            businness.Save(this);
        }

        public virtual void Update()
        {
            throw new NotImplementedException();
        }

        public virtual void Delete()
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<T> List(int page, int pageSize)
        {
            throw new NotImplementedException();
        }
    }
}
