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
using Siesa.SDK.Shared.DataAnnotations;
using Siesa.SDK.Backend.Services;
using Siesa.SDK.Shared.DTOS;
using System.Linq.Expressions;

namespace Siesa.SDK.Business
{
    public class BLBackendSimple : IBLBase<BaseSDK<int>>
    {
        [JsonIgnore]
        protected IAuthenticationService AuthenticationService { get; set; }

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

            AuthenticationService = (IAuthenticationService)_provider.GetService(typeof(IAuthenticationService));
        }
    }
    public class BLBackendSimple<T, K> : IBLBase<T> where T : class, IBaseSDK where K : BLBaseValidator<T>
    {
        [JsonIgnore]
        protected IAuthenticationService AuthenticationService { get; set; }

        public SDKBusinessModel GetBackend(string business_name)
        {
            return BackendRouterService.Instance.GetSDKBusinessModel(business_name, AuthenticationService);
        }

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
            var _bannedTypes = new List<Type>() { typeof(string), typeof(byte[]) };
            _relatedProperties = BaseObj.GetType().GetProperties().Where(p => p.PropertyType.IsClass && !p.PropertyType.IsPrimitive && !p.PropertyType.IsEnum && !_bannedTypes.Contains(p.PropertyType) && p.Name != "RowVersion").Select(p => p.Name).ToArray();


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
            var message = BackendExceptionManager.ExceptionToString(exception, Context);
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
                    // foreach (var relatedProperty in _relatedProperties)
                    // {
                    //     query = query.Include(relatedProperty);
                    // }
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

        public virtual DeleteBusinessObjResponse Delete()
        {
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
                using (SDKContext context = _dbFactory.CreateDbContext())
                {
                    context.SetProvider(_provider);
                    context.Set<T>().Remove(BaseObj);
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                //response.Errors.AddRange(result.Errors);

                return null;
            }

            return new DeleteBusinessObjResponse();
        }

        public virtual IQueryable<T> EntityFieldFilters(IQueryable<T> query)
        {
            return query;
        }

        public virtual Siesa.SDK.Shared.Business.LoadResult EntityFieldSearch(string searchText, string prefilters = "")
        {
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
        protected virtual void ValidateBussines(ref ValidateAndSaveBusinessObjResponse operationResult, BLUserActionEnum action)
        {
            // Do nothing
        }

        [SDKExposedMethod]
        public virtual ActionResult<string> GetObjectString(Int64 rowid)
        {
            using (SDKContext context = _dbFactory.CreateDbContext())
            {
                context.SetProvider(_provider);
                var query = context.Set<T>().AsQueryable();
                context.ChangeTracker.LazyLoadingEnabled = true;
                query = query.Where("Rowid == @0", rowid);
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

        [SDKExposedMethod]
        public ActionResult<dynamic> SDKFlexPreviewData(SDKFlexRequestData requestData)
        {

            using (var Context = _dbFactory.CreateDbContext())
            {
                List<SDKFlexColumn> columns = requestData.columns;
                if (columns.Count == 0)
                {
                    return new BadRequestResult<dynamic>();
                }

                string strColumns = "";
                //string strRelatedColumns = "";            
                Dictionary<string, List<SDKFlexColumn>> columnsRelated = new Dictionary<string, List<SDKFlexColumn>>();

                foreach (SDKFlexColumn column in columns)
                {
                    var i = columns.IndexOf(column);
                    if (column.path.Contains("::"))
                    {
                        List<SDKFlexColumn> value = new List<SDKFlexColumn>();
                        if (columnsRelated.TryGetValue(column.path, out value))
                        {
                            value.Add(column);
                            columnsRelated[column.path] = value;
                        }
                        else
                        {
                            columnsRelated.Add(column.path, new List<SDKFlexColumn>() { column });
                        }
                        /*strRelatedColumns += column.name + " as " + column.name.ToLower();
                        if (i != columns.Count - 1)
                        {
                            strRelatedColumns += ", ";
                        }*/
                    }
                    else
                    {
                        strColumns += column.name + " as " + column.name.ToLower();
                        if (i != columns.Count - 1)
                        {
                            strColumns += ", ";
                        }
                    }
                }

                List<SDKFlexFilters> filters = requestData.filters;

                var nameEntity = requestData.selected_class;
                var nameSpaceEntity = requestData.module_path;

                try
                {
                    var entityType = Utilities.SearchType(nameSpaceEntity + "." + nameEntity, true);
                    IQueryable<dynamic> contextSet = Context.GetType().GetMethod("Set", types: Type.EmptyTypes).MakeGenericMethod(entityType).Invoke(Context, null) as IQueryable<dynamic>;

                    createWhere(ref contextSet, filters, entityType);

                    IQueryable select = contextSet
                    .Select($"new ({strColumns})");

                    IQueryable query;
                    if (columnsRelated.Count > 0)
                    {                        
                        query = select;
                    }
                    else
                    {
                        query = select;
                    }

                    List<dynamic> resource = query.Take(50).ToDynamicList();
                    if (resource != null)
                    {
                        return new ActionResult<dynamic>() { Data = resource };
                    }
                }
                catch (Exception e)
                {
                    return new ActionResult<dynamic>() { Success = false, Errors = new List<string>() { "Error al crear la consulta" } };
                }
            }

            return null;
        }

        private Expression GetPropertyExpression(Expression pe, string chain)
        {
            var properties = chain.Split('.');
            foreach (var property in properties)
                pe = Expression.Property(pe, property);

            return pe;
        }

        private void createWhere(ref IQueryable<object> select, List<SDKFlexFilters> filters, Type entityType)
        {
            Expression combined = null;
            ParameterExpression pe = Expression.Parameter(entityType, entityType.Name);

            foreach (var filter in filters)
            {
                //Expression for accessing Fields name property

                Expression columnNameProperty = GetPropertyExpression(pe, filter.name);
                var columnType = entityType.GetProperty(filter.name).PropertyType;

                Expression columnValue;
                //the name constant to match 
                var value = filter.equal_from;

                if (value == null && (!columnType.IsGenericType || columnType.GetGenericTypeDefinition() != typeof(Nullable<>)))
                {
                    value = Activator.CreateInstance(columnType);
                }

                //TimeOnly y TimeSpan se estan tratando como charField por eso se cambia el tipo del columnType
                if (columnType == typeof(TimeOnly) || columnType == typeof(TimeSpan))
                {
                    var valueSplit = value.ToString().Split(":").ToList();
                    int hours = 0;
                    int minutes = 0;
                    int seconds = 0;
                    if (valueSplit.Count >= 1)
                    {
                        hours = int.Parse(valueSplit[0]);
                    }
                    if (valueSplit.Count >= 2)
                    {
                        minutes = int.Parse(valueSplit[1]);
                    }
                    if (valueSplit.Count == 3)
                    {
                        seconds = int.Parse(valueSplit[2]);
                    }

                    value = Activator.CreateInstance(columnType, hours, minutes, seconds);
                }

                object filterValue;

                switch (filter.selected_operator)
                {

                    case "equal":
                        filterValue = Convert.ChangeType(value, columnType);
                        columnValue = Expression.Constant(filterValue);
                        Expression equalExpression;
                        if (columnType == typeof(DateTime))
                        {
                            var x = filterValue as DateTime?;
                            Expression greatEqualE = Expression.GreaterThanOrEqual(columnNameProperty, columnValue);
                            Expression lessEqualE = Expression.LessThan(columnNameProperty, Expression.Constant(x?.AddMinutes(1)));
                            equalExpression = Expression.And(lessEqualE, greatEqualE);
                            addExpression(ref combined, equalExpression);
                        }
                        else
                        {
                            equalExpression = Expression.Equal(columnNameProperty, columnValue);
                        }
                        addExpression(ref combined, equalExpression);
                        break;
                    case "not_equal":
                        filterValue = Convert.ChangeType(value, columnType);
                        columnValue = Expression.Constant(filterValue);
                        Expression notEqualExpression;
                        if (columnType == typeof(DateTime))
                        {
                            var xx = filterValue as DateTime?;
                            Expression lessNotEqualE = Expression.LessThan(columnNameProperty, columnValue);
                            Expression greatNotEqualE = Expression.GreaterThan(columnNameProperty, Expression.Constant(xx?.AddMinutes(1)));
                            notEqualExpression = Expression.Or(lessNotEqualE, greatNotEqualE);
                        }
                        else
                        {
                            notEqualExpression = Expression.NotEqual(columnNameProperty, columnValue);
                        }
                        addExpression(ref combined, notEqualExpression);
                        break;
                    case "starts_with":
                        filterValue = Convert.ChangeType(value, columnType);
                        columnValue = Expression.Constant(filterValue);
                        Expression startsWithExpression = Expression.Call(columnNameProperty, getMethodInfo("StartsWith", typeof(string)), columnValue);
                        addExpression(ref combined, startsWithExpression);
                        break;
                    case "end_with":
                        filterValue = Convert.ChangeType(value, columnType);
                        columnValue = Expression.Constant(filterValue);
                        Expression endWithExpression = Expression.Call(columnNameProperty, getMethodInfo("EndsWith", typeof(string)), columnValue);
                        addExpression(ref combined, endWithExpression);
                        break;
                    case "contains":
                        filterValue = Convert.ChangeType(value, columnType);
                        columnValue = Expression.Constant(filterValue);
                        Expression containsExpression = Expression.Call(columnNameProperty, getMethodInfo("Contains", typeof(string)), columnValue);
                        addExpression(ref combined, containsExpression);
                        break;
                    case "null_or_empty":
                    case "empty":
                        Expression nullOrEmptyExpression;
                        if (columnType == typeof(DateTime))
                        {
                            filterValue = Convert.ChangeType(Activator.CreateInstance(columnType), columnType);
                            columnValue = Expression.Constant(filterValue);
                            nullOrEmptyExpression = Expression.Equal(columnNameProperty, columnValue);
                        }
                        else
                        {
                            nullOrEmptyExpression = Expression.Call(getMethodInfo("IsNullOrEmpty", typeof(string)), columnNameProperty);
                        }
                        addExpression(ref combined, nullOrEmptyExpression);
                        break;
                    case "not_empty":
                        Expression notEmptyExpression;
                        if (columnType == typeof(DateTime))
                        {
                            filterValue = Convert.ChangeType(Activator.CreateInstance(columnType), columnType);
                            columnValue = Expression.Constant(filterValue);
                            notEmptyExpression = Expression.Equal(columnNameProperty, columnValue);
                        }
                        else
                        {
                            notEmptyExpression = Expression.Call(getMethodInfo("IsNullOrEmpty", typeof(string)), columnNameProperty);
                        }
                        notEmptyExpression = Expression.Not(notEmptyExpression);
                        addExpression(ref combined, notEmptyExpression);
                        break;
                    case "gt":
                        filterValue = Convert.ChangeType(value, columnType);
                        columnValue = Expression.Constant(filterValue);
                        Expression greatThanExpression = Expression.GreaterThan(columnNameProperty, columnValue);
                        addExpression(ref combined, greatThanExpression);
                        break;
                    case "lt":
                        filterValue = Convert.ChangeType(value, columnType);
                        columnValue = Expression.Constant(filterValue);
                        Expression lessThanExpression = Expression.LessThan(columnNameProperty, columnValue);
                        addExpression(ref combined, lessThanExpression);
                        break;
                    case "gte":
                        filterValue = Convert.ChangeType(value, columnType);
                        columnValue = Expression.Constant(filterValue);
                        Expression greatThanOrEqualsExpression = Expression.GreaterThanOrEqual(columnNameProperty, columnValue);
                        addExpression(ref combined, greatThanOrEqualsExpression);
                        break;
                    case "between":
                        filterValue = Convert.ChangeType(value, columnType);
                        columnValue = Expression.Constant(filterValue);
                        Expression columnTo = Expression.Constant(Convert.ChangeType(filter.to, columnType));
                        Expression greaterExpression = Expression.GreaterThan(columnNameProperty, columnValue);
                        Expression lessExpression = Expression.LessThan(columnNameProperty, columnTo);
                        Expression betweenExpression = Expression.And(lessExpression, greaterExpression);
                        addExpression(ref combined, betweenExpression);
                        break;
                    case "before":
                        filterValue = Convert.ChangeType(value, columnType);
                        columnValue = Expression.Constant(filterValue);
                        Expression beforeExpression = Expression.LessThan(columnNameProperty, columnValue);
                        addExpression(ref combined, beforeExpression);
                        break;
                    case "after":
                        filterValue = Convert.ChangeType(value, columnType);
                        columnValue = Expression.Constant(filterValue);
                        Expression afterExpression = Expression.GreaterThan(columnNameProperty, columnValue);
                        addExpression(ref combined, afterExpression);
                        break;
                    case "in_past":
                        columnValue = Expression.Constant(Convert.ChangeType(DateTime.Now, columnType));
                        Expression inPastExpression = Expression.LessThan(columnNameProperty, columnValue);
                        addExpression(ref combined, inPastExpression);
                        break;
                    case "in_future":
                        columnValue = Expression.Constant(Convert.ChangeType(DateTime.Now, columnType));
                        Expression inFuctureExpression = Expression.GreaterThan(columnNameProperty, columnValue);
                        addExpression(ref combined, inFuctureExpression);
                        break;
                    case "current_month":
                        DateTime todayCurrentMonth = DateTime.Now;
                        var month = todayCurrentMonth.Month;
                        var year = todayCurrentMonth.Year;
                        var numDays = DateTime.DaysInMonth(year, month);
                        var startDateMonth = new DateTime(year, month, 1);
                        var endDateMonth = new DateTime(year, month, numDays);

                        columnValue = Expression.Constant(Convert.ChangeType(startDateMonth, typeof(DateTime)));
                        Expression greaterCurrentMonthE = Expression.GreaterThanOrEqual(columnNameProperty, columnValue);

                        columnValue = Expression.Constant(Convert.ChangeType(endDateMonth, typeof(DateTime)));
                        Expression lessCurrentMonthE = Expression.LessThanOrEqual(columnNameProperty, columnValue);

                        var currentMonthExpression = Expression.And(lessCurrentMonthE, greaterCurrentMonthE);
                        addExpression(ref combined, currentMonthExpression);
                        break;
                    case "current_week":
                        DateTime todayCurrentWeek = DateTime.Now;
                        var dayWeek = ((double)todayCurrentWeek.DayOfWeek);
                        var startDayWeek = todayCurrentWeek.AddDays(-dayWeek);
                        var endDateWeek = todayCurrentWeek.AddDays(6 - dayWeek);

                        columnValue = Expression.Constant(Convert.ChangeType(startDayWeek, typeof(DateTime)));
                        Expression greaterCurrentWeekE = Expression.GreaterThanOrEqual(columnNameProperty, columnValue);

                        columnValue = Expression.Constant(Convert.ChangeType(endDateWeek, typeof(DateTime)));
                        Expression lessCurrentWeekE = Expression.LessThanOrEqual(columnNameProperty, columnValue);

                        var currentWeekExpression = Expression.And(lessCurrentWeekE, greaterCurrentWeekE);
                        addExpression(ref combined, currentWeekExpression);
                        break;
                    case "last_month":
                        DateTime todayLastMonth = DateTime.Now;
                        var lastMonth = todayLastMonth.Month - 1;
                        var lastYear = todayLastMonth.Year;

                        var startDateLastMonth = new DateTime();
                        var endDateLastMonth = new DateTime();
                        if (lastMonth <= 0)
                        {
                            startDateLastMonth = new DateTime(lastYear - 1, 12, 1);
                            endDateLastMonth = new DateTime(lastYear - 1, 12, 31);
                        }
                        else
                        {
                            var numDaysLastMonth = DateTime.DaysInMonth(lastYear, lastMonth);
                            startDateLastMonth = new DateTime(lastYear, lastMonth, 1);
                            endDateLastMonth = new DateTime(lastYear, lastMonth, numDaysLastMonth);
                        }

                        columnValue = Expression.Constant(Convert.ChangeType(startDateLastMonth, typeof(DateTime)));
                        Expression greaterLastMonthE = Expression.GreaterThanOrEqual(columnNameProperty, columnValue);

                        columnValue = Expression.Constant(Convert.ChangeType(endDateLastMonth, typeof(DateTime)));
                        Expression lessLastMonthE = Expression.LessThanOrEqual(columnNameProperty, columnValue);

                        var lastMonthExpression = Expression.And(lessLastMonthE, greaterLastMonthE);
                        addExpression(ref combined, lastMonthExpression);
                        break;
                    case "today":
                        DateTime today = DateTime.Now;

                        var startDate = today.Date;
                        var endDate = startDate.AddDays(1);

                        columnValue = Expression.Constant(Convert.ChangeType(startDate, typeof(DateTime)));
                        Expression greaterTodayE = Expression.GreaterThan(columnNameProperty, columnValue);

                        columnValue = Expression.Constant(Convert.ChangeType(endDate, typeof(DateTime)));
                        Expression lessTodayE = Expression.LessThan(columnNameProperty, columnValue);

                        var todayExpression = Expression.And(lessTodayE, greaterTodayE);
                        addExpression(ref combined, todayExpression);
                        break;
                    case "last_n_days":
                        DateTime todayLastNDate = DateTime.Now;
                        int daysLast = int.Parse(value.ToString());
                        var startDateLastnDays = todayLastNDate.Date.AddDays(-daysLast);
                        var endDateLastnDays = todayLastNDate;

                        columnValue = Expression.Constant(Convert.ChangeType(startDateLastnDays, typeof(DateTime)));
                        Expression greaterLastnDayE = Expression.GreaterThan(columnNameProperty, columnValue);

                        columnValue = Expression.Constant(Convert.ChangeType(endDateLastnDays, typeof(DateTime)));
                        Expression lessLastnDayE = Expression.LessThan(columnNameProperty, columnValue);

                        var lastnDaysExpression = Expression.And(lessLastnDayE, greaterLastnDayE);
                        addExpression(ref combined, lastnDaysExpression);
                        break;
                    case "next_n_days":
                        DateTime todayNextNDate = DateTime.Now;
                        int daysNext = int.Parse(value.ToString());
                        var startDateNextnDays = todayNextNDate.Date.AddDays(daysNext);
                        var endDateNextnDays = todayNextNDate;

                        columnValue = Expression.Constant(Convert.ChangeType(startDateNextnDays, typeof(DateTime)));
                        Expression greaterNextnDayE = Expression.GreaterThan(columnNameProperty, columnValue);

                        columnValue = Expression.Constant(Convert.ChangeType(endDateNextnDays, typeof(DateTime)));
                        Expression lessNextnDayE = Expression.LessThan(columnNameProperty, columnValue);

                        var nextnDaysExpression = Expression.And(lessNextnDayE, greaterNextnDayE);
                        addExpression(ref combined, nextnDaysExpression);
                        break;
                    case "this_year":
                        var thisYear = DateTime.Now.Year;

                        var startDateThisYear = new DateTime(thisYear, 1, 1);
                        var endDateThisYear = new DateTime(thisYear, 12, 31);

                        columnValue = Expression.Constant(Convert.ChangeType(startDateThisYear, typeof(DateTime)));
                        Expression greaterThisYearE = Expression.GreaterThan(columnNameProperty, columnValue);

                        columnValue = Expression.Constant(Convert.ChangeType(endDateThisYear, typeof(DateTime)));
                        Expression lessThisYearE = Expression.LessThan(columnNameProperty, columnValue);

                        var thisYearExpression = Expression.And(lessThisYearE, greaterThisYearE);
                        addExpression(ref combined, thisYearExpression);
                        break;
                    case "last_year":
                        var yearLastYear = DateTime.Now.Year - 1;

                        var startDateLastYear = new DateTime(yearLastYear, 1, 1);
                        var endDateLastYear = new DateTime(yearLastYear, 12, 31);

                        columnValue = Expression.Constant(Convert.ChangeType(startDateLastYear, typeof(DateTime)));
                        Expression greaterLastYearE = Expression.GreaterThan(columnNameProperty, columnValue);

                        columnValue = Expression.Constant(Convert.ChangeType(endDateLastYear, typeof(DateTime)));
                        Expression lessLastYearE = Expression.LessThan(columnNameProperty, columnValue);

                        var lastYearExpression = Expression.And(lessLastYearE, greaterLastYearE);
                        addExpression(ref combined, lastYearExpression);
                        break;
                    case "next_year":
                        var yearNextYear = DateTime.Now.Year + 1;

                        var startDateNextYear = new DateTime(yearNextYear, 1, 1);
                        var endDateNextYear = new DateTime(yearNextYear, 12, 31);

                        columnValue = Expression.Constant(Convert.ChangeType(startDateNextYear, typeof(DateTime)));
                        Expression greaterNextYearE = Expression.GreaterThan(columnNameProperty, columnValue);

                        columnValue = Expression.Constant(Convert.ChangeType(endDateNextYear, typeof(DateTime)));
                        Expression lessNextYearE = Expression.LessThan(columnNameProperty, columnValue);

                        var nextYearExpression = Expression.And(lessNextYearE, greaterNextYearE);
                        addExpression(ref combined, nextYearExpression);
                        break;
                    default:
                        break;
                }
            }
            if (combined != null)
            {
                var funcExpression = typeof(Func<,>).MakeGenericType(new Type[] { entityType, typeof(bool) });
                var returnExp = Expression.Lambda(funcExpression, combined, new ParameterExpression[] { pe });
                select = select.Where(returnExp);
            }

        }

        private MethodInfo getMethodInfo(string nameMethod, Type type)
        {
            var r = type.GetMethod(nameMethod, new Type[] { type });
            return r;
        }

        private void addExpression(ref Expression combined, Expression expression)
        {
            if (combined == null)
            {
                combined = expression;
            }
            else
            {
                combined = Expression.And(combined, expression);
            }
        }
    }

}
