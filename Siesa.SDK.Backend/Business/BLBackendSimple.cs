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
        private ILogger _logger;
        protected ILogger Logger { get { return _logger; } }
        protected dynamic _dbFactory;        

        private SDKContext myContext;

        protected SDKContext Context { get { return myContext; } }

        private IEnumerable<INavigation> _navigationProperties = null;

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

        public Shared.Business.LoadResult GetData(int? skip, int? take, string filter = "", string orderBy = "", QueryFilterDelegate<BaseSDK<int>> queryFilter = null, bool includeCount = false, bool includeAttachments = true)
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

            if(retContext == null)
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
        private ILogger _logger;
        protected ILogger Logger { get { return _logger; } }
        protected dynamic _dbFactory;
        protected IFeaturePermissionService _featurePermissionService;

        public string BusinessName { get; set; }
        public T BaseObj { get; set; }

        private string[] _relatedProperties = null;
        private string[] _relatedAttachmentsType = null;
        protected SDKContext ContextMetadata;
        public List<string> RelFieldsToSave { get; set; } = new List<string>();
        private bool CanCreate { get; set; } = true;
        private bool CanEdit { get; set; } = true;
        private IEnumerable<INavigation> _navigationProperties = null;

        private List<object> unique_indexes = new List<object>();

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
            if(typeContext != null){
                _navigationProperties = ContextMetadata.Model.FindEntityType(typeof(T)).GetNavigations().Where(p => p.IsOnDependent);
            }else{
                _navigationProperties = new List<INavigation>();
            };

            AuthenticationService = (IAuthenticationService)_provider.GetService(typeof(IAuthenticationService));

            _backendRouterService = (IBackendRouterService)_provider.GetService(typeof(IBackendRouterService));
            _featurePermissionService = (IFeaturePermissionService)_provider.GetService(typeof(IFeaturePermissionService));

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
                    T currentObject =null;   
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

                        if(currentObject != null){
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

            if (extraFields != null && extraFields.Count > 0)
            {
                extraFields.Add("Rowid");

                var selectedFields = string.Join(",", extraFields.Select(x =>
                {
                    var splitInclude = x.Split('.');
                    if (splitInclude.Length > 1) 
                    {
                        for (int i = 1; i <= splitInclude.Length; i++)
                        {
                            var include = string.Join(".", splitInclude.Take(i));
                            query = query.Include(include);
                        }
                    }
                    return splitInclude[0];
                }).Distinct());

                query = query.Select<T>($"new ({selectedFields})");

            }
            else
            {
                foreach (var relatedProperty in _relatedProperties)
                {
                    query = query.Include(relatedProperty);
                }
            }

            query = query.Where("Rowid == @0", ConvertToRowidType(rowid));

            return query.FirstOrDefault();
        }
    }
        
        public virtual ValidateAndSaveBusinessObjResponse ValidateAndSave()
        {
            ValidateAndSaveBusinessObjResponse result = new();
            if(_featurePermissionService != null && !string.IsNullOrEmpty(BusinessName)){
                CanCreate = _featurePermissionService.CheckUserActionPermission(BusinessName, 1,AuthenticationService);
                CanEdit = _featurePermissionService.CheckUserActionPermission(BusinessName, 2,AuthenticationService);
            }
            if(!CanCreate && !CanEdit){
                AddMessageToResult("Custom.Generic.Unauthorized", result);
                return result;
            }

            try
            {

                Validate(ref result);

                if (result.Errors.Count > 0)
                {
                    return result;
                }

                result.Rowid = Save();
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

        private Int64 Save()
        {
            this._logger.LogInformation($"Save {this.GetType().Name}");
            using (SDKContext context = CreateDbContext())
            {
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
                    DisableRelatedProperties(BaseObj, _navigationProperties);
                    context.SetProvider(_provider);
                    context.Set<T>().Remove(BaseObj);
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                this._logger.LogError(e, $"Error deleting {this.GetType().Name}");
                response.Errors.Add(new OperationError() { Message = e.Message });
            }

            return response;
        }

        public virtual IQueryable<T> EntityFieldFilters(IQueryable<T> query)
        {
            //check if has field Status
            try{
                var statusProperty = BaseObj.GetType().GetProperty("Status");
                //check if status is a enumStatusBaseMaster 
                if (statusProperty != null && statusProperty.PropertyType == typeof(enumStatusBaseMaster))
                {
                    query = query.Where("Status == @0", enumStatusBaseMaster.Active);
                }
            }catch(Exception e){
                this._logger.LogError(e, $"Error checking status property {this.GetType().Name}");
            }
            return query;
        }

        public virtual Siesa.SDK.Shared.Business.LoadResult EntityFieldSearch(string searchText, string prefilters = "", int? top = null, string orderBy = "")
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
            if (top.HasValue){
                take = top.Value;
            }
            return this.GetData(0, take, filter, orderBy, filterDelegate, includeAttachments: false);
        }

        [SDKExposedMethod]
        public async Task<ActionResult<List<dynamic>>> GetDataWithTop(string filter = ""){
            var result = new List<dynamic>();
            using (SDKContext context = CreateDbContext())
            {
                context.SetProvider(_provider);
                IQueryable query = context.Set<T>();
                if(!string.IsNullOrEmpty(filter)){
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

        public virtual Siesa.SDK.Shared.Business.LoadResult GetData(int? skip, int? take, string filter = "", string orderBy = "", QueryFilterDelegate<T> queryFilter = null, bool includeCount = false, bool includeAttachments = true)
        {
            this._logger.LogInformation($"Get Data {this.GetType().Name}");
            var result = new Siesa.SDK.Shared.Business.LoadResult();
            using (SDKContext context = CreateDbContext())
            {
                context.SetProvider(_provider);
                var query = context.Set<T>().AsQueryable();
                foreach (var relatedProperty in _relatedProperties)
                {
                    if(!includeAttachments && _relatedAttachmentsType != null && _relatedAttachmentsType.Contains(relatedProperty))
                    {
                        continue;
                    }
                    query = query.Include(relatedProperty);
                }

                if (!string.IsNullOrEmpty(filter))
                {
                    query = query.Where(filter);
                }
                var total = 0;
                if(includeCount){
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
                //data
                result.Data = query.ToList();
            }
            return result;
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

            if(retContext == null)
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
        public ActionResult<long> SaveAttachmentEntity(dynamic BaseObj){
            this.BaseObj = BaseObj;
            var result = this.ValidateAndSave();
			if(result.Errors.Count == 0){
				var response = result.Rowid;
				return new ActionResult<long>{Success = true, Data = response};
			}else {
				return new BadRequestResult<long>{Success = false, Errors = new List<string> { result.Errors[0].Message }};
			}
            return null;
        }
        
        [SDKExposedMethod]
        public async Task<ActionResult<SDKFileUploadDTO>> SaveFile(byte[] fileBytes, string name){
            MemoryStream stream = new MemoryStream(fileBytes);
            IFormFile file = new FormFile(stream, 0, fileBytes.Length, name, name);
            var result = new SDKFileUploadDTO();
            IWebHostEnvironment env = _provider.GetRequiredService<IWebHostEnvironment>();
            var untrustedFileName = file.FileName;
            try{
                var guid = Guid.NewGuid().ToString();
                untrustedFileName = string.Concat(guid.Substring(1,10), "_", untrustedFileName);
                var path = Path.Combine(env.ContentRootPath,"Uploads");
                Directory.CreateDirectory(path);
                var filePath = Path.Combine(path, untrustedFileName);
                await using FileStream fs = new(filePath, FileMode.Create);
                await file.CopyToAsync(fs);
                result.Url = filePath;
                result.FileName = untrustedFileName;
            }
            catch (IOException ex){
                return new BadRequestResult<SDKFileUploadDTO>{Success = false, Errors = new List<string> { ex.Message }};
            }
            return new ActionResult<SDKFileUploadDTO>{Success = true, Data = result};
        }

        [SDKExposedMethod]
        public async Task<ActionResult<string>> DownloadFile(string url){
            IWebHostEnvironment env = _provider.GetRequiredService<IWebHostEnvironment>();
            var filePath = Path.Combine(url);
            var file = new FileInfo(filePath);
            if (file.Exists){
                var fileBytes = await File.ReadAllBytesAsync(filePath);
                var base64 = Convert.ToBase64String(fileBytes);
                return new ActionResult<string>{Success = true, Data = base64};
            }
            return new BadRequestResult<string>{Success = false, Errors = new List<string> { "File not found" }};
        }

        [SDKExposedMethod]
        public async Task<ActionResult<SDKFileFieldDTO>> DownloadFileByRowid(Int32 rowid){
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
                    FileType = data.FileType
                };
            }else{
                var errors = JsonConvert.DeserializeObject<List<string>> (response.Errors.ToString());
                throw new ArgumentException(errors[0]);
            }
            IWebHostEnvironment env = _provider.GetRequiredService<IWebHostEnvironment>();
            var filePath = Path.Combine(SDKFileField.Url);
            var file = new FileInfo(filePath);
            if (file.Exists){
                var fileBytes = await File.ReadAllBytesAsync(filePath);
                var base64 = Convert.ToBase64String(fileBytes);
                SDKFileField.FileBase64 = base64;
                return new ActionResult<SDKFileFieldDTO>{Success = true, Data = SDKFileField};
            }
            return new BadRequestResult<SDKFileFieldDTO>{Success = false, Errors = new List<string> { "File not found" }};
        }
        
        [SDKExposedMethod]
        public async Task<ActionResult<T>> DataEntity(object rowid){
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
    }

}
