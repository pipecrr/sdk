using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Siesa.SDK.Entities;
using Siesa.SDK.Frontend.Components.FormManager.Model;
using Siesa.SDK.Shared.Business;

namespace Siesa.SDK.Business
{
    public class BLFrontendSimple<T> : IBLBase<T> where T : BaseEntity
    {
        public string BusinessName { get; set; }
        [JsonIgnore]
        public List<Panel> Paneles = new List<Panel>();
        [ValidateComplexType]
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

        public async virtual Task<int> SaveAsync()
        {
            var businness = Frontend.BusinessManager.Instance.GetBusiness(BusinessName);
            var result = await businness.Save(this);
            return result;
        }

        public async virtual Task InitializeBusiness(int id)
        {
            BaseObj = await GetAsync(id);
        }

        public virtual int Save()
        {
            return SaveAsync().GetAwaiter().GetResult();
        }

        public virtual void Update()
        {
            throw new NotImplementedException();
        }

        public virtual int Delete()
        {
            return DeleteAsync().GetAwaiter().GetResult();
        }

        public async virtual Task<int> DeleteAsync()
        {
            var businness = Frontend.BusinessManager.Instance.GetBusiness(BusinessName);
            var result = await businness.Delete(BaseObj.RowID);
            return result;
        }

        public override string ToString()
        {
            return BaseObj.ToString();
        }

        public virtual LoadResult List(int page = 0, int pageSize = 30, string options = "")
        {
            return ListAsync(page, pageSize, options).GetAwaiter().GetResult();
        }

        public async virtual Task<LoadResult> ListAsync(int page = 0, int pageSize = 30, string options = "")
        {
            var businness = Frontend.BusinessManager.Instance.GetBusiness(BusinessName);
            var result = await businness.List(page, pageSize, options);
            LoadResult response = new LoadResult();
            response.Data = result.Data.Select(x => JsonConvert.DeserializeObject<T>(x)).ToList();
            response.TotalCount = result.TotalCount;
            response.GroupCount = result.GroupCount;
            return response;
        }
    }
}
