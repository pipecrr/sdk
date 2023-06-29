using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Siesa.SDK.Backend.Access;
using Siesa.SDK.Entities;
using Siesa.SDK.Protos;
using Siesa.SDK.Shared.Business;
using Siesa.SDK.Shared.Validators;
using Siesa.SDK.Shared.Exceptions;
using Siesa.SDK.Backend.Exceptions;
using Microsoft.Extensions.Logging;
using Siesa.SDK.GRPCServices;
using System.Linq.Dynamic.Core;
using Siesa.SDK.Shared.Services;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using Siesa.SDK.Shared.DataAnnotations;
using Siesa.SDK.Backend.Services;
using Siesa.SDK.Shared.DTOS;
using System.Linq.Expressions;
using Siesa.SDK.Backend.Extensions;
using System.Data;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Siesa.Global.Enums;
using Siesa.SDK.Shared.Utilities;
using System.Collections;
using Siesa.SDK.Backend.LinqHelper.DynamicLinqHelper;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

using Siesa.Global.Enums;

namespace Siesa.SDK.Business
{
    public class BLBackendSimple : IBLBase<BaseSDK<int>>
    {
        [JsonIgnore]
        protected IAuthenticationService AuthenticationService { get; set; }
        [JsonIgnore]
        protected IBackendRouterService _backendRouterService { get; set; }
        protected IFeaturePermissionService FeaturePermissionService { get; set; }
        private IServiceProvider _provider;
        private IAmazonS3 _s3Client;
        private IConfiguration _configuration;
        private ILogger _logger;
        protected ILogger Logger { get { return _logger; } }
        protected dynamic _dbFactory;

        private SDKContext myContext;
        private bool _useS3 = false;
        protected SDKContext Context { get { return myContext; } }
        private IEnumerable<INavigation> _navigationProperties = null;

        private bool _containAttachments;

        public SDKBusinessModel GetBackend(string business_name)
        {
            return BackendRouterService.Instance.GetSDKBusinessModel(business_name, AuthenticationService);
        }

        public BLBackendSimple(IAuthenticationService authenticationService)
        {
            AuthenticationService = authenticationService;
        }

        public string BusinessName { get; set; }
        public BaseSDK<int> BaseObj { get; set; }
        public List<dynamic> DynamicEntities { get; set; }

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

        public Shared.Business.LoadResult GetData(int? skip, int? take, string filter = "", string orderBy = "", QueryFilterDelegate<BaseSDK<int>> queryFilter = null, bool includeCount = false, bool includeAttachments = true, List<string> extraFields = null)
        {
            return null;
        }

        public Shared.Business.LoadResult GetUData(int? skip, int? take, string filter = "", string uFilter = "", string orderBy = "", QueryFilterDelegate<BaseSDK<int>> queryFilter = null, bool includeCount = false, List<string> selectFields = null)
        {
            return null;
        }

        public void Update()
        {
        }

        public ValidateAndSaveBusinessObjResponse ValidateAndSave(bool ignorePermissions = false)
        {
            return null;
        }
        public void ShareProvider(dynamic bl)
        {
            bl.SetProvider(_provider);
        }

        public IServiceProvider GetProvider()
        {
            return _provider;
        }


        public virtual void SetProvider(IServiceProvider provider)
        {
            _provider = provider;

            _dbFactory = _provider.GetService(typeof(IDbContextFactory<SDKContext>));
            ILoggerFactory loggerFactory = (ILoggerFactory)_provider.GetService(typeof(ILoggerFactory));
            _logger = loggerFactory.CreateLogger(this.GetType().FullName);

            myContext = _dbFactory.CreateDbContext();
            myContext.SetProvider(_provider);

            AuthenticationService = (IAuthenticationService)_provider.GetService(typeof(IAuthenticationService));
            _backendRouterService = _provider.GetService(typeof(IBackendRouterService)) as IBackendRouterService;
            _configuration = (IConfiguration)_provider.GetService(typeof(IConfiguration));
            _useS3 = _configuration.GetValue<bool>("AWS:UseS3");
            if(_useS3){
                _s3Client = (IAmazonS3)_provider.GetService(typeof(IAmazonS3)) as IAmazonS3;
            }

            if(string.IsNullOrEmpty(BusinessName))
            {
                BusinessName = this.GetType().Name;
            }
        }

        public SDKContext CreateDbContext(bool UseLazyLoadingProxies = false)
        {
            dynamic retContext = null;
            try
            {
                var tenantProvider = _provider.GetRequiredService<ITenantProvider>();
                if (UseLazyLoadingProxies)
                {
                    tenantProvider.SetUseLazyLoadingProxies(true);
                    retContext = _provider.GetService(typeof(SDKContext));
                }
                else
                {
                    tenantProvider.SetUseLazyLoadingProxies(false);
                }
            }
            catch (System.Exception)
            {
            }

            if (retContext == null)
            {
                retContext = _dbFactory.CreateDbContext();
            }

            if (UseLazyLoadingProxies)
            {
                retContext.ChangeTracker.LazyLoadingEnabled = true;
            }
            else
            {
                retContext.ChangeTracker.LazyLoadingEnabled = false;
            }
            retContext.SetProvider(_provider);
            return retContext;
        }
    }
    public class BLBackendSimple<T, K> : IBLBase<T> where T : class, IBaseSDK where K : BLBaseValidator<T>
    {
        [JsonIgnore]
        protected IAuthenticationService AuthenticationService { get; set; }
        [JsonIgnore]
        protected IBackendRouterService _backendRouterService { get; set; }
        [JsonIgnore]
        protected IFeaturePermissionService FeaturePermissionService { get; set; }

        public SDKBusinessModel GetBackend(string business_name)
        {
            return BackendRouterService.Instance.GetSDKBusinessModel(business_name, AuthenticationService);
        }

        private IServiceProvider _provider;
        private IAmazonS3 _s3Client;
        private IConfiguration _configuration;
        private ILogger _logger;
        protected ILogger Logger { get { return _logger; } }
        protected dynamic _dbFactory;
        protected IFeaturePermissionService _featurePermissionService;

        public string BusinessName { get; set; }
        public T BaseObj { get; set; }
        public List<dynamic> DynamicEntities { get; set; }
        private string[] _relatedProperties = null;
        private string[] _relatedAttachmentsType = null;
        protected SDKContext ContextMetadata;
        public List<string> RelFieldsToSave { get; set; } = new List<string>();
        private bool CanCreate { get; set; } = true;
        private bool CanUploadAttachment { get; set; } = true;
        private bool CanDownloadAttachment { get; set; } = true;
        private bool CanEdit { get; set; } = true;
        private IEnumerable<INavigation> _navigationProperties = null;
        private bool _useS3 = false;
        private List<object> unique_indexes = new List<object>();

        private bool _containAttachments;

        public void DetachedBaseObj()
        {
            //TODO: Complete
            //myContext.Entry(BaseObj).State = EntityState.Detached;
            //BaseObj = (T)myContext.Entry(BaseObj).CurrentValues.ToObject();
        }

        private void InternalConstructor()
        {
            BaseObj = Activator.CreateInstance<T>();
            var _bannedTypes = new List<Type>() { typeof(string), typeof(byte[]) };
            _relatedProperties = BaseObj.GetType().GetProperties().Where(
                p => p.PropertyType.IsClass
                    && !p.PropertyType.IsPrimitive
                    && !p.PropertyType.IsEnum
                    && !_bannedTypes.Contains(p.PropertyType)
                    && p.Name != "RowVersion"
                    && p.GetCustomAttribute(typeof(NotMappedAttribute)) == null
            ).Select(p => p.Name).ToArray();
            _relatedAttachmentsType = BaseObj.GetType().GetProperties().Where(p => p.PropertyType == typeof(E00271_AttachmentDetail)).Select(p => p.Name).ToArray();
            unique_indexes = BaseObj.GetType()
                .GetCustomAttributes(typeof(Microsoft.EntityFrameworkCore.IndexAttribute), false)
                .Where(x =>
                    x.GetType().GetProperty("IsUnique").GetValue(x, null).Equals(true)
                ).Select(x =>
                    x.GetType().GetProperty("PropertyNames").GetValue(x, null)
                ).ToList();
                
            if(BaseObj.GetType().GetProperty("RowidAttachment") != null){
                _containAttachments = true;
            }

        }

        public BLBackendSimple(IServiceProvider provider)
        {
            if (provider != null)
            {
                SetProvider(provider);
            }
            InternalConstructor();

        }

        public BLBackendSimple(IAuthenticationService authenticationService)
        {
            AuthenticationService = authenticationService;
            InternalConstructor();

        }

        public void ShareProvider(dynamic bl)
        {
            bl.SetProvider(_provider);
        }

        public IServiceProvider GetProvider()
        {
            return _provider;
        }

        public virtual void SetProvider(IServiceProvider provider)
        {
            _provider = provider;

            _dbFactory = _provider.GetService(typeof(IDbContextFactory<SDKContext>));

            ILoggerFactory loggerFactory = (ILoggerFactory)_provider.GetService(typeof(ILoggerFactory));
            _logger = loggerFactory.CreateLogger(this.GetType().FullName);

            ContextMetadata = _dbFactory.CreateDbContext();
            ContextMetadata.SetProvider(_provider);
            var typeContext = ContextMetadata.Model.FindEntityType(typeof(T));
            if (typeContext != null)
            {
                _navigationProperties = ContextMetadata.Model.FindEntityType(typeof(T)).GetNavigations().Where(p => p.IsOnDependent);
            }
            else
            {
                _navigationProperties = new List<INavigation>();
            };

            AuthenticationService = (IAuthenticationService)_provider.GetService(typeof(IAuthenticationService));


            _backendRouterService = (IBackendRouterService)_provider.GetService(typeof(IBackendRouterService));
            _featurePermissionService = (IFeaturePermissionService)_provider.GetService(typeof(IFeaturePermissionService));
            _configuration = (IConfiguration)_provider.GetService(typeof(IConfiguration));
            _useS3 = _configuration.GetValue<bool>("AWS:UseS3");
            if(_useS3){
                _s3Client = (IAmazonS3)_provider.GetService(typeof(IAmazonS3));
            }

            if(string.IsNullOrEmpty(BusinessName))
            {
                BusinessName = this.GetType().Name;
            }
        }

        [SDKExposedMethod]
        public ActionResult<bool> CheckUnique(T requestObj)
        {
            try
            {
                if (unique_indexes.Count <= 0)
                {
                    return new ActionResult<bool>() { Success = true, Data = false };
                }

                using (SDKContext context = CreateDbContext())
                {
                    var entityType = BaseObj.GetType();
                    T currentObject = null;
                    if (requestObj.GetRowid() != 0)// Si editando, entonces asignar el objeto a una variable para comparar con el mismo
                    {
                        currentObject = Get(requestObj.GetRowid());
                    }
                    foreach (var u_index in unique_indexes)
                    {
                        List<string> index_fields = (List<string>)u_index;
                        bool exist_row = false;
                        Expression existExpression = null;
                        ParameterExpression pe = Expression.Parameter(entityType, entityType.Name);
                        //(pe => campo1 == 2 && campo2 == 3)
                        foreach (var index_field in index_fields)
                        {
                            var columnNameProperty = SDKFlexExtension.GetPropertyExpression(pe, index_field);
                            var field_value = requestObj.GetType().GetProperty(index_field).GetValue(requestObj, null);
                            if (index_field.StartsWith("Rowid"))
                            {
                                try
                                {
                                    var related_field_value = requestObj.GetType().GetProperty(index_field.Replace("Rowid", "")).GetValue(requestObj, null);
                                    if (related_field_value != null)
                                    {
                                        field_value = related_field_value.GetType().GetProperty("Rowid").GetValue(related_field_value, null);
                                    }
                                }
                                catch (System.Exception)
                                {
                                }
                            }
                            var columnValue = Expression.Constant(field_value);
                            Expression tmpExp = Expression.Equal(columnNameProperty, columnValue);
                            if (existExpression == null)
                            {
                                existExpression = tmpExp;
                            }
                            else
                            {
                                existExpression = Expression.And(existExpression, tmpExp);
                            }
                        }

                        if (currentObject != null)
                        {
                            try
                            {

                                existExpression = Expression.And(existExpression, Expression.NotEqual(Expression.Property(pe, "Rowid"), Expression.Constant(currentObject.GetType().GetProperty("Rowid").GetValue(currentObject, null))));
                            }
                            catch (System.Exception)
                            {
                            }
                        }
                        var funcExpression = typeof(Func<,>).MakeGenericType(new Type[] { entityType, typeof(bool) });
                        var returnExp = Expression.Lambda(funcExpression, existExpression, new ParameterExpression[] { pe });
                        var query = context.AllSet<T>().Where(returnExp);
                        exist_row = query.Count() > 0;
                        if (exist_row)
                        {
                            return new ActionResult<bool>() { Success = true, Data = true };
                        }
                    }
                }
                return new ActionResult<bool>() { Success = true, Data = false };
            }
            catch (Exception e)
            {
                return new BadRequestResult<bool> { Success = false, Errors = new List<string> { e.Message } };
            }
        }

        public virtual T Get(Int64 rowid, List<string> extraFields = null)
        {
            using (SDKContext context = CreateDbContext())
            {
                var query = context.Set<T>().AsQueryable();
                var selectedFields = "";
                bool hasRelated = false;
                bool hasExtraFields = false;
                List<string> inlcudesAdd = new List<string>();
                if (extraFields != null && extraFields.Count > 0)
                {
                    hasExtraFields = true;
                    CreateQueryExtraFields(query, inlcudesAdd, extraFields, ref selectedFields, ref hasRelated, _containAttachments);
                }
                else
                {
                    foreach (var relatedProperty in _relatedProperties)
                    {
                        query = query.Include(relatedProperty);
                    }
                }

                query = query.Where("Rowid == @0", ConvertToRowidType(rowid));
                if(hasRelated){
                    var dynamicQuery = query.Select($"new ({selectedFields})");
                    dynamic dynamicObj = dynamicQuery.FirstOrDefault();

                    dynamic result = (T)CreateDynamicObject(typeof(T), dynamicObj);
    
                    return result;
                }else{
                    if(hasExtraFields){
                        query = query.Select<T>($"new ({selectedFields})");
                    }
                    return query.FirstOrDefault();
                }

            }
        }

        private T CreateDynamicObject(Type type, dynamic dynamicObj)
        {
            dynamic result = Activator.CreateInstance(type);
            foreach (var property in dynamicObj.GetType().GetProperties()){
                var propertyName = property.Name;
                var splitProperty = propertyName.Split('_');
                if(splitProperty.Length > 1){
                    var auxType = result;
                    for (int i = 0; i < splitProperty.Length; i++){
                        propertyName = splitProperty[i];
                        if(i == splitProperty.Length-1){
                            auxType.GetType().GetProperty(propertyName).SetValue(auxType, property.GetValue(dynamicObj, null));
                        }else{
                            dynamic InstanceDynamicProp = auxType.GetType().GetProperty(propertyName).GetValue(auxType, null);
                            if(InstanceDynamicProp == null){
                                InstanceDynamicProp = Activator.CreateInstance(auxType.GetType().GetProperty(propertyName).PropertyType);
                            }
                            auxType.GetType().GetProperty(propertyName).SetValue(auxType, InstanceDynamicProp);
                            auxType = InstanceDynamicProp;
                        }
                    }
                }else{
                    bool existProperty = type.GetProperty(propertyName) != null;
                    if(existProperty){
                        var propertyValue = property.GetValue(dynamicObj, null);
                        type.GetProperty(propertyName).SetValue(result, propertyValue);
                    }
                }
            }
            return (T)result;
        }

        //utilizado despues de que ya se guardo el objeto.
        //comentado para luego utilizar docfx
        public virtual void AfterValidateAndSave(ref ValidateAndSaveBusinessObjResponse result)
        {
            //Do nothing
        }
        //utilizado despues que se guarda el objeto en la base de datos con la posibilidad de hacer rollback en caso de error.
        //comentado para luego utilizar docfx
        public virtual void AfterSave(ref ValidateAndSaveBusinessObjResponse result, SDKContext context)
        {
            //Do nothing
        }
        //utilizado antes de guardar el objeto en la base de datos con la posibilidad de hacer rollback en caso de error.
        //comentado para luego utilizar docfx
        public virtual void BeforeSave(ref ValidateAndSaveBusinessObjResponse result, SDKContext context)
        {
            //Do nothing
        }
        //utilizado despues de eliminar el objeto, con la posibilidad de hacer rollback en caso de error.
        //comentado para luego utilizar docfx
        public virtual void AfterDelete(ref ValidateAndSaveBusinessObjResponse result, SDKContext context)
        {
            //Do nothing
        }
        //utilizado antes de eliminar el objeto, con la posibilidad de hacer rollback en caso de error.
        //comentado para luego utilizar docfx
        public virtual void BeforeDelete(ref ValidateAndSaveBusinessObjResponse result, SDKContext context)
        {
            //Do nothing
        }
        public virtual ValidateAndSaveBusinessObjResponse ValidateAndSave(bool ignorePermissions = false)
        {
            ValidateAndSaveBusinessObjResponse result = new();
            if (!ignorePermissions)
            {
                if (_featurePermissionService != null && !string.IsNullOrEmpty(BusinessName) && !BusinessName.Equals("BLAttachmentDetail"))
                {
                    CanCreate = _featurePermissionService.CheckUserActionPermission(BusinessName, 1, AuthenticationService);
                    CanEdit = _featurePermissionService.CheckUserActionPermission(BusinessName, 2, AuthenticationService);
                }
                if (!CanCreate && !CanEdit)
                {
                    AddMessageToResult("Custom.Generic.Unauthorized", result);
                    return result;
                }
            }

            try
            {

                Validate(ref result);

                if (result.Errors.Count > 0)
                {
                    return result;
                }
                using(SDKContext context = CreateDbContext()){
                    try{
                        context.BeginTransaction();
                        BeforeSave(ref result, context);
                        result.Rowid = Save(context);
                        if (DynamicEntities != null && DynamicEntities.Count > 0)
                        {
                            SaveDynamicEntity(result.Rowid, context);
                        }
                        AfterSave(ref result, context);
                        context.Commit();
                        AfterValidateAndSave(ref result);
                    }catch(Exception ex){
                        context.Rollback();
                        result.Errors.Add(new OperationError() { Message = ex.Message });
                    }
                }
            }
            catch (DbUpdateException exception)
            {
                exception.Data.Add("Entity:", "entityName");
                AddExceptionToResult(exception, result);
                _logger.LogError(exception, "Error saving in BLBackend");
                _logger.LogError("Text information");
            }
            catch (Exception exception)
            {
                AddExceptionToResult(exception, result);
                _logger.LogError(exception, "Error saving in BLBackend");
            }
            return result;
        }

        protected virtual void SaveDynamicEntity(Int64 rowid, SDKContext context)
        {
            var nameSpaceEntity = typeof(T).Namespace;
            var nameDynamicEntity = "D"+typeof(T).Name.Substring(1);
            var dynamicEntityType = Utilities.SearchType(nameSpaceEntity + "." + nameDynamicEntity, true);

            var dynamicEntitiesType = typeof(List<>).MakeGenericType(new Type[] { dynamicEntityType });
            dynamic dynamicEntitiesToInsert = Activator.CreateInstance(dynamicEntitiesType);
            dynamic dynamicEntitiesToUpdate = Activator.CreateInstance(dynamicEntitiesType);

            var methodAdd = dynamicEntitiesType.GetMethod("Add");
            bool existInsert = false;
            bool existUpdate = false;
            foreach (dynamic dynamicEntityDTO in DynamicEntities)
            {
                var rowidGroup = dynamicEntityDTO.Rowid; 
                var DynamicEntityFieldsType = typeof(DynamicEntityFieldsDTO<>).MakeGenericType(BaseObj.GetRowidType());
                dynamic fields = dynamicEntityDTO.Fields;
                var dynamicObject = JObject.Parse(dynamicEntityDTO.DynamicObject.ToString());
                foreach (var prop in dynamicObject)
                {
                    dynamic dynamicEntity = Activator.CreateInstance(dynamicEntityType);
                    dynamicEntity.GetType().GetProperty("RowidRecord").SetValue(dynamicEntity, Convert.ChangeType(rowidGroup, dynamicEntity.GetType().GetProperty("RowidRecord").PropertyType));
                    SetValuesDynamicEntity(dynamicEntity, dynamicObject, prop, fields, dynamicEntityType, rowid);
                    
                    if(dynamicEntity.Rowid == 0){
                        methodAdd.Invoke(dynamicEntitiesToInsert, new object[] { dynamicEntity });
                        existInsert = true;
                    }else{
                        methodAdd.Invoke(dynamicEntitiesToUpdate, new object[] { dynamicEntity });
                        existUpdate = true;
                    }
                }
            }

            try{
                if(existUpdate){
                    Assembly assembly = typeof(DbContextExtensions).Assembly;
                    var updateRangeMethod = typeof(DbContext).GetMethod("UpdateRange", new Type[] { typeof(IEnumerable<>).MakeGenericType(dynamicEntityType) });
                    updateRangeMethod.Invoke(context, new object[] { dynamicEntitiesToUpdate });
                }
                if(existInsert){
                    var AddRangeMethod = typeof(DbContext).GetMethod("AddRange", new Type[] { typeof(IEnumerable<>).MakeGenericType(dynamicEntityType) });
                    AddRangeMethod.Invoke(context, new object[] { dynamicEntitiesToInsert });
                }
            }catch(Exception ex){
                throw new Exception("Error updating or inserting aditional fields", ex);
            }

            context.SaveChanges();
        }

        private void SetValuesDynamicEntity(dynamic dynamicEntity, dynamic dynamicObject, dynamic prop, dynamic fields, Type dynamicEntityType, dynamic rowidRecord)
        {   
            var value = prop.Value;
            if (value.Type == JTokenType.Date)
            {
                dynamicEntity.GetType().GetProperty("DateData").SetValue(dynamicEntity, value.ToObject<DateTime>());
            }
            else if (value.Type == JTokenType.String)
            {
                dynamicEntity.GetType().GetProperty("TextData").SetValue(dynamicEntity, value.ToObject<string>());
            }
            else if (value.Type == JTokenType.Float || value.Type == JTokenType.Integer)
            {
                dynamicEntity.GetType().GetProperty("NumericData").SetValue(dynamicEntity, value.ToObject<decimal>());
            }
            if (fields.ContainsKey(prop.Name))
            {
                dynamic field = fields[prop.Name];
                dynamicEntity.GetType().GetProperty("Rowid").SetValue(dynamicEntity, Convert.ChangeType(field.Rowid, dynamicEntityType.GetProperty("Rowid").PropertyType));
                dynamicEntity.GetType().GetProperty("RowVersion").SetValue(dynamicEntity, Convert.ChangeType(field.RowVersion, typeof(byte[])));
                dynamicEntity.GetType().GetProperty("CreationDate").SetValue(dynamicEntity, Convert.ChangeType(field.CreationDate, typeof(DateTime)));
                dynamicEntity.GetType().GetProperty("LastUpdateDate").SetValue(dynamicEntity, Convert.ChangeType(field.LastUpdateDate, typeof(DateTime?)));
                dynamicEntity.GetType().GetProperty("Source").SetValue(dynamicEntity, Convert.ChangeType(field.Source, dynamicEntityType.GetProperty("Source").PropertyType));
                dynamicEntity.GetType().GetProperty("RowidUserCreates").SetValue(dynamicEntity, Convert.ChangeType(field.RowidUserCreates, typeof(Int32)));
                dynamicEntity.GetType().GetProperty("RowidUserLastUpdate").SetValue(dynamicEntity, Convert.ChangeType(field.RowidUserLastUpdate, typeof(Int32)));
                dynamicEntity.GetType().GetProperty("RowidSession").SetValue(dynamicEntity, Convert.ChangeType(field.RowidSession, typeof(Int32?)));
                dynamicEntity.GetType().GetProperty("RowidRecord").SetValue(dynamicEntity, Convert.ChangeType(rowidRecord, BaseObj.GetRowidType()));
                dynamicEntity.GetType().GetProperty("RowidEntityColumn").SetValue(dynamicEntity, Convert.ChangeType(field.RowidEntityColumn, typeof(Int32)));
                dynamicEntity.GetType().GetProperty("RowData").SetValue(dynamicEntity, Convert.ChangeType(field.RowData, typeof(short)));
                dynamicEntity.GetType().GetProperty("RowidInternalEntityData").SetValue(dynamicEntity, Convert.ChangeType(field.RowidInternalEntityData, typeof(Int32?)));
                dynamicEntity.GetType().GetProperty("RowidGenericEntityData").SetValue(dynamicEntity, Convert.ChangeType(field.RowidGenericEntityData, typeof(Int32?)));
            }
            if (value.Type == JTokenType.Date)
            {
                dynamicEntity.GetType().GetProperty("DateData").SetValue(dynamicEntity, value.ToObject<DateTime>());
            }
            else if (value.Type == JTokenType.String)
            {
                dynamicEntity.GetType().GetProperty("TextData").SetValue(dynamicEntity, value.ToObject<string>());
            }
            else if (value.Type == JTokenType.Float || value.Type == JTokenType.Integer)
            {
                dynamicEntity.GetType().GetProperty("NumericData").SetValue(dynamicEntity, value.ToObject<decimal>());
            }

        }
        private void AddExceptionToResult(DbUpdateException exception, ValidateAndSaveBusinessObjResponse result)
        {
            var message = BackendExceptionManager.ExceptionToString(exception, ContextMetadata);
            AddMessageToResult(message, result);
        }

        private void AddExceptionToResult(Exception exception, ValidateAndSaveBusinessObjResponse result)
        {
            var message = ExceptionManager.ExceptionToString(exception);
            AddMessageToResult(message, result);
        }

        private void AddMessageToResult(string message, ValidateAndSaveBusinessObjResponse result)
        {
            message += $"Bussiness Name: {BusinessName}";
            message += $"\nObject {BaseObj}";
            result.Errors.Add(new OperationError() { Message = message });
        }

        private void Validate(ref ValidateAndSaveBusinessObjResponse baseOperation)
        {
            ValidateBussines(ref baseOperation, BaseObj.GetRowid() == 0 ? BLUserActionEnum.Create : BLUserActionEnum.Update);
            //K validator = Activator.CreateInstance<K>();
            K validator = ActivatorUtilities.CreateInstance(_provider, typeof(K)) as K;
            validator.ValidatorType = "Entity";
            SDKValidator.Validate<T>(BaseObj, validator, ref baseOperation);
        }

        private Dictionary<string, object> GetPrimaryKey()
        {

            Dictionary<string, object> returnValue = new Dictionary<string, object>();

            var properties = BaseObj.GetType().GetProperties()
                .Where(x => (x.GetCustomAttributes()
                    .Where(x => x.GetType() == typeof(KeyAttribute)
                            ).ToList().Count > 0
                    )
                );

            var property = properties.FirstOrDefault();

            if (property is not null)
            {

                var valueProp = BaseObj.GetType().GetProperty(property.Name).GetValue(BaseObj);
                if (valueProp != null)
                {
                    var valuePropBigInt = Convert.ToInt64(valueProp);
                    returnValue.Add(property.Name, (valuePropBigInt == 0) ? null : valuePropBigInt.ToString());
                }

            }

            return returnValue;
        }

        private void DisableRelatedProperties(object obj, IEnumerable<INavigation> relatedProperties, List<string> propertiesToKeep = null)
        {
            //TODO: Probar el desvinculado de las propiedades relacionadas
            if (relatedProperties != null)
            {
                foreach (var navProperty in relatedProperties)
                {
                    var foreignKey = navProperty.ForeignKey;
                    var fkProperties = foreignKey.Properties;
                    var principalProperties = foreignKey.PrincipalKey.Properties;
                    if (foreignKey.DependentToPrincipal != null)
                    {
                        var fkFieldName = foreignKey.DependentToPrincipal.Name;
                        var fkFieldValue = obj.GetType().GetProperty(fkFieldName).GetValue(obj);
                        if (fkFieldValue != null)
                        {
                            if (propertiesToKeep != null && propertiesToKeep.Contains(fkFieldName))
                            {
                                var relNavigations = ContextMetadata.Model.FindEntityType(fkFieldValue.GetType()).GetNavigations().Where(p => p.IsOnDependent);
                                DisableRelatedProperties(fkFieldValue, relNavigations);
                                continue;
                            }
                            var principalFieldName = principalProperties[0].Name;
                            var principalFieldValue = fkFieldValue.GetType().GetProperty(principalFieldName).GetValue(fkFieldValue);
                            if (principalFieldValue != null)
                            {
                                if (Convert.ToInt64(principalFieldValue) != 0)
                                {
                                    obj.GetType().GetProperty(fkProperties[0].Name).SetValue(obj, principalFieldValue);
                                }

                                //empty the navigation property
                                obj.GetType().GetProperty(fkFieldName).SetValue(obj, null);
                            }
                        }
                    }
                }
            }
        }

        private Int64 Save(SDKContext context)
        {
            this._logger.LogInformation($"Save {this.GetType().Name}");
            
            context.SetProvider(_provider);
            if (BaseObj.GetRowid() == 0)
            {
                DisableRelatedProperties(BaseObj, _navigationProperties, RelFieldsToSave);
                var entry = context.Add<T>(BaseObj);
            }
            else
            {

                var query = context.Set<T>().AsQueryable();
                // foreach (var relatedProperty in _relatedProperties)
                // {
                //     query = query.Include(relatedProperty);
                // }
                var rowidSearch = BaseObj.GetRowid();
                try
                {
                    rowidSearch = ((dynamic)BaseObj).Rowid;
                }
                catch (System.Exception)
                {
                }
                query = query.Where("Rowid == @0", rowidSearch);
                T entity = query.FirstOrDefault();
                context.ResetConcurrencyValues(entity, BaseObj);
                DisableRelatedProperties(BaseObj, _navigationProperties, RelFieldsToSave);
                context.Entry(entity).CurrentValues.SetValues(BaseObj);
                foreach (var relatedProperty in RelFieldsToSave)
                {
                    entity.GetType().GetProperty(relatedProperty).SetValue(entity, BaseObj.GetType().GetProperty(relatedProperty).GetValue(BaseObj));
                }
            }

            context.SaveChanges(); //TODO: Capturar errores db y hacer rollback
            return BaseObj.GetRowid();
        }

        public virtual void Update()
        {
            throw new NotImplementedException();
        }

        public virtual DeleteBusinessObjResponse Delete()
        {
            this._logger.LogInformation($"Detele {this.GetType().Name}");
            var response = new DeleteBusinessObjResponse();

            ValidateAndSaveBusinessObjResponse result = new();
            try
            {
                ValidateBussines(ref result, BLUserActionEnum.Delete);
                if (result.Errors.Count > 0)
                {
                    response.Errors.AddRange(result.Errors);
                    return response;
                }
                using (SDKContext context = CreateDbContext())
                {
                    try{
                        context.BeginTransaction();
                        BeforeDelete(ref result, context);
                        DeleteDynamicEntity(context);
                        DisableRelatedProperties(BaseObj, _navigationProperties);
                        context.SetProvider(_provider);
                        context.Set<T>().Remove(BaseObj);
                        context.SaveChanges();
                        AfterDelete(ref result, context);
                        context.Commit();
                    }catch(Exception ex){
                        context.Rollback();
                        response.Errors.Add(new OperationError() { Message = ex.Message });
                    }
                }
            }
            catch (Exception e)
            {
                this._logger.LogError(e, $"Error deleting {this.GetType().Name}");
                response.Errors.Add(new OperationError() { Message = e.Message });
            }
            return response;
        }

        protected virtual void DeleteDynamicEntity(SDKContext Context, List<int> rowidsEnityColumn = null){
            var nameSpaceEntity = typeof(T).Namespace;
            var nameDynamicEntity = "D"+typeof(T).Name.Substring(1);
            var dynamicEntityType = Utilities.SearchType(nameSpaceEntity + "." + nameDynamicEntity, true);

            if(dynamicEntityType != null){
                try{
                    var dynamicContextSet = Context.GetType().GetMethod("AllSet", types: Type.EmptyTypes).MakeGenericMethod(dynamicEntityType).Invoke(Context, null);
                    var rowid = BaseObj.GetRowid();
                    Assembly assemblyWhere = typeof(System.Linq.Dynamic.Core.DynamicQueryableExtensions).Assembly;
                    var whereMethod = typeof(IQueryable).GetExtensionMethod(assemblyWhere, "Where", new[] { typeof(IQueryable), typeof(string), typeof(object[])});

                    if(rowidsEnityColumn != null){
                        dynamicContextSet = whereMethod.Invoke(dynamicContextSet, new object[] { dynamicContextSet, "RowidEntityColumn in @0", new object[] { rowidsEnityColumn } });
                    }else{
                        dynamicContextSet = whereMethod.Invoke(dynamicContextSet, new object[] { dynamicContextSet, "RowidRecord = @0", new object[] { rowid } });
                    }

                    Assembly assemblyDynamic = typeof(System.Linq.Dynamic.Core.DynamicEnumerableExtensions).Assembly;
                    var dynamicListMethod = typeof(IEnumerable).GetExtensionMethod(assemblyDynamic, "ToDynamicList", new[] { typeof(IEnumerable) });
                    dynamic dynamicEntitiesToDelete = dynamicListMethod.Invoke(dynamicContextSet, new object[] { dynamicContextSet });
                    
                    if(dynamicEntitiesToDelete.Count > 0){
                        var DeleteRangeMethod = typeof(DbContext).GetMethod("RemoveRange", new Type[] { typeof(IEnumerable<>).MakeGenericType(dynamicEntityType) });
                        DeleteRangeMethod.Invoke(Context, new object[] { dynamicEntitiesToDelete });
                    }
                }catch(Exception e){
                    throw new Exception($"Error deleting aditional fields {nameDynamicEntity} {e.Message}");
                }

            }
        }

        public virtual IQueryable<T> EntityFieldFilters(IQueryable<T> query)
        {
            //check if has field Status
            try
            {
                var statusProperty = BaseObj.GetType().GetProperty("Status");
                //check if status is a enumStatusBaseMaster 
                if (statusProperty != null && statusProperty.PropertyType == typeof(enumStatusBaseMaster))
                {
                    query = query.Where("Status == @0", enumStatusBaseMaster.Active);
                }
            }
            catch (Exception e)
            {
                this._logger.LogError(e, $"Error checking status property {this.GetType().Name}");
            }
            return query;
        }

        public virtual Siesa.SDK.Shared.Business.LoadResult EntityFieldSearch(string searchText, string prefilters = "", int? top = null, string orderBy = "", List<string> extraFields = null)
        {
            this._logger.LogInformation($"Field Search {this.GetType().Name}");
            var string_fields = BaseObj.GetType().GetProperties().Where(p => p.PropertyType == typeof(string) && p.GetCustomAttributes().Where(x => x.GetType() == typeof(NotMappedAttribute)).Count() == 0).Select(p => p.Name).ToArray();
            string filter = "";
            foreach (var field in string_fields)
            {
                if (!string.IsNullOrEmpty(filter))
                {
                    filter += " || ";
                }
                filter += $"({field} == null ? \"\" : {field}).ToLower().Contains(\"{searchText}\".ToLower())";
            }
            if (!string.IsNullOrEmpty(prefilters) && !string.IsNullOrEmpty(filter))
            {
                filter = $"({prefilters}) && ({filter})";
            }
            QueryFilterDelegate<T> filterDelegate = EntityFieldFilters;
            var take = 10;
            if (top.HasValue)
            {
                take = top.Value;
            }
            return this.GetData(0, take, filter, orderBy, filterDelegate, includeAttachments: false, extraFields: extraFields);
        }

        [SDKExposedMethod]
        public async Task<ActionResult<List<dynamic>>> GetDataWithTop(string filter = "")
        {
            var result = new List<dynamic>();
            using (SDKContext context = CreateDbContext())
            {
                context.SetProvider(_provider);
                IQueryable query = context.Set<T>();
                if (!string.IsNullOrEmpty(filter))
                {
                    query = query.Where(filter);
                }
                query = query.OrderBy("Rowid");
                query = query.Take(2);
                query = query.Select("Rowid");
                var data = query.ToDynamicList();
                result = data;
            }
            return new ActionResult<List<dynamic>>
            {
                Data = result
            };
        }

        public virtual Siesa.SDK.Shared.Business.LoadResult GetData(int? skip, int? take, string filter = "", string orderBy = "", QueryFilterDelegate<T> queryFilter = null, bool includeCount = false, bool includeAttachments = true, List<string> extraFields = null)
        {
            this._logger.LogInformation($"Get Data {this.GetType().Name}");
            var result = new Siesa.SDK.Shared.Business.LoadResult();
            using (SDKContext context = CreateDbContext())
            {
                context.SetProvider(_provider);
                var query = context.Set<T>().AsQueryable();
                var selectedFields = "";
                bool hasRelated = false;
                bool hasExtraFields = false;
                List<string> inlcudesAdd = new List<string>();
                if (extraFields != null && extraFields.Count > 0)
                {
                    hasExtraFields = true;
                    CreateQueryExtraFields(query, inlcudesAdd, extraFields, ref selectedFields, ref hasRelated);
                }
                else
                {
                    foreach (var relatedProperty in _relatedProperties)
                    {
                        if (!includeAttachments && _relatedAttachmentsType != null && _relatedAttachmentsType.Contains(relatedProperty))
                        {
                            continue;
                        }
                        query = query.Include(relatedProperty);
                    }
                }

                if (!string.IsNullOrEmpty(filter))
                {
                    query = query.Where(filter);
                }
                var total = 0;
                if (includeCount)
                {
                    total = query.Select("Rowid").Count();
                }

                if (!string.IsNullOrEmpty(orderBy))
                {
                    query = query.OrderBy(orderBy);
                }
                else
                {
                    query = query.OrderBy("Rowid");
                }

                if (skip.HasValue)
                {
                    query = query.Skip(skip.Value);
                }
                if (take.HasValue)
                {
                    query = query.Take(take.Value);
                }

                if (queryFilter != null)
                {
                    query = queryFilter(query);
                }
                //total data
                result.TotalCount = total;
                
                if(hasRelated){
                    var dynamicQuery = query.Select($"new ({selectedFields})");
                    dynamic dynamicList = dynamicQuery.ToDynamicList();
                    dynamic listEntities = new List<T>();
                    foreach (var dynamicObj in dynamicList)
                    {
                        dynamic entity = (T)CreateDynamicObject(typeof(T), dynamicObj);
                        listEntities.Add(entity);
                    }
    
                    result.Data = listEntities;
                }else{
                    if(hasExtraFields){
                        query = query.Select<T>($"new ({selectedFields})");
                    }
                    result.Data = query.ToList();
                }
            }
            return result;
        }

        private void CreateQueryExtraFields(IQueryable<T> query, List<string> inlcudesAdd, List<string> extraFields, ref string selectedFields, ref bool hasRelated, bool containAttachments = false)
        {
            bool hasRelatedTmp = false;
            extraFields.Add("Rowid");
            if(containAttachments)
            {
                extraFields.Add("RowidAttachment");
            }
            selectedFields = string.Join(",", extraFields.Select(x =>
            {
                dynamic splitInclude = x.Split('.');
                if (splitInclude.Length > 1)
                {
                    hasRelatedTmp = true;
                    List<string> inlcudes = new List<string>();
                    for (int i = 0; i < splitInclude.Length-1; i++)
                    {
                        inlcudes.Add(splitInclude[i]);
                    }
                    string include = string.Join(".", inlcudes);
                    if(!inlcudesAdd.Contains(include)){
                        inlcudesAdd.Add(include);
                        query = query.Include(include);

                    }
                }
                return x+" as "+x.Replace(".","_");
            }).Distinct());
            hasRelated = hasRelatedTmp;
        }

        public Task<T> GetAsync(Int64 rowid,List<string> extraFields = null)
        {
            throw new NotImplementedException();
        }
        protected virtual void ValidateBussines(ref ValidateAndSaveBusinessObjResponse operationResult, BLUserActionEnum action)
        {
            // Do nothing
        }

        public SDKContext CreateDbContext(bool UseLazyLoadingProxies = false)
        {
            dynamic retContext = null;
            try
            {
                var tenantProvider = _provider.GetRequiredService<ITenantProvider>();
                if (UseLazyLoadingProxies)
                {
                    tenantProvider.SetUseLazyLoadingProxies(true);
                    retContext = _provider.GetService(typeof(SDKContext));
                }
                else
                {
                    tenantProvider.SetUseLazyLoadingProxies(false);
                }
            }
            catch (System.Exception)
            {
            }

            if (retContext == null)
            {
                retContext = _dbFactory.CreateDbContext();
            }

            if (UseLazyLoadingProxies)
            {
                retContext.ChangeTracker.LazyLoadingEnabled = true;
            }
            else
            {
                retContext.ChangeTracker.LazyLoadingEnabled = false;
            }
            retContext.SetProvider(_provider);
            return retContext;
        }

        private object ConvertToRowidType(Int64 rowid)
        {
            try
            {
                return Convert.ChangeType(rowid, BaseObj.GetRowidType());
            }
            catch (System.Exception)
            {
                return rowid;
            }
        }

        [SDKExposedMethod]
        public virtual ActionResult<string> GetObjectString(Int64 rowid)
        {
            using (SDKContext context = CreateDbContext(true))
            {
                context.SetProvider(_provider);
                var query = context.Set<T>().AsQueryable();
                query = query.Where("Rowid == @0", ConvertToRowidType(rowid));
                var entity = query.FirstOrDefault();
                if (entity != null)
                {
                    return new ActionResult<string>
                    {
                        Data = entity.ToString()
                    };
                }
                return new ActionResult<string>
                {
                    Data = ""
                };
            }
        }

        static Expression<Func<TSource, dynamic>> DynamicFields<TSource>(IEnumerable<string> fields)
        {
            var source = Expression.Parameter(typeof(TSource), "o");
            var properties = fields
                .Select(f => typeof(TSource).GetProperty(f))
                .Select(p => new DynamicProperty(p.Name, p.PropertyType))
                .ToList();
            var resultType = DynamicClassFactory.CreateType(properties, false);
            var bindings = properties.Select(p => Expression.Bind(resultType.GetProperty(p.Name), Expression.Property(source, p.Name)));
            var result = Expression.MemberInit(Expression.New(resultType), bindings);
            return Expression.Lambda<Func<TSource, dynamic>>(result, source);
        }

        [SDKExposedMethod]
        public virtual ActionResult<dynamic> SDKFlexPreviewData(SDKFlexRequestData requestData, bool setTop = true)
        {
            using (var Context = CreateDbContext())
            {
                var response = SDKFlexExtension.SDKFlexPreviewData(Context, requestData, AuthenticationService, setTop);
                return response;
            }
        }

        [SDKExposedMethod]
        public ActionResult<long> SaveAttachmentEntity(dynamic BaseObj)
        {
            this.BaseObj = BaseObj;
            var result = this.ValidateAndSave();
            if (result.Errors.Count == 0)
            {
                var response = result.Rowid;
                return new ActionResult<long> { Success = true, Data = response };
            }
            else
            {
                return new BadRequestResult<long> { Success = false, Errors = new List<string> { result.Errors[0].Message } };
            }
            return null;
        }

        [SDKExposedMethod]
        public async Task<ActionResult<SDKFileUploadDTO>> SaveFile(byte[] fileBytes, string name, string contentType, bool SaveBytes = false)
        {
            CanUploadAttachment = await _featurePermissionService.CheckUserActionPermission(BusinessName, enumSDKActions.UploadAttachment ,AuthenticationService);

            if(!CanUploadAttachment)
                return new BadRequestResult<SDKFileUploadDTO>{Success = false, Errors = new List<string> { "You don't have permission to upload attachment" }};

            MemoryStream stream = new MemoryStream(fileBytes);
            var result = new SDKFileUploadDTO();
            var untrustedFileName = name;
            var guid = Guid.NewGuid().ToString();
            untrustedFileName = string.Concat(guid.Substring(1,10), "_", untrustedFileName);
            IFormFile file = new FormFile(stream, 0, fileBytes.Length, untrustedFileName, untrustedFileName);
            if(_useS3){
                return await SaveFileS3(file, contentType);
            }
            IWebHostEnvironment env = _provider.GetRequiredService<IWebHostEnvironment>();
            try{
                result.FileName = untrustedFileName;
                if(!SaveBytes){
                    var path = Path.Combine(env.ContentRootPath,"Uploads");
                    Directory.CreateDirectory(path);
                    var filePath = Path.Combine(path, untrustedFileName);
                    await using FileStream fs = new(filePath, FileMode.Create);
                    await file.CopyToAsync(fs);
                    result.Url = filePath;
                }else{
                    result.Url = untrustedFileName;
                    result.FileContent = fileBytes;
                }
            }
            catch (IOException ex){
                return new BadRequestResult<SDKFileUploadDTO>{Success = false, Errors = new List<string> { ex.Message }};
            }
            return new ActionResult<SDKFileUploadDTO>{Success = true, Data = result};
        }

        private async Task<ActionResult<SDKFileUploadDTO>> SaveFileS3(IFormFile file, string contentType){
            var result = new SDKFileUploadDTO();
            var name = file.FileName;
            var bucketName = _configuration.GetValue<string>("AWS:S3BucketName");
            if(string.IsNullOrEmpty(bucketName)){
                return new BadRequestResult<SDKFileUploadDTO>{Success = false, Errors = new List<string> { "Custom.S3.BucketName.NotFound" }};
            }
            try{
                PutObjectRequest request = new PutObjectRequest{
                    BucketName = bucketName,
                    Key = name,
                    InputStream = file.OpenReadStream(),
                    ContentType = contentType
                };

                var response = await _s3Client.PutObjectAsync(request);
                result.Url = name;
                result.FileName = name;
            }catch(AmazonS3Exception ex){
                return new BadRequestResult<SDKFileUploadDTO>{Success = false, Errors = new List<string> { ex.Message }};
            }catch(Exception ex){
                return new BadRequestResult<SDKFileUploadDTO>{Success = false, Errors = new List<string> { ex.Message }};
            }
            return new ActionResult<SDKFileUploadDTO>{Success = true, Data = result};
        }

        [SDKExposedMethod]
        public async Task<ActionResult<string>> DownloadFile(string url, string contentType)
        {
            CanDownloadAttachment = await _featurePermissionService.CheckUserActionPermission(BusinessName, enumSDKActions.DownloadAttachment ,AuthenticationService);

            if(!CanDownloadAttachment)
                return new BadRequestResult<string>{Success = false, Errors = new List<string> { "You don't have permission to download attachment" }};
            
            var urlRes = "";
            if(_useS3)
        {
                return await DownloadFileS3(url);
            }
            IWebHostEnvironment env = _provider.GetRequiredService<IWebHostEnvironment>();
            var filePath = Path.Combine(url);
            var file = new FileInfo(filePath);
            if (file.Exists)
            {
                var fileBytes = await File.ReadAllBytesAsync(filePath);
                var base64 = Convert.ToBase64String(fileBytes);
                urlRes = $"data:{contentType};base64,{base64}";
                return new ActionResult<string>{Success = true, Data = urlRes};
                return new ActionResult<string> { Success = true, Data = base64 };
            }
            return new BadRequestResult<string>{Success = false, Errors = new List<string> { "Custom.Attatchment.FileNotFound" }};
        }

        public bool GetUses3(){
            return _useS3;
        }

        public async Task<ActionResult<string>> DownloadFileS3(string url)
        {
            var bucketName = _configuration.GetValue<string>("AWS:S3BucketName");
            if(string.IsNullOrEmpty(bucketName)){
                return new BadRequestResult<string>{Success = false, Errors = new List<string> { "Custom.S3.BucketName.NotFound" }};
            }
            var duration = _configuration.GetValue<int>("AWS:TimeoutDuration");
            if(duration == 0){
                duration = 60;
            }
            try
            {            
                var request = new GetPreSignedUrlRequest
                {
                    BucketName = bucketName,
                    Key = url,
                    Expires = DateTime.UtcNow.AddMinutes(duration)
                };
                var urlS3 = _s3Client.GetPreSignedURL(request);
                return new ActionResult<string>{Success = true, Data = urlS3};
            }catch (AmazonS3Exception ex){
                return new BadRequestResult<string>{Success = false, Errors = new List<string> { ex.Message }};
            }catch (Exception ex){
                return new BadRequestResult<string>{Success = false, Errors = new List<string> { ex.Message }};
            }
        }

        [SDKExposedMethod]
        public async Task<ActionResult<SDKFileFieldDTO>> DownloadFileByRowid(Int32 rowid)
        {
            string url = "";
            string dataType = "";
            var BLAttatchmentDetail = GetBackend("BLAttachmentDetail");
            var response = await BLAttatchmentDetail.Call("GetAttatchmentDetail", rowid);
            SDKFileFieldDTO SDKFileField = new SDKFileFieldDTO();
            if(response.Success){
                var data = response.Data;
                SDKFileField = new SDKFileFieldDTO{
                    Url = data.Url,
                    FileName = data.FileName,
                    FileType = data.FileType,
                    FileByte = data.FileByte
                };
            }
            else
            {
                var errors = JsonConvert.DeserializeObject<List<string>>(response.Errors.ToString());
                throw new ArgumentException(errors[0]);
            }
            if(_useS3){
                var downloadS3 = await DownloadFileS3(SDKFileField.Url);
                if(downloadS3.Success){
                    SDKFileField.Url = downloadS3.Data;
                    return new ActionResult<SDKFileFieldDTO>{Success = true, Data = SDKFileField};
                }else{
                    return new BadRequestResult<SDKFileFieldDTO>{Success = false, Errors = downloadS3.Errors};
                }
            }
            if(SDKFileField.FileByte != null){
                var base64 = Convert.ToBase64String(SDKFileField.FileByte);
                SDKFileField.FileBase64 = base64;
                SDKFileField.Url = $"data:{SDKFileField.FileType};base64,{base64}";
                return new ActionResult<SDKFileFieldDTO>{Success = true, Data = SDKFileField};
            }
            IWebHostEnvironment env = _provider.GetRequiredService<IWebHostEnvironment>();
            var filePath = Path.Combine(SDKFileField.Url);
            var file = new FileInfo(filePath);
            if (file.Exists)
            {
                var fileBytes = await File.ReadAllBytesAsync(filePath);
                var base64 = Convert.ToBase64String(fileBytes);
                SDKFileField.FileBase64 = base64;
                SDKFileField.Url = $"data:{SDKFileField.FileType};base64,{base64}";
                return new ActionResult<SDKFileFieldDTO>{Success = true, Data = SDKFileField};
            }
            return new BadRequestResult<SDKFileFieldDTO>{Success = false, Errors = new List<string> { "Custom.Attatchment.FileNotFound" }};
        }

        [SDKExposedMethod]
        public async Task<ActionResult<T>> DataEntity(object rowid)
        {
            using (SDKContext context = CreateDbContext())
            {
                var entityType = typeof(T);
                var rowidType = entityType.GetProperty("Rowid").PropertyType;
                var rowidValue = Convert.ChangeType(rowid, rowidType);
                var query = context.Set<T>().AsQueryable();
                query = query.Where("Rowid == @0", rowidValue);
                var entity = query.FirstOrDefault();
                if (entity != null)
                {
                    return new ActionResult<T>
                    {
                        Data = entity
                    };
                }
                return new ActionResult<T>
                {
                    Data = null
                };
            }
        }

        [SDKExposedMethod]
        public async Task<ActionResult<dynamic>> GetDynamicEntitiesData(Int64 rowid)
        {
            using (SDKContext Context = CreateDbContext())
            {
                var nameEntity = typeof(T).Name;
                var nameDynamicEntity = nameEntity.Replace(nameEntity[0].ToString(), "D");
                var nameSpaceEntity = typeof(T).Namespace;
                var dynamicEntityType = Utilities.SearchType(nameSpaceEntity + "." + nameDynamicEntity, true);

                Assembly _assemblySelect = typeof(System.Linq.Dynamic.Core.DynamicQueryableExtensions).Assembly;
                var contextSet = Context.GetType().GetMethod("Set", types: Type.EmptyTypes).MakeGenericMethod(dynamicEntityType).Invoke(Context, null);
                //include
                Assembly _assemblyInclude = typeof(Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions).Assembly;
                var includeMethod = typeof(IQueryable<object>).GetExtensionMethod(_assemblyInclude, "Include", new[] { typeof(IQueryable<object>), typeof(string) });
                var includeMethodGeneric = includeMethod.MakeGenericMethod(dynamicEntityType);
                contextSet = includeMethodGeneric.Invoke(contextSet, new object[] { contextSet,"EntityColumn" });
                //where
                var whereMethod = typeof(IQueryable).GetExtensionMethod(_assemblySelect, "Where", new[] { typeof(IQueryable), typeof(string), typeof(object[])});
                contextSet = whereMethod.Invoke(null, new object[] { contextSet, "RowidRecord == @0", new object[] { rowid }});
                Assembly _assemblyDynamic = typeof(System.Linq.Dynamic.Core.DynamicEnumerableExtensions).Assembly;
                var dynamicListMethod = typeof(IEnumerable).GetExtensionMethod(_assemblyDynamic, "ToDynamicList", new[] { typeof(IEnumerable) });
                var dynamicList = dynamicListMethod.Invoke(contextSet, new object[] { contextSet });

                return new ActionResult<dynamic>
                {
                    Data = dynamicList
                };
            }
        }

        private string GetUTableEntity()
        {
            var dataAnnotation = typeof(T).GetCustomAttributes(typeof(SDKAuthorization), false);

            string TableName = "";

            if (dataAnnotation.Length > 0)
            {
                //Get the table name
                TableName = ((SDKAuthorization)dataAnnotation[0]).TableName;

                if (!string.IsNullOrEmpty(TableName))
                    return TableName;
            }

            //Get table name from the context
            TableName = typeof(T).Name;

            //Replace the first character of the table name with the letter "u"
            if (TableName.Length > 0)
                TableName = "U" + TableName.Substring(1);

            TableName = $"{typeof(T).Namespace}.{TableName}";
            return TableName;
        }

        public virtual Siesa.SDK.Shared.Business.LoadResult GetUData(int? skip, int? take, string filter = "", string uFilter = "", string orderBy = "", QueryFilterDelegate<T> queryFilter = null, bool includeCount = false, List<string> selectFields = null)
        {
            this._logger.LogInformation($"Get UData {this.GetType().Name}");

            var result = new Siesa.SDK.Shared.Business.LoadResult();
            using (SDKContext context = CreateDbContext())
            {
                context.SetProvider(_provider);
                var query = context.Set<T>().AsQueryable();

                if (!string.IsNullOrEmpty(filter))
                {
                    query = query.Where(filter);
                }

                if (!string.IsNullOrEmpty(orderBy))
                {
                    query = query.OrderBy(orderBy);
                }
                else
                {
                    query = query.OrderBy("Rowid");
                }

                var total = 0;
                if (includeCount)
                {
                    total = query.Select("Rowid").Count();
                }

                if (skip.HasValue)
                {
                    query = query.Skip(skip.Value);
                }
                if (take.HasValue)
                {
                    query = query.Take(take.Value);
                }

                List<string> LeftColumns = new() { "Rowid as ERowid", "Id as Id", "Name as Name", "Status as Status", "IsPrivate as IsPrivate" };

                if (selectFields != null && selectFields.Count > 0)
                {
                    LeftColumns.Clear();
                    selectFields.Add("Rowid");

                    var selectedFields = string.Join(",", selectFields.Select(x =>
                    {
                        var splitInclude = x.Split('.');
                        var Length = splitInclude.Length;
                        if (Length > 1)
                        {
                            for (int i = 1; i <= Length; i++)
                            {
                                var include = string.Join(".", splitInclude.Take(i));

                                try
                                {
                                    var Result = query.Include(include);

                                    if (Result.Any())
                                    {
                                        query = Result;
                                    }
                                }
                                catch (System.Exception)
                                {
                                }
                            }
                            var Alias = string.Join("", splitInclude);
                            LeftColumns.Add($"{string.Join(".", splitInclude)} as E{Alias}");
                        }
                        else
                        {
                            LeftColumns.Add($"{x} as E{x}");
                        }

                        return splitInclude[0];
                    }).Distinct());

                    query = query.Select<T>($"new ({selectedFields})");

                }

                List<string> UExtraFields = new(){
                    "Rowid", "UserType", "AuthorizationType", "RestrictionType"
                };

                var UTableName = GetUTableEntity();
                Type DynamicEntityType = typeof(T).Assembly.GetType(UTableName);
                var RowidRecordType = DynamicEntityType.GetProperty("RowidRecord");

                dynamic TableProxy = context.GetType().GetMethod("Set", types: Type.EmptyTypes).MakeGenericMethod(DynamicEntityType).Invoke(context, null);

                var authSet = TableProxy.AsQueryable();
                List<string> RightColumns = new();

                authSet = GetUFilter(DynamicEntityType, authSet, uFilter);
                authSet = GetUSelect(authSet, UExtraFields, RightColumns, DynamicEntityType);

                Dictionary<string, Type> virtualColumnsNameType = new();
                virtualColumnsNameType.Add("RowidRecord", RowidRecordType.PropertyType);

                Type _typeLeftJoinExtension = typeof(LeftJoinExtension);
                var leftJoinMethod = _typeLeftJoinExtension.GetMethod("LeftJoin");

                var CoincidenceResult = leftJoinMethod.Invoke(null, new object[] { query, authSet, "Rowid", "RowidRecord", LeftColumns, RightColumns });

                var _assemblyDynamic = typeof(System.Linq.Dynamic.Core.DynamicEnumerableExtensions).Assembly;
                var dynamicListMethod = typeof(IEnumerable).GetExtensionMethod(_assemblyDynamic, "ToDynamicList", new[] { typeof(IEnumerable) });

                var dynamicLeftList = dynamicListMethod.Invoke(CoincidenceResult, new object[] { CoincidenceResult });

                //total data
                result.TotalCount = total;
                //data
                result.Data = (IEnumerable<dynamic>)dynamicLeftList;
            }
            return result;
        }

        private IQueryable GetUSelect(dynamic context, List<string> ExtraFields, List<string> RightColumns, Type TypeToReturn)
        {
            //Actualmente no hay necesidad de incluir foraneas
            string strSelect = string.Join(",", ExtraFields.Select(x =>
            {
                RightColumns.Add($"{x} as U{x}");
                return x;
            }));

            // var t = typeof(EntityQueryable<>);
            // Type[] tArg = {TypeToReturn};
            // var GenericType = t.MakeGenericType(tArg);

            // var _assemblySelect = typeof(System.Linq.Dynamic.Core.DynamicQueryableExtensions).Assembly;

            // var selectMethod = typeof(IQueryable).GetExtensionMethod(_assemblySelect, "Select", new[] { typeof(IQueryable), typeof(string), typeof(object[]) });

            // context = selectMethod.Invoke(context, new object[] { context, $"new ({strSelect})", null });

            return context;
        }

        //To-Do : Mejorar el filtrado, actualmente sólo recibe: x == y
        private IQueryable GetUFilter(Type DynamicEntityType, dynamic authSet, string Filter)
        {
            if (string.IsNullOrEmpty(Filter))
                return authSet;

            var FilterSplit = Filter.Split("==").Select(x => x.Trim()).ToArray();

            var pe = Expression.Parameter(DynamicEntityType, DynamicEntityType.Name);

            Expression CoincidenceExpression;
            Expression ColumnNameProperty;
            Expression ColumnValue;

            ColumnNameProperty = Expression.Property(pe, FilterSplit[0]);

            if (DynamicEntityType.GetProperty(FilterSplit[0]).PropertyType.GenericTypeArguments[0] == typeof(Int16))
            {
                var Value = Int16.Parse(FilterSplit[1]);
                ColumnValue = Expression.Constant(Value, typeof(Int16?));
            }
            else
            {
                var Value = Int32.Parse(FilterSplit[1]);
                ColumnValue = Expression.Constant(Value, typeof(int?));
            }

            CoincidenceExpression = Expression.Equal(ColumnNameProperty, ColumnValue);

            authSet = GetWhereExpression(authSet, DynamicEntityType, CoincidenceExpression, pe);

            return authSet;
        }

        [SDKExposedMethod]
        public ActionResult<dynamic> UGetByUserType(int Rowid, PermissionUserTypes UserType, List<string> ExtraFields)
        {
            try
            {
                this._logger.LogInformation($"Get general UObject by UserType {this.GetType().Name}");

                dynamic Result = null;
                var UTableName = GetUTableEntity();
                Type DynamicEntityType = typeof(T).Assembly.GetType(UTableName);

                var RowidRecordType = DynamicEntityType.GetProperty("RowidRecord");

                var EntityExpression = Expression.Parameter(DynamicEntityType, DynamicEntityType.Name);

                Expression ColumnNameProperty = Expression.Property(EntityExpression, "RowidRecord");
                Expression ColumnValue = Expression.Constant(null, typeof(int?));

                //RowidRecord is null
                Expression CoincidenceExpression = Expression.Equal(ColumnNameProperty, ColumnValue);

                string ColumnName;

                switch (UserType)
                {
                    case PermissionUserTypes.Team:
                        ColumnName = "RowidDataVisibilityGroup";
                        break;
                    case PermissionUserTypes.User:
                        ColumnName = "RowidUser";
                        break;
                    default:
                        throw new ArgumentNullException("UserType not supported");
                }

                var _assemblyDynamicQueryable = typeof(System.Linq.Dynamic.Core.DynamicQueryableExtensions).Assembly;

                ColumnNameProperty = Expression.Property(EntityExpression, ColumnName);

                ColumnValue = Expression.Constant(Rowid, typeof(int?));

                CoincidenceExpression = Expression.And(CoincidenceExpression, Expression.Equal(ColumnNameProperty, ColumnValue));

                var FirstOrDefaultMethod = typeof(IQueryable).GetExtensionMethod(_assemblyDynamicQueryable, "FirstOrDefault", new[] { typeof(IQueryable) });

                using (var context = CreateDbContext())
                {
                    dynamic Table = context.GetType().GetMethod("Set", types: Type.EmptyTypes).MakeGenericMethod(DynamicEntityType).Invoke(context, null);

                    var DbSet = Table.AsQueryable();

                    DbSet = GetWhereExpression(DbSet, DynamicEntityType, CoincidenceExpression, EntityExpression);

                    if (ExtraFields.Any() && GetDynamicAny(DbSet))
                    {
                        if (ExtraFields.Any(x => x.Contains(".")))
                            throw new Exception("Foreign keys attributes are not supported to this method");

                        ExtraFields.Add("Rowid");
                        ExtraFields.Add(ColumnName);

                        var selectMethod = typeof(IQueryable).GetExtensionMethod(_assemblyDynamicQueryable, "Select", new[] { typeof(IQueryable), typeof(string), typeof(object[]) });

                        var strSelect = string.Join(",", ExtraFields);
                        DbSet = selectMethod.Invoke(DbSet, new object[] { DbSet, $"new ({strSelect})", null });

                        var AnonymousValue = FirstOrDefaultMethod.Invoke(DbSet, new object[] { DbSet });

                        var JsonAnonymousValue = JsonConvert.SerializeObject(AnonymousValue);

                        Result = JsonConvert.DeserializeObject(JsonAnonymousValue, type: DynamicEntityType);
                    }
                    else
                    {
                        Result = FirstOrDefaultMethod.Invoke(DbSet, new object[] { DbSet });
                    }
                }
                return new ActionResult<dynamic>()
                {
                    Success = true,
                    Data = Result
                };
            }
            catch (Exception e)
            {
                return new BadRequestResult<dynamic>() { Errors = new List<string>() { e.Message } };
            }
        }

        [SDKExposedMethod]
        public ActionResult<string> ManageUData(List<UObjectDTO> Data)
        {
            try
            {
                if (!Data.Any())
                    throw new Exception("Data is required");

                this._logger.LogInformation($"Manage U data {this.GetType().Name} - Create, Update, Delete");

                var UTableName = GetUTableEntity();
                Type DynamicEntityType = typeof(T).Assembly.GetType(UTableName);

                var DataToAdd = Data.Where(x => x.Action == BLUserActionEnum.Create)
                                    .Select(x => JsonConvert.DeserializeObject($"{x.UObject}", type: DynamicEntityType))
                                    .ToList();
                var DataToUpdate = Data.Where(x => x.Action == BLUserActionEnum.Update)
                                    .Select(x => JsonConvert.DeserializeObject($"{x.UObject}", type: DynamicEntityType))
                                    .ToList();
                var DataToDelete = Data.Where(x => x.Action == BLUserActionEnum.Delete)
                                    .Select(x => JsonConvert.DeserializeObject($"{x.UObject}", type: DynamicEntityType));

                var RowidsToDelete = DataToDelete.Select(x => (int)x.GetType().GetProperty("Rowid").GetValue(x)).ToList();

                int TotalAdded = DataToAdd.Count;
                int TotalUpdated = DataToUpdate.Count;
                int TotalDelete = RowidsToDelete.Count;

                var EntityExpression = Expression.Parameter(DynamicEntityType, DynamicEntityType.Name);

                var RowidColumn = Expression.Property(EntityExpression, "Rowid");

                using (var Context = CreateDbContext())
                {
                    dynamic Table = Context.GetType().GetMethod("Set", types: Type.EmptyTypes).MakeGenericMethod(DynamicEntityType).Invoke(Context, null);

                    var DbSet = Table.AsQueryable();

                    if (TotalAdded > 0)
                    {
                        Context.AddRange(DataToAdd);
                    }

                    if (TotalDelete > 0)
                    {
                        var InExpression = GetInExpression(RowidColumn, RowidsToDelete);
                        var RowsToDelete = GetWhereExpression(DbSet, DynamicEntityType, InExpression, EntityExpression);
                        var ListToDelete = GetDynamicList(RowsToDelete);
                        Context.RemoveRange(ListToDelete);
                    }

                    if (TotalUpdated > 0)
                    {
                        var RowidsToUpdate = DataToUpdate.Select(x => (int)x.GetType().GetProperty("Rowid").GetValue(x)).ToList();

                        var InExpression = GetInExpression(RowidColumn, RowidsToUpdate);
                        var RowsToUpdate = GetWhereExpression(DbSet, DynamicEntityType, InExpression, EntityExpression);
                        var ListToUpdate = GetDynamicList(RowsToUpdate);

                        foreach (var Item in ListToUpdate)
                        {
                            var ObjectUpdated = DataToUpdate.Where(x => (int)x.GetType().GetProperty("Rowid").GetValue(x) == Item.Rowid)
                                                            .First();

                            var RestrictionTypeProperty = ObjectUpdated.GetType().GetProperty("RestrictionType");
                            var AuthorizationTypeProperty = ObjectUpdated.GetType().GetProperty("AuthorizationType");

                            var NewRestrictionType = RestrictionTypeProperty.GetValue(ObjectUpdated);
                            var NewAuthorizationType = AuthorizationTypeProperty.GetValue(ObjectUpdated);

                            RestrictionTypeProperty.SetValue(Item, NewRestrictionType);
                            AuthorizationTypeProperty.SetValue(Item, NewAuthorizationType);
                        }

                    }

                    Context.SaveChanges();
                }

                return new ActionResult<string>()
                {
                    Success = true,
                    Data = $"TotalAdded: {TotalAdded}, TotalUpdated: {TotalUpdated}, TotalDelete: {TotalDelete}"
                };
            }
            catch (Exception e)
            {
                return new BadRequestResult<string>() { Errors = new List<string>() { e.Message } };
            }
        }

        private Expression GetInExpression(Expression ColumNameProperty, List<int> RowidRecords)
        {
            RowidRecords = RowidRecords.Distinct().ToList();

            Type ColumNameType = ColumNameProperty.Type;
            Expression InExpression = Expression.Equal(ColumNameProperty, Expression.Constant(RowidRecords[0], ColumNameType));

            for (int i = 1; i < RowidRecords.Count; i++)
            {
                var OrValueExpression = Expression.Equal(ColumNameProperty, Expression.Constant(RowidRecords[i], ColumNameType));
                InExpression = Expression.Or(InExpression, OrValueExpression);
            }

            return InExpression;
        }

        private IQueryable GetWhereExpression(dynamic Data, Type DynamicEntityType, Expression CoincidenceExpression, ParameterExpression EntityExpression)
        {
            var funcExpression = typeof(Func<,>).MakeGenericType(new Type[] { DynamicEntityType, typeof(bool) });
            var returnExp = Expression.Lambda(funcExpression, CoincidenceExpression, new ParameterExpression[] { EntityExpression });

            var _assemblySelect = typeof(System.Linq.Dynamic.Core.DynamicQueryableExtensions).Assembly;

            var whereMethod = typeof(IQueryable<object>).GetExtensionMethod(_assemblySelect, "Where", new[] { typeof(IQueryable<object>), typeof(LambdaExpression) });

            var whereMethodGeneric = whereMethod.MakeGenericMethod(DynamicEntityType);

            Data = whereMethodGeneric.Invoke(Data, new object[] { Data, returnExp });

            return Data;
        }

        private dynamic GetDynamicList(dynamic Result)
        {
            var _assemblyDynamic = typeof(System.Linq.Dynamic.Core.DynamicEnumerableExtensions).Assembly;
            var dynamicListMethod = typeof(IEnumerable).GetExtensionMethod(_assemblyDynamic, "ToDynamicList", new[] { typeof(IEnumerable) });

            var DynamicList = dynamicListMethod.Invoke(Result, new object[] { Result });

            return DynamicList;
        }

        private bool GetDynamicAny(dynamic Data)
        {
            var _assemblyAny = typeof(System.Linq.Dynamic.Core.DynamicQueryableExtensions).Assembly;

            var AnyMethod = typeof(IQueryable).GetExtensionMethod(_assemblyAny, "Any", new[] { typeof(IQueryable) });

            bool Any = AnyMethod.Invoke(Data, new object[] { Data });

            return Any;
        }

        [SDKExposedMethod]
        public ActionResult<E00250_DynamicEntity> GetDynamicEntity(int rowid){
            using (SDKContext context = CreateDbContext())
            {
                var resource = context.Set<E00250_DynamicEntity>().Where(x => x.Rowid == rowid).FirstOrDefault();
                if (resource != null)
                {
                    return new ActionResult<E00250_DynamicEntity>() { Data = resource };
                }
                else
                {
                    return new NotFoundResult<E00250_DynamicEntity>();
                }
            }
        }

        [SDKExposedMethod]
        public async Task<ActionResult<List<E00250_DynamicEntity>>> GetGroupsDynamicEntity(string blName)
        {
            using (SDKContext context = CreateDbContext())
            {
                var resource = context.Set<E00250_DynamicEntity>().Include(x => x.Feature).Where(x => x.Feature.BusinessName == blName).Select(x => new E00250_DynamicEntity()
                {
                    Rowid = x.Rowid,
                    Tag = x.Tag,
                    Id = x.Id,
                    RowidFeature = x.Feature.Rowid
                }).ToList();
                if (resource != null)
                {
                    return new ActionResult<List<E00250_DynamicEntity>>() { Data = resource };
                }
                else
                {
                    return new NotFoundResult<List<E00250_DynamicEntity>>();
                }
            }
        }

        [SDKExposedMethod]
        public async Task<ActionResult<List<E00251_DynamicEntityColumn>>> GetColumnsDynamicEntity(Int64 rowidDynamicEntity)
        {
            using (SDKContext context = CreateDbContext())
            {
                var resource = context.Set<E00251_DynamicEntityColumn>().Where(x => x.RowidDynamicEntity == rowidDynamicEntity).OrderBy(x => x.Order).ToList();
                if (resource != null)
                {
                    return new ActionResult<List<E00251_DynamicEntityColumn>>() { Data = resource };
                }
                else
                {
                    return new NotFoundResult<List<E00251_DynamicEntityColumn>>();
                }
            }
        }

        [SDKExposedMethod]
        public ActionResult<int> SaveDynamicEntityColumn(E00251_DynamicEntityColumn dynamicEntityColumn)
        {
            try{                
                using(SDKContext context = CreateDbContext()){
                    var entityColumn = context.Set<E00251_DynamicEntityColumn>().Where(x => x.Rowid == dynamicEntityColumn.Rowid).FirstOrDefault();
                    if(entityColumn != null){                        
                        context.Entry(entityColumn).CurrentValues.SetValues(dynamicEntityColumn);                        
                        context.SaveChanges();
                        return new ActionResult<int>() { Success = true, Data = dynamicEntityColumn.Rowid };
                    }else{
                        var LastEntity = context.Set<E00251_DynamicEntityColumn>().OrderByDescending(x => x.Rowid).FirstOrDefault();
                        if(LastEntity == null){
                            dynamicEntityColumn.Rowid = 1;
                        }else{
                            dynamicEntityColumn.Rowid = LastEntity.Rowid + 1;
                        }

                        context.Add(dynamicEntityColumn);
                        context.SaveChanges();
                        return new ActionResult<int>() { Success = true, Data = dynamicEntityColumn.Rowid };
                    }
                }

            }catch (Exception e){
                return new BadRequestResult<int>() { Success = false, Errors = new List<string>() { e.Message } };
            }
        }

        [SDKExposedMethod]
        public ActionResult<int> SaveGroupsDynamicEntity(E00250_DynamicEntity dynamicEntity)
        {
            try{
                using (SDKContext context = CreateDbContext())
                {
                    var resource = context.Set<E00250_DynamicEntity>().Where(x => x.Rowid == dynamicEntity.Rowid).FirstOrDefault();
                    if (resource != null)
                    {
                        resource.Tag = dynamicEntity.Tag;
                        resource.Id = dynamicEntity.Id;
                        resource.Order = dynamicEntity.Order;
                        resource.RowidFeature = dynamicEntity.RowidFeature;
                        resource.IsInternal = dynamicEntity.IsInternal;
                        resource.IsMultiRecord = dynamicEntity.IsMultiRecord;
                        resource.IsOptional = dynamicEntity.IsOptional;
                        resource.IsDisable = dynamicEntity.IsDisable;
                        resource.IsLocked = dynamicEntity.IsLocked;
                        context.SaveChanges();
                        return new ActionResult<int>() { Success=true, Data = resource.Rowid };
                    }
                    else
                    {
                        var LastEntity = context.Set<E00250_DynamicEntity>().OrderByDescending(x => x.Rowid).FirstOrDefault();
                        if(LastEntity == null){
                            dynamicEntity.Rowid = 1;
                        }else{
                            dynamicEntity.Rowid = LastEntity.Rowid + 1;
                        }
                        dynamicEntity.RowidCompanyGroup = AuthenticationService.User.RowidCompanyGroup;
                        context.Add(dynamicEntity);
                        context.SaveChanges();
                        return new ActionResult<int>() { Success = true, Data = dynamicEntity.Rowid };
                    }
                }
            }catch(Exception e){
                return new BadRequestResult<int>() { Success = false, Errors = new List<string>() { e.Message } };
            }

        }

        [SDKExposedMethod]
        public ActionResult<int> DeleteGroupDynamicEntity(int rowid){
            try{
                using (SDKContext context = CreateDbContext())
                {
                    var columns = context.Set<E00251_DynamicEntityColumn>().Where(x => x.RowidDynamicEntity == rowid).ToList();
                    if(columns != null){
                        context.RemoveRange(columns);
                        List<int> rowidsEnityColumn = columns.Select(x => x.Rowid).ToList();
                        DeleteDynamicEntity(context, rowidsEnityColumn);
                    }
                    var resource = context.Set<E00250_DynamicEntity>().Where(x => x.Rowid == rowid).FirstOrDefault();
                    if (resource != null)
                    {
                        context.Remove(resource);
                        context.SaveChanges();
                        return new ActionResult<int>() { Success = true, Data = rowid };
                    }
                    else
                    {
                        return new NotFoundResult<int>();
                    }
                }
            }catch(Exception e){
                return new BadRequestResult<int>() { Success = false, Errors = new List<string>() { e.Message } };
            }
        }
        
        [SDKExposedMethod]
        public ActionResult<SDKResultImportDataDTO> ImportData(string dataStr){
            JArray dataList = JArray.Parse(dataStr);
            List<dynamic> SuccessData = new List<dynamic>();
            List<dynamic> ErrorData = new List<dynamic>();
            foreach (var item in dataList){
                JObject itemObj = (JObject)item;
                dynamic result = CreateDynamicObjectFromJson(typeof(T), itemObj);
                BaseObj = result;
                var resultValidate = ValidateAndSave();
                if(resultValidate.Errors.Count > 0){
                    itemObj.Add("Errors", JToken.FromObject(resultValidate.Errors));
                    ErrorData.Add(itemObj);
                }else{
                    SuccessData.Add(BaseObj.GetRowid());
                }
            }
            SDKResultImportDataDTO resultImport = new SDKResultImportDataDTO();
            resultImport.Success = SuccessData;
            resultImport.Errors = ErrorData;

            return new ActionResult<SDKResultImportDataDTO>{Success = true, Data = resultImport};
        }
        private static T CreateDynamicObjectFromJson(Type type, dynamic dynamicObj)
        {
            dynamic result = Activator.CreateInstance(type);
            
            foreach (var property in dynamicObj.Properties()){
                var propertyName = property.Name;
                var propertyEntity = type.GetProperty(propertyName);
                if(propertyEntity != null){
                    dynamic value;
                    //TODO : Revisar diferentes tipos de datos
                    if(propertyEntity.PropertyType == typeof(bool)){                        
                        if(property.Value == 1){
                            value = true;
                        }else{
                            value = false;
                        }
                    }else{
                        value = Convert.ChangeType(property.Value, propertyEntity.PropertyType);
                    }
                    type.GetProperty(propertyName).SetValue(result, value);
                }
            }
            return (T)result;
        }

    }

}
