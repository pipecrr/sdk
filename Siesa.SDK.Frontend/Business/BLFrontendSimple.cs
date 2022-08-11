using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Siesa.SDK.Entities;
using Siesa.SDK.Frontend;
using Siesa.SDK.Frontend.Components.FormManager.Model;
using Siesa.SDK.Frontend.Components.FormManager.Model.Fields;
using Siesa.SDK.Frontend.Components.FormManager.ViewModels;
using Siesa.SDK.Protos;
using Siesa.SDK.Shared.Business;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Shared.Utilities;
using Siesa.SDK.Shared.Validators;

namespace Siesa.SDK.Business
{
    public class BLFrontendSimple : IBLBase<BaseSDK<int>>
    {
        [JsonIgnore]
        public dynamic ParentComponent {get;set;}

        public string BusinessName { get; set; }
        [JsonIgnore]
        public List<Panel> Panels = new List<Panel>();
        [JsonIgnore]
        public List<FieldOptions> ListViewFields = new List<FieldOptions>();
        public BaseSDK<int> BaseObj { get; set; }

        [JsonIgnore]
        protected IAuthenticationService AuthenticationService {get; set;}

        public BLFrontendSimple(IAuthenticationService authenticationService)
        {
            AuthenticationService = authenticationService;
        }

        public DeleteBusinessObjResponse Delete()
        {
            return new DeleteBusinessObjResponse();
        }

        public BaseSDK<int> Get(Int64 rowid)
        {
            return null;
        }

        public Task<BaseSDK<int>> GetAsync(Int64 rowid)
        {
            return null;
        }

        public Shared.Business.LoadResult GetData(int? skip, int? take, string filter = "", string orderBy = "", QueryFilterDelegate<BaseSDK<int>> queryFilter = null)
        {
            return null;
        }

        public void Update()
        {
        }

        public ValidateAndSaveBusinessObjResponse ValidateAndSave()
        {
            return null;
        }
    }

    
    public class BLFrontendSimple<T, K> : IBLBase<T> where T : class,IBaseSDK where K : BLBaseValidator<T>
    {
        [JsonIgnore]
        public dynamic ParentComponent {get;set;}
        public string BusinessName { get; set; }
        [JsonIgnore]
        public BusinessFrontendModel Backend {get { return Frontend.BusinessManagerFrontend.Instance.GetBusiness(BusinessName, AuthenticationService); } }

        public BusinessFrontendModel GetBackend(string business_name){
            return Frontend.BusinessManagerFrontend.Instance.GetBusiness(business_name, AuthenticationService);
        }
        [JsonIgnore]
        public List<Panel> Panels = new List<Panel>();
        [JsonIgnore]
        public List<FieldOptions> ListViewFields = new List<FieldOptions>();
        [ValidateComplexType]
        public T BaseObj { get; set; }

        public List<string> RelFieldsToSave {get;set;} = new List<string>();

        public async Task Refresh() {
            if(ParentComponent != null){
                try
                {
                    ParentComponent.Refresh();
                }
                catch (System.Exception)
                {
                }
            }
        }

        public void AddRelFieldToSave(string fieldName)
        {
            if (!RelFieldsToSave.Contains(fieldName))
            {
                RelFieldsToSave.Add(fieldName);
            }
        }

 
        [JsonIgnore]
        protected IAuthenticationService AuthenticationService {get; set;}

        public BLFrontendSimple(IAuthenticationService authenticationService)
        {
            AuthenticationService = authenticationService;
            BaseObj = Activator.CreateInstance<T>();
        }

        public virtual T Get(Int64 rowid)
        {
            return GetAsync(rowid).GetAwaiter().GetResult();
        }

        public async virtual Task<T> GetAsync(Int64 rowid)
        {
            var message = await Backend.Get(rowid);
            var result = JsonConvert.DeserializeObject<T>(message);
            return result;
        }

        public async virtual Task<Int64> SaveAsync()
        {
            var result = await Backend.Save(this);
            return result;
        }

        public async virtual Task InitializeBusiness(Int64 rowid)
        {
            BaseObj = await GetAsync(rowid);
        }

        public async virtual Task GetDuplicateInfo(Int64 rowid)
        {
            BaseObj = await GetAsync(rowid);
            //clear rowid
            BaseObj.SetRowid(0);
            var blankBaseObj = Activator.CreateInstance<T>();
            if(Utilities.IsAssignableToGenericType(BaseObj.GetType(), typeof(BaseAudit<>)))
            {
                foreach(var field in typeof(BaseAudit<>).GetProperties())
                {
                    //if field is nullable, set it to null
                    if(field.PropertyType.IsGenericType && field.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        BaseObj.GetType().GetProperty(field.Name).SetValue(BaseObj, null);
                    }else{
                        BaseObj.GetType().GetProperty(field.Name).SetValue(BaseObj, blankBaseObj.GetType().GetProperty(field.Name).GetValue(blankBaseObj));
                    }
                }
            }
        }


        public virtual Int64 Save()
        {
            return SaveAsync().GetAwaiter().GetResult();
        }

        public virtual ValidateAndSaveBusinessObjResponse ValidateAndSave()
        {
            return ValidateAndSaveAsync().GetAwaiter().GetResult();
        }

        public virtual void Update()
        {
            throw new NotImplementedException();
        }

        public virtual DeleteBusinessObjResponse Delete()
        {
            return DeleteAsync().GetAwaiter().GetResult();
        }

        public async virtual Task<DeleteBusinessObjResponse> DeleteAsync()
        {
            var result = await Backend.Delete(BaseObj.GetRowid());
            return result;
        }

        public override string ToString()
        {
            if (BaseObj == null)
            {
                return "";
            }
            return BaseObj.ToString();
        }

        public virtual Siesa.SDK.Shared.Business.LoadResult GetData(int? skip, int? take, string filter = "", string orderBy = "", QueryFilterDelegate<T> queryFilter = null)
        {
            return GetDataAsync(skip, take, filter, orderBy).GetAwaiter().GetResult();
        }

        public virtual Siesa.SDK.Shared.Business.LoadResult EntityFieldSearch(string searchText, string filters)
        {
            return EntityFieldSearchAsync(searchText, filters).GetAwaiter().GetResult();
        }

        public async virtual Task<Siesa.SDK.Shared.Business.LoadResult> EntityFieldSearchAsync(string searchText, string filters)
        {
            var result = await Backend.EntityFieldSearch(searchText, filters);
            Siesa.SDK.Shared.Business.LoadResult response = new Siesa.SDK.Shared.Business.LoadResult();
            response.Data = result.Data.Select(x => JsonConvert.DeserializeObject<T>(x)).ToList();
            response.TotalCount = result.TotalCount;
            response.GroupCount = result.GroupCount;
            return response;
        }

        public async virtual Task<Siesa.SDK.Shared.Business.LoadResult> GetDataAsync(int? skip, int? take, string filter = "", string orderBy = "")
        {
            var result = await Backend.GetData(skip, take, filter, orderBy);
            Siesa.SDK.Shared.Business.LoadResult response = new Siesa.SDK.Shared.Business.LoadResult();
            response.Data = result.Data.Select(x => JsonConvert.DeserializeObject<T>(x)).ToList();
            response.TotalCount = result.TotalCount;
            response.GroupCount = result.GroupCount;
            return response;
        }

        public async virtual Task<ValidateAndSaveBusinessObjResponse> ValidateAndSaveAsync()
        {
            ValidateAndSaveBusinessObjResponse resultValidationFront = new();
            Validate(ref resultValidationFront);
            if (resultValidationFront.Errors.Count > 0)
            {
                return resultValidationFront;
            }
            var result = await Backend.ValidateAndSave(this);
            return result;
        }

        private void Validate(ref ValidateAndSaveBusinessObjResponse baseOperation)
        {
            ValidateBussines(ref baseOperation, BaseObj.GetRowid() == 0 ? BLUserActionEnum.Create : BLUserActionEnum.Update);
            K validator = Activator.CreateInstance<K>();
            validator.ValidatorType = "Entity";
            SDKValidator.Validate<T>(BaseObj, validator, ref baseOperation);
        }

        protected virtual void ValidateBussines(ref ValidateAndSaveBusinessObjResponse operationResult, BLUserActionEnum action)
        {
            // Do nothing
        }

        public virtual void OnReady(DynamicViewType viewType, long rowid = 0)
        {
            // Do nothing
        }

        public void SetPropertyValue(string propertyName, object value)
        {
            string[] fieldPath = propertyName.Split('.');
            object currentObject = this;
            for (int i = 0; i < fieldPath.Length - 1; i++)
            {
                var tmpType = currentObject.GetType();
                var tmpProperty = tmpType.GetProperty(fieldPath[i]);
                var tmpValue = tmpProperty.GetValue(currentObject, null);
                var isEntity = tmpProperty.PropertyType.IsSubclassOf(typeof(BaseSDK<>));
                if (tmpValue == null && isEntity)
                {
                    tmpValue = Activator.CreateInstance(tmpProperty.PropertyType);
                    tmpProperty.SetValue(currentObject, tmpValue);
                }
                currentObject = tmpValue;
            }
            if(currentObject != null)
            {
                var property = currentObject.GetType().GetProperty(fieldPath.Last());
                if (property != null)
                {
                    property.SetValue(currentObject, value);
                }
            }
        }

    }
}
