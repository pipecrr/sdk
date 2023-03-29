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
using Siesa.SDK.Frontend.Services;
using Siesa.SDK.Protos;
using Siesa.SDK.Shared.Business;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Shared.Utilities;
using Siesa.SDK.Shared.Validators;
using Microsoft.Extensions.Logging;
using Siesa.SDK.Shared.DataAnnotations;
using Siesa.SDK.Shared.DTOS;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.Extensions.DependencyInjection;

namespace Siesa.SDK.Business
{
    public class BLFrontendSimple : IBLBase<BaseSDK<int>>
    {
        [JsonIgnore]
        public dynamic ParentComponent {get;set;}
        
        public string BLParentBusinessName {get;set;}

        public string BusinessName { get; set; }
        [JsonIgnore]
        public SDKBusinessModel Backend {get { return BackendRouterService.Instance.GetSDKBusinessModel(BusinessName, AuthenticationService); } }
        public SDKBusinessModel GetBackend(string business_name){
            return BackendRouterService.Instance.GetSDKBusinessModel(business_name, AuthenticationService);
        }
        [JsonIgnore]
        public List<Panel> Panels = new List<Panel>();
        [JsonIgnore]
        public List<FieldOptions> ListViewFields = new List<FieldOptions>();
        public BaseSDK<int> BaseObj { get; set; }

        [JsonIgnore]
        protected IAuthenticationService AuthenticationService { get; set; }

        [JsonIgnore]
        protected SDKNotificationService NotificationService { get; set; }

        private void InternalConstructor(IAuthenticationService authenticationService, SDKNotificationService notificationService)
        {
            AuthenticationService = authenticationService;
            NotificationService = notificationService;
        }

        public BLFrontendSimple(IAuthenticationService authenticationService, SDKNotificationService notificationService = null)
        {
            InternalConstructor(authenticationService, notificationService);
        }

        public BLFrontendSimple(IServiceProvider provider)
        {
            IAuthenticationService authService = (IAuthenticationService)provider.GetService(typeof(IAuthenticationService));
            SDKNotificationService notiService = (SDKNotificationService)provider.GetService(typeof(SDKNotificationService));

            InternalConstructor(authService, notiService);
        }

        public virtual void OnReady(DynamicViewType viewType, long rowid = 0)
        {
            // Do nothing
        }

        public DeleteBusinessObjResponse Delete()
        {
            return new DeleteBusinessObjResponse();
        }

        public BaseSDK<int> Get(Int64 rowid, List<string> extraFields = null)
        {
            return null;
        }

        public Task<BaseSDK<int>> GetAsync(Int64 rowid, List<string> extraFields = null)
        {
            return null;
        }

        public Shared.Business.LoadResult GetData(int? skip, int? take, string filter = "", string orderBy = "", QueryFilterDelegate<BaseSDK<int>> queryFilter = null, bool includeCount = false)
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


    public class BLFrontendSimple<T, K> : IBLBase<T> where T : class, IBaseSDK where K : BLBaseValidator<T>
    {
        [JsonIgnore]
        public dynamic ParentComponent {get;set;}
        public string BusinessName { get; set; }
        [JsonIgnore]
        public SDKBusinessModel Backend {get { return BackendRouterService.Instance.GetSDKBusinessModel(BusinessName, AuthenticationService); } }

        public SDKBusinessModel GetBackend(string business_name){
            return BackendRouterService.Instance.GetSDKBusinessModel(business_name, AuthenticationService);
        }
        [JsonIgnore]
        public List<Panel> Panels = new List<Panel>();
        [JsonIgnore]
        public List<FieldOptions> ListViewFields = new List<FieldOptions>();
        [ValidateComplexType]
        public T BaseObj { get; set; }

        public List<string> RelFieldsToSave { get; set; } = new List<string>();

        public async Task Refresh(bool Reload = false) {

            if(ParentComponent != null){
                try
                {
                    if (Reload)
                    {
                        ParentComponent.Refresh(Reload);
                    }else
                    {
                        ParentComponent.Refresh();
                    }
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
        protected IAuthenticationService AuthenticationService { get; set; }

        [JsonIgnore]
        protected SDKNotificationService NotificationService { get; set; }

        [JsonIgnore]
        public ILogger Logger { get; set; }

        private void InternalConstructor(IAuthenticationService authenticationService, SDKNotificationService notificationService, ILoggerFactory loggerFactory )
        {
            AuthenticationService = authenticationService;
            NotificationService = notificationService;
            if(loggerFactory !=null){
            Logger = loggerFactory.CreateLogger(this.GetType().FullName);
            }
            BaseObj = Activator.CreateInstance<T>();

            if (AuthenticationService?.User != null && Utilities.IsAssignableToGenericType(BaseObj.GetType(), typeof(BaseCompanyGroup<>)))
            {
                var companyGroup = new E00200_CompanyGroup();
                companyGroup.Rowid = AuthenticationService.User.RowidCompanyGroup;
                BaseObj.GetType().GetProperty("RowidCompanyGroup").SetValue(BaseObj, AuthenticationService.User.RowidCompanyGroup);
                BaseObj.GetType().GetProperty("CompanyGroup").SetValue(BaseObj, companyGroup);
            }

            if(string.IsNullOrEmpty(BusinessName)){
                BusinessName = this.GetType().Name;
            }
        }

        public BLFrontendSimple(IAuthenticationService authenticationService, SDKNotificationService notificationService = null, ILoggerFactory loggerFactory = null)
        {
            InternalConstructor(authenticationService, notificationService,loggerFactory);
        }

        public BLFrontendSimple(IServiceProvider provider)
        {
            IAuthenticationService authService = (IAuthenticationService)provider.GetService(typeof(IAuthenticationService));

            SDKNotificationService notiService = (SDKNotificationService)provider.GetService(typeof(SDKNotificationService));
            ILoggerFactory loggerFactory = (ILoggerFactory)provider.GetService(typeof(ILoggerFactory));
            InternalConstructor(authService, notiService,loggerFactory);
        }

        public virtual T Get(Int64 rowid, List<string> extraFields = null)
        {
            return GetAsync(rowid).GetAwaiter().GetResult();
        }

        public async virtual Task<T> GetAsync(Int64 rowid, List<string> extraFields = null)
        {
            var message = await Backend.Get(rowid, extraFields);
            var result = JsonConvert.DeserializeObject<T>(message);
            return result;
        }

        public async virtual Task<Int64> SaveAsync()
        {
            var result = await Backend.Save(this);
            return result;
        }

        public async virtual Task InitializeBusiness(Int64 rowid, List<string> extraFields = null)
        {
            BaseObj = await GetAsync(rowid, extraFields );
        }

        public async virtual Task GetDuplicateInfo(Int64 rowid)
        {
            BaseObj = await GetAsync(rowid);
            //clear rowid
            BaseObj.SetRowid(0);
            var blankBaseObj = Activator.CreateInstance<T>();
            if (Utilities.IsAssignableToGenericType(BaseObj.GetType(), typeof(BaseAudit<>)))
            {
                foreach (var field in typeof(BaseAudit<>).GetProperties())
                {
                    //if field is nullable, set it to null
                    if (field.PropertyType.IsGenericType && field.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        BaseObj.GetType().GetProperty(field.Name).SetValue(BaseObj, null);
                    }
                    else
                    {
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
            try
            {
                var result = await Backend.Delete(BaseObj.GetRowid());
                return result;
            }
            catch (Exception e)
            {
            await GetNotificacionService("Custom.Generic.Message.DeleteError");

            return null;
            }
        }

        public override string ToString()
        {
            if (BaseObj == null)
            {
                return "";
            }
            return BaseObj.ToString();
        }

        public virtual Siesa.SDK.Shared.Business.LoadResult GetData(int? skip, int? take, string filter = "", string orderBy = "", QueryFilterDelegate<T> queryFilter = null, bool includeCount = false)
        {
            return GetDataAsync(skip, take, filter, orderBy).GetAwaiter().GetResult();
        }

        public virtual Siesa.SDK.Shared.Business.LoadResult EntityFieldSearch(string searchText, string filters, int? top = null)
        {
            return EntityFieldSearchAsync(searchText, filters, top).GetAwaiter().GetResult();
        }

        public async virtual Task<Siesa.SDK.Shared.Business.LoadResult> EntityFieldSearchAsync(string searchText, string filters, int? top = null)
        {
            var result = await Backend.EntityFieldSearch(searchText, filters, top);
            Siesa.SDK.Shared.Business.LoadResult response = new Siesa.SDK.Shared.Business.LoadResult();
            response.Data = result.Data.Select(x => JsonConvert.DeserializeObject<T>(x)).ToList();
            response.TotalCount = result.TotalCount;
            response.GroupCount = result.GroupCount;
            return response;
        }

        public async Task<List<dynamic>> GetDataWithTop(string filters = ""){
            var result = new List<dynamic>();
            var response = await Backend.Call("GetDataWithTop", filters);
            if(response != null){
                result = response.Data;
            }
            return result;
        }

        public async virtual Task<Siesa.SDK.Shared.Business.LoadResult> GetDataAsync(int? skip, int? take, string filter = "", string orderBy = "", bool includeCount = false)
        {
            Siesa.SDK.Shared.Business.LoadResult response = new Siesa.SDK.Shared.Business.LoadResult();
            try
            {
                var result = await Backend.GetData(skip, take, filter, orderBy, includeCount);

                response.Data = result.Data.Select(x => JsonConvert.DeserializeObject<T>(x)).ToList();
                response.TotalCount = result.Data.Count;
                if(includeCount){
                    response.TotalCount = result.TotalCount;
                }
                response.GroupCount = result.GroupCount;

                return response;
            }
            catch (Exception e)
            {
                await GetNotificacionService("Custom.Generic.Message.Error");

                return response;
            }

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
            if (currentObject != null)
            {
                var property = currentObject.GetType().GetProperty(fieldPath.Last());
                if (property != null)
                {
                    property.SetValue(currentObject, value);
                }
            }
        }
        public async Task GetNotificacionService(string message)
        {

            if (NotificationService != null)
            {
                await NotificationService.ShowError(message);
            }
            else
            {
                Console.WriteLine(message);
            }
        }

        public async Task<string> DowunloadFile(string url)
        {
            var result = await Backend.Call("DowunloadFile", url);
            return result.Data;
        }

        [SDKApiMethod("POST")]
        public virtual async Task<SDKFileUploadDTO> UploadSingle(IFormFile file){
            var result = new SDKFileUploadDTO();
            if (file == null){
                throw new Exception("File is null");
            }
            byte[] fileBytes = null;
            using (var ms = new MemoryStream()){
                file.CopyTo(ms);
                fileBytes = ms.ToArray();
            }
            var response = await Backend.Call("SaveFile", fileBytes, file.FileName);
            if(response.Success){
                result.Url = response.Data.Url;
                result.FileType = file.ContentType;
                result.FileName = file.FileName;
            }else{
                var errors = JsonConvert.DeserializeObject<List<string>> (response.Errors.ToString());
                throw new ArgumentException(errors[0]);
            }
            return result;
        }

        [SDKApiMethod("POST")]
        public virtual async Task<SDKFileUploadDTO> UploadSingleByte(IFormFile file){
            var result = new SDKFileUploadDTO();
            if (file == null){
                throw new Exception("File is null");
            }
            byte[] fileBytes = null;
            using (var ms = new MemoryStream()){
                file.CopyTo(ms);
                fileBytes = ms.ToArray();
            }
            var response = await Backend.Call("SaveFile", fileBytes, file.FileName);
            if(response.Success){
                result.Url = response.Data.Url;
                result.FileType = file.ContentType;
                result.FileName = file.FileName;
                result.FileContent = fileBytes;
            }else{
                var errors = JsonConvert.DeserializeObject<List<string>> (response.Errors.ToString());
                throw new ArgumentException(errors[0]);
            }
            return result;
        }

        public async Task<int> SaveAttachmentDetail(SDKFileUploadDTO obj, int rowid = 0){
            var BLAttatchmentDetail = GetBackend("BLAttachmentDetail");
            var result = await BLAttatchmentDetail.Call("SaveAttatchmentDetail", obj, rowid);
            if(result.Success){
                return result.Data;
            }else{
                var errors = JsonConvert.DeserializeObject<List<string>> (result.Errors.ToString());
                throw new ArgumentException(errors[0]);
            }
        }
    }
}
