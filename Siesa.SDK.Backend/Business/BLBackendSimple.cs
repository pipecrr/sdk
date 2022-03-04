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

namespace Siesa.SDK.Business
{
    public class BLBackendSimple : IBLBase<BaseEntity>
    {
        [JsonIgnore]
        private IAuthenticationService AuthenticationService {get; set;}

        public BLBackendSimple(IAuthenticationService authenticationService)
        {
            AuthenticationService = authenticationService;
        }
        
        public string BusinessName { get;set; }
        public BaseEntity BaseObj { get;set; }

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

        public Shared.Business.LoadResult GetData(int? skip, int? take, string filter = "", string orderBy = "")
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
        private IAuthenticationService AuthenticationService {get; set;}        
        private IServiceProvider _provider;
        private ILogger _logger;
        protected ILogger Logger { get { return _logger; } }
        protected dynamic _dbFactory;

        public string BusinessName { get; set; }
        public T BaseObj { get; set; }

        private string[] _relatedProperties = null;

        private SDKContext myContext;
        protected SDKContext Context { get { return myContext; } }

        public List<string> RelFieldsToSave {get;set;} = new List<string>();

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
            catch(DbUpdateException exception)
            {
                exception.Data.Add("Entity:","entityName");
                AddExceptionToResult(exception, result);
                _logger.LogError(exception, "Error saving in BLBackend");
                _logger.LogError("Text information");
            }
            catch (Exception exception)
            {
                AddExceptionToResult(exception,result);
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

        private int Save(SDKContext context)
        {
            if (BaseObj.Rowid == 0)
            {
                foreach (var relatedProperty in _relatedProperties)
                {
                    var entityValue = BaseObj.GetType().GetProperty(relatedProperty).GetValue(BaseObj);
                    
                    if (entityValue != null)
                    {
                        var entityValueRowid = (int)entityValue.GetType().GetProperty("Rowid").GetValue(entityValue);
                        if (entityValueRowid != 0) {
                            context.Entry(entityValue).State = EntityState.Unchanged;
                        }
                    }
                }

                var entry = context.Add<T>(BaseObj);
            }
            else
            {
                //demo borrar
                //get by rowid
                T entity = context.Set<T>().Find(BaseObj.Rowid);
                //context.Entry(entity).OriginalValues["RowVersion"] = BaseObj.RowVersion;
                context.ResetConcurrencyValues(entity, BaseObj);
                context.Entry(entity).CurrentValues.SetValues(BaseObj);
                

                //Loop through all foreign keys and set the values
                foreach (var relatedProperty in _relatedProperties)
                {
                    var bodyValue = BaseObj.GetType().GetProperty(relatedProperty).GetValue(BaseObj);
                    context.Entry(entity).Reference(relatedProperty).CurrentValue = bodyValue;
                    //Does not save the related object
                    if (bodyValue != null) {
                        var entityValueRowid = (int)bodyValue.GetType().GetProperty("Rowid").GetValue(bodyValue);
                        if (entityValueRowid != 0 && !RelFieldsToSave.Contains(relatedProperty)) {
                            context.Entry(bodyValue).State = EntityState.Unchanged;
                        }
                    }
                }

                //set updated values
                entity.LastUpdateDate = DateTime.Now;
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

        public virtual Siesa.SDK.Shared.Business.LoadResult EntityFieldSearch(string searchText)
        {
            //TODO: Define the search logic using the search text and the entity properties 

            var string_fields = BaseObj.GetType().GetProperties().Where(p => p.PropertyType == typeof(string)).Select(p => p.Name).ToArray();
            var filter = "";
            foreach (var field in string_fields)
            {
                if(!string.IsNullOrEmpty(filter))
                {
                    filter += " || ";
                }
                //"(Name == null ? \"\" : Name).ToLower().Contains(\"tole\".ToLower())"
                filter += $"({field} == null ? \"\" : {field}).ToLower().Contains(\"{searchText}\".ToLower())";
            }
            return this.GetData(0, 100, filter);
        }

        public virtual Siesa.SDK.Shared.Business.LoadResult GetData(int? skip, int? take, string filter = "", string orderBy = "")
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

                if(!string.IsNullOrEmpty(filter))
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
