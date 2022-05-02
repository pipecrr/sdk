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
using Siesa.SDK.Shared.Utilities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;

namespace Siesa.SDK.Business
{
    public class BLBackendSimple : IBLBase<BaseSDK<int>>
    {
        [JsonIgnore]
        private IAuthenticationService AuthenticationService { get; set; }

        public BLBackendSimple(IAuthenticationService authenticationService)
        {
            AuthenticationService = authenticationService;
        }

        public string BusinessName { get; set; }
        public BaseSDK<int> BaseObj { get; set; }

        public Int64 Delete()
        {
            return 0;
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
    public class BLBackendSimple<T, K> : IBLBase<T> where T : class, IBaseSDK where K : BLBaseValidator<T>
    {
        [JsonIgnore]
        private IAuthenticationService AuthenticationService { get; set; }
        private IServiceProvider _provider;
        private ILogger _logger;
        protected ILogger Logger { get { return _logger; } }
        protected dynamic _dbFactory;

        public string BusinessName { get; set; }
        public T BaseObj { get; set; }

        private string[] _relatedProperties = null;

        private SDKContext myContext;
        protected SDKContext Context { get { return myContext; } }

        public List<string> RelFieldsToSave { get; set; } = new List<string>();

        private IEnumerable<INavigation> _navigationProperties = null;

        public void DetachedBaseObj()
        {
            //TODO: Complete
            //myContext.Entry(BaseObj).State = EntityState.Detached;
            //BaseObj = (T)myContext.Entry(BaseObj).CurrentValues.ToObject();
        }

        public BLBackendSimple(IAuthenticationService authenticationService)
        {
            AuthenticationService = authenticationService;
            BaseObj = Activator.CreateInstance<T>();
            _relatedProperties = BaseObj.GetType().GetProperties().Where(p => p.PropertyType.IsClass && !p.PropertyType.IsPrimitive && !p.PropertyType.IsEnum && p.PropertyType != typeof(string) && p.Name != "RowVersion").Select(p => p.Name).ToArray();


        }

        public void ShareProvider(dynamic bl)
        {
            bl.SetProvider(_provider);
        }

        public void SetProvider(IServiceProvider provider)
        {
            _provider = provider;

            _dbFactory = _provider.GetService(typeof(IDbContextFactory<SDKContext>));

            ILoggerFactory loggerFactory = (ILoggerFactory)_provider.GetService(typeof(ILoggerFactory));
            _logger = loggerFactory.CreateLogger(this.GetType().FullName);

            myContext = _dbFactory.CreateDbContext();
            myContext.SetProvider(_provider);

            _navigationProperties = myContext.Model.FindEntityType(typeof(T)).GetNavigations().Where(p => p.IsOnDependent);
            AuthenticationService = (IAuthenticationService)_provider.GetService(typeof(IAuthenticationService));
        }

        public virtual T Get(Int64 rowid)
        {
            using (SDKContext context = _dbFactory.CreateDbContext())
            {
                context.SetProvider(_provider);
                var query = context.Set<T>().AsQueryable();
                foreach (var relatedProperty in _relatedProperties)
                {
                    query = query.Include(relatedProperty);
                }
                query = query.Where("Rowid == @0", rowid);
                return query.FirstOrDefault();
            }
        }

        public virtual ValidateAndSaveBusinessObjResponse ValidateAndSave()
        {
            ValidateAndSaveBusinessObjResponse result = new();

            try
            {

                Validate(ref result);
                ValidateIndex(ref result);

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
            var message = BackendExceptionManager.ExceptionToString(exception);
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
            ValidateBussines(ref baseOperation);
            //K validator = Activator.CreateInstance<K>();
            K validator = ActivatorUtilities.CreateInstance(_provider, typeof(K)) as K;
            validator.ValidatorType = "Entity";
            SDKValidator.Validate<T>(BaseObj, validator, ref baseOperation);
        }

        private void ValidateIndex(ref ValidateAndSaveBusinessObjResponse baseOperation)
        {
            //TODO: Refactorizar, soportar las 2 formas de definir los indices 
            var attrs_types = System.Attribute.GetCustomAttributes(BaseObj.GetType());
            var attrs = System.Attribute.GetCustomAttributes(BaseObj.GetType()).ToList()
                .Where(x => x is IndexAttribute)
                .Where(x => ((IndexAttribute)x).IsUnique);

            foreach (var attr in attrs)
            {

                var dictionaryByIndex = GetPropertiesToIndex((IndexAttribute)attr);

                var primaryKey = GetPrimaryKey();

                if (ExistsRowByIndex(Utilities.GetDinamycWhere(dictionaryByIndex, primaryKey)))
                {
                    baseOperation.Errors.Add(new OperationError
                    {
                        Attribute = (dictionaryByIndex.Count > 1) ? string.Join("", dictionaryByIndex) : dictionaryByIndex.First().Key,
                        Message = (dictionaryByIndex.Count > 1) ? string.Join("Los campos ", dictionaryByIndex, " deben ser unicos") : "El campo " + dictionaryByIndex.First().Key + " debe ser unico"
                    });
                }
            }
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

                var valueProp = (int)BaseObj.GetType().GetProperty(property.Name).GetValue(BaseObj);
                returnValue.Add(property.Name, (valueProp == 0) ? null : valueProp.ToString());
            }

            return returnValue;
        }
        private Dictionary<string, object> GetPropertiesToIndex(IndexAttribute index)
        {

            Dictionary<string, object> returnValue = new Dictionary<string, object>();
            Dictionary<string, object> collectionFks = new Dictionary<string, object>();

            var propNames = index.PropertyNames;

            foreach (var propName in propNames)
            {
                PropertyInfo prop = BaseObj.GetType().GetProperty(propName);
                var valueProp = prop.GetValue(BaseObj);

                // Filtra solo los custom attributes que sean de tipo foraneo y que coincidan con el nombre del PropName  
                var properties = BaseObj.GetType().GetProperties()
                                                        .Where(x => (x.GetCustomAttributes()
                                                                    .Where(x => x.GetType() == typeof(ForeignKeyAttribute)
                                                                                && ((ForeignKeyAttribute)x).Name == propName
                                                                          ).ToList().Count > 0
                                                                  )
                                                              );

                var property = properties.FirstOrDefault();

                if (property is not null)
                {

                    var bodyValue = BaseObj.GetType().GetProperty(property.Name).GetValue(BaseObj);
                    var entityValueRowid = bodyValue.GetType().GetProperty("Rowid").GetValue(bodyValue);

                    valueProp = entityValueRowid;
                }

                returnValue.Add(propName, valueProp.ToString());
            }

            return returnValue;
        }

        private bool ExistsRowByIndex(string filter)
        {
            using (SDKContext context = _dbFactory.CreateDbContext())
            {
                context.SetProvider(_provider);
                var query = context.Set<T>().AsQueryable();

                foreach (var relatedProperty in _relatedProperties)
                {
                    query = query.Include(relatedProperty);
                }

                query = query.Where(filter);

                return query.Count() > 0;
            }
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
                                var relNavigations = myContext.Model.FindEntityType(fkFieldValue.GetType()).GetNavigations().Where(p => p.IsOnDependent);
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
            using (SDKContext context = _dbFactory.CreateDbContext())
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
                    foreach (var relatedProperty in _relatedProperties)
                    {
                        query = query.Include(relatedProperty);
                    }
                    query = query.Where("Rowid == @0", BaseObj.GetRowid());
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

        public virtual Int64 Delete()
        {
            using (SDKContext context = _dbFactory.CreateDbContext())
            {
                context.SetProvider(_provider);
                context.Set<T>().Remove(BaseObj);
                context.SaveChanges();
            }
            return 0;
        }

        public virtual IQueryable<T> EntityFieldFilters(IQueryable<T> query)
        {
            return query;
        }

        public virtual Siesa.SDK.Shared.Business.LoadResult EntityFieldSearch(string searchText, string prefilters = "")
        {
            var string_fields = BaseObj.GetType().GetProperties().Where(p => p.PropertyType == typeof(string)).Select(p => p.Name).ToArray();
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
            return this.GetData(0, 100, filter, "", filterDelegate);
        }

        public virtual Siesa.SDK.Shared.Business.LoadResult GetData(int? skip, int? take, string filter = "", string orderBy = "", QueryFilterDelegate<T> queryFilter = null)
        {
            var result = new Siesa.SDK.Shared.Business.LoadResult();
            using (SDKContext context = _dbFactory.CreateDbContext())
            {
                context.SetProvider(_provider);
                var query = context.Set<T>().AsQueryable();
                foreach (var relatedProperty in _relatedProperties)
                {
                    query = query.Include(relatedProperty);
                }

                if (!string.IsNullOrEmpty(filter))
                {
                    query = query.Where(filter);
                }
                var total = query.Count();

                if (skip.HasValue)
                {
                    query = query.Skip(skip.Value);
                }
                if (take.HasValue)
                {
                    query = query.Take(take.Value);
                }


                if (!string.IsNullOrEmpty(orderBy))
                {
                    query = query.OrderBy(orderBy);
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

        public Task<T> GetAsync(Int64 rowid)
        {
            throw new NotImplementedException();
        }
        protected virtual void ValidateBussines(ref ValidateAndSaveBusinessObjResponse operationResult)
        {
            // Do nothing
        }
    }

}
