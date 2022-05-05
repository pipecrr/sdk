﻿using System;
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
using Siesa.SDK.Protos;
using Siesa.SDK.Shared.Business;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Shared.Validators;

namespace Siesa.SDK.Business
{
    public class BLFrontendSimple : IBLBase<BaseEntity>
    {
        public string BusinessName { get; set; }
        [JsonIgnore]
        public List<Panel> Panels = new List<Panel>();
        public BaseEntity BaseObj { get; set; }

        [JsonIgnore]
        private IAuthenticationService AuthenticationService {get; set;}

        public BLFrontendSimple(IAuthenticationService authenticationService)
        {
            AuthenticationService = authenticationService;
        }

        public DeleteBusinessObjResponse Delete()
        {
            return new DeleteBusinessObjResponse();
        }

        public BaseEntity Get(int id)
        {
            return null;
        }

        public Task<BaseEntity> GetAsync(int id)
        {
            return null;
        }

        public Shared.Business.LoadResult GetData(int? skip, int? take, string filter = "", string orderBy = "", QueryFilterDelegate<BaseEntity> queryFilter = null)
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

    
    public class BLFrontendSimple<T, K> : IBLBase<T> where T : BaseEntity where K : BLBaseValidator<T>
    {
        public string BusinessName { get; set; }
        [JsonIgnore]
        public BusinessFrontendModel Backend {get { return Frontend.BusinessManagerFrontend.Instance.GetBusiness(BusinessName, AuthenticationService); } }

        public BusinessFrontendModel GetBackend(string business_name){
            return Frontend.BusinessManagerFrontend.Instance.GetBusiness(business_name, AuthenticationService);
        }
        [JsonIgnore]
        public List<Panel> Panels = new List<Panel>();
        [ValidateComplexType]
        public T BaseObj { get; set; }

        public List<string> RelFieldsToSave {get;set;} = new List<string>();

        public void AddRelFieldToSave(string fieldName)
        {
            if (!RelFieldsToSave.Contains(fieldName))
            {
                RelFieldsToSave.Add(fieldName);
            }
        }

 
        [JsonIgnore]
        private IAuthenticationService AuthenticationService {get; set;}

        public BLFrontendSimple(IAuthenticationService authenticationService)
        {
            AuthenticationService = authenticationService;
            BaseObj = Activator.CreateInstance<T>();
        }

        public virtual T Get(int id)
        {
            return GetAsync(id).GetAwaiter().GetResult();
        }

        public async virtual Task<T> GetAsync(int id)
        {
            var message = await Backend.Get(id);
            var result = JsonConvert.DeserializeObject<T>(message);
            return result;
        }

        public async virtual Task<int> SaveAsync()
        {
            var result = await Backend.Save(this);
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
            var result = await Backend.Delete(BaseObj.Rowid);
            return result;
        }

        public override string ToString()
        {
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
            ValidateBussines(ref baseOperation, BaseObj.Rowid == 0 ? BLUserActionEnum.Create : BLUserActionEnum.Update);
            K validator = Activator.CreateInstance<K>();
            validator.ValidatorType = "Entity";
            SDKValidator.Validate<T>(BaseObj, validator, ref baseOperation);
        }

        protected virtual void ValidateBussines(ref ValidateAndSaveBusinessObjResponse operationResult, BLUserActionEnum action)
        {
            // Do nothing
        }

        public virtual void OnReady(){

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
                var isEntity = tmpProperty.PropertyType.IsSubclassOf(typeof(BaseEntity));
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
