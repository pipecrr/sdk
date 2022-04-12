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

namespace Siesa.SDK.Business
{
    public class BLBackendSimple : IBLBase<BaseEntity>
    {
        [JsonIgnore]
        private IAuthenticationService AuthenticationService { get; set; }

        public BLBackendSimple(IAuthenticationService authenticationService)
        {
            AuthenticationService = authenticationService;
        }

        public string BusinessName { get; set; }
        public BaseEntity BaseObj { get; set; }

        public int Delete()
        {
            return 0;
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
    public class BLBackendSimple<T, K> : IBLBase<T> where T : BaseEntity where K : BLBaseValidator<T>
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

            _navigationProperties = myContext.Model.FindEntityType(typeof(T)).GetNavigations();
            AuthenticationService = (IAuthenticationService)_provider.GetService(typeof(IAuthenticationService));
        }

        public virtual T Get(int rowid)
        {
            var context = myContext;
            //var query = context.Set<T>().AsQueryable();
            var query = context.Set<T>().Where(x => x.Rowid == rowid).AsQueryable();
            foreach (var relatedProperty in _relatedProperties)
            {
                query = query.Include(relatedProperty);
            }

            return query.FirstOrDefault();



        }

        public virtual ValidateAndSaveBusinessObjResponse ValidateAndSave()
        {
            ValidateAndSaveBusinessObjResponse result = new();

            try
            {

                Validate(ref result);

                if (result.Errors.Count > 0)
                {
                    return result;
                }

                using (SDKContext context = _dbFactory.CreateDbContext())
                {
                    context.SetProvider(_provider);
                    result.Rowid = Save(context);
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
            K validator = Activator.CreateInstance<K>();
            validator.ValidatorType = "Entity";
            SDKValidator.Validate<T>(BaseObj, validator, ref baseOperation);
        }

        private void DisableRelatedProperties(object obj, IEnumerable<INavigation> relatedProperties , List<string> propertiesToKeep = null)
        {
            //TODO: Probar el desvinculado de las propiedades relacionadas
            if (relatedProperties != null)
            {
                foreach (var navProperty in relatedProperties)
                {
                    var foreignKey = navProperty.ForeignKey;
                    var fkProperties = foreignKey.Properties;
                    var principalProperties = foreignKey.PrincipalKey.Properties;
                    if(foreignKey.DependentToPrincipal != null){
                        var fkFieldName = foreignKey.DependentToPrincipal.Name;
                        var fkFieldValue = obj.GetType().GetProperty(fkFieldName).GetValue(obj);
                        if (fkFieldValue !=  null)
                        {
                            if (propertiesToKeep != null && propertiesToKeep.Contains(fkFieldName))
                            {
                                var relNavigations = myContext.Model.FindEntityType(fkFieldValue.GetType()).GetNavigations();
                                DisableRelatedProperties(fkFieldValue, relNavigations);
                                continue;
                            }
                            var principalFieldName = principalProperties[0].Name;
                            var principalFieldValue = fkFieldValue.GetType().GetProperty(principalFieldName).GetValue(fkFieldValue);
                            if(principalFieldValue != null){
                                if((int)principalFieldValue != 0){
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

        private int Save(SDKContext context)
        {
            if (BaseObj.Rowid == 0)
            { 
                DisableRelatedProperties(BaseObj, _navigationProperties, RelFieldsToSave);
                BaseObj.LastUpdateDate = DateTime.Now;
                BaseObj.RowidCreator = AuthenticationService.User.Rowid;
                BaseObj.RowidLastEditUser = AuthenticationService.User.Rowid;
                var entry = context.Add<T>(BaseObj);
            }
            else
            {
                //demo borrar
                //get by rowid
                T entity = context.Set<T>().Where(x => x.Rowid == BaseObj.Rowid).FirstOrDefault();
                //context.Entry(entity).OriginalValues["RowVersion"] = BaseObj.RowVersion;
                context.ResetConcurrencyValues(entity, BaseObj);
                DisableRelatedProperties(BaseObj, _navigationProperties, RelFieldsToSave);
                context.Entry(entity).CurrentValues.SetValues(BaseObj);
                foreach (var relatedProperty in RelFieldsToSave)
                {
                    entity.GetType().GetProperty(relatedProperty).SetValue(entity, BaseObj.GetType().GetProperty(relatedProperty).GetValue(BaseObj));
                }
                //DisableRelatedProperties(entity, _navigationProperties, RelFieldsToSave);
                

                //set updated values
                entity.LastUpdateDate = DateTime.Now;
                entity.RowidLastEditUser = AuthenticationService.User.Rowid;
            }

            context.SaveChanges(); //TODO: Capturar errores db y hacer rollback
            return BaseObj.Rowid;
        }

        public virtual void Update()
        {
            throw new NotImplementedException();
        }

        public virtual int Delete()
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

        public Task<T> GetAsync(int id)
        {
            throw new NotImplementedException();
        }
        protected virtual void ValidateBussines(ref ValidateAndSaveBusinessObjResponse operationResult)
        {
            // Do nothing
        }
    }

}
