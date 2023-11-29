using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Siesa.SDK.Entities;
using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Siesa.SDK.Shared.DataAnnotations;
using Siesa.SDK.Shared.Services;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Siesa.Global.Enums;
using Siesa.SDK.Backend.Extensions;
using Siesa.SDK.Shared.Utilities;
//using System.Linq.Dynamic.Core;

namespace Siesa.SDK.Backend.Access
{
    public class DbSetProxy<TEntity> : DbSet<TEntity>, IQueryable<TEntity>,
                                   IAsyncEnumerable<TEntity> where TEntity : class
    {

        public override IEntityType EntityType => set.EntityType;

        private readonly DbSet<TEntity> set;
        private readonly IQueryable<TEntity> query;
        private SDKContext _context;
        private IAuthenticationService AuthenticationService { get; set; }
        private IServiceProvider _provider;
        protected dynamic _dbFactory;

        private EntityEntry<TEntity> EntryWithoutDetectChanges(TEntity entity)
        => new(_context.GetDependencies().StateManager.GetOrCreateEntry(entity, EntityType));

        private void SetEntityState(InternalEntityEntry entry, EntityState entityState)
        {
            if (entry.EntityState == EntityState.Detached)
            {
                _context.GetDependencies().EntityGraphAttacher.AttachGraph(
                    entry,
                    entityState,
                    entityState,
                    forceStateWhenUnknownKey: true);
            }
            else
            {
                entry.SetEntityState(
                    entityState,
                    acceptChanges: true,
                    forceStateWhenUnknownKey: entityState);
            }
        }

        public DbSetProxy(IAuthenticationService authenticationService, SDKContext context, DbSet<TEntity> set, IServiceProvider provider = null, bool ignoreVisibility = false)
        : this(authenticationService, context, set, set, provider, ignoreVisibility)
        {
        }

        public DbSetProxy(IAuthenticationService authenticationService, SDKContext context, DbSet<TEntity> set, IQueryable<TEntity> query, IServiceProvider provider = null, bool ignoreVisibility = false)
        {
            AuthenticationService = authenticationService;
            this.set = set;
            this._context = context;
            this.query = query;
            this._provider = provider;
            //Check if the entity is a BaseSDK
            Type entytyType = typeof(TEntity);
            //Check if the entity has a dataannotation named "SDKAuthorization"
            var dataAnnotation = entytyType.GetCustomAttributes(typeof(SDKAuthorization), false);
            if(EvaluateVisibility(entytyType, ignoreVisibility, dataAnnotation)){
                int currentUser = 0;
                if (AuthenticationService != null && AuthenticationService.User != null)
                {
                    currentUser = AuthenticationService.User.Rowid;
                }

                bool hasPropetyIsPrivate = entytyType.GetProperty("IsPrivate") != null;

                //Get the table name
                var authorizationTableName = GetNameAuthorizationTable(dataAnnotation, entytyType);
                

                //Get the type of the authorization table
                Type authEntityType = entytyType.Assembly.GetType(authorizationTableName);
                if(authEntityType != null && _provider != null){

                    Type dbContextFactoryGenericType = typeof(IDbContextFactory<>);
                    Type desiredType = context.GetType();
                    Type specificDbContextFactoryType = dbContextFactoryGenericType.MakeGenericType(desiredType);
                    _dbFactory = _provider.GetService(specificDbContextFactoryType);

                    
                    dynamic dataAuthorizedU = null;
                    dynamic dataUnauthorizedU = null;
                    using (SDKContext otherContext = _dbFactory.CreateDbContext())
                    {
                        dynamic authSet = otherContext.GetType().GetMethod("Set", types: Type.EmptyTypes)?.MakeGenericMethod(authEntityType).Invoke(otherContext, null);
                        otherContext.SetProvider(_provider);
                        List<int> listUserGroup = otherContext.AllSet<E00225_UserDataVisibilityGroup>().Include("DataVisibilityGroup").Where(x => x.RowidUser == currentUser && x.DataVisibilityGroup.Status == enumStatusBaseMaster.Active).Select(x => x.RowidDataVisibilityGroup).ToList();
                        dataAuthorizedU = GetDataUByRestrictionType(authSet, currentUser, 2, listUserGroup);
                        dataUnauthorizedU = GetDataUByRestrictionType(authSet, currentUser, 1, listUserGroup);
                    }                    
                    
                    bool hasAuthDefaulConfig = ((IEnumerable<dynamic>)dataAuthorizedU).Any(x => x.RowidRecord == null);

                    List<int?> rowidsAuthorizedU = GetRowidsAuthorizedU(dataAuthorizedU, dataUnauthorizedU);
                    List<int?> rowidsUnauthorizedU = GetRowidsUnauthorizedU(dataAuthorizedU, dataUnauthorizedU);
                    
                    Type typeBaseSdk = typeof(BaseSDK<>);
                    Type elementType = entytyType.GetProperty("Rowid")?.PropertyType;
                    Type specificBaseSdk = typeBaseSdk.MakeGenericType(elementType);
                    var newQuery = ((IQueryable)query).Cast(specificBaseSdk);
                    if (rowidsAuthorizedU.Any() || rowidsUnauthorizedU.Any()){
                        
                        newQuery = EvaluateAuthorization(rowidsAuthorizedU, newQuery, hasAuthDefaulConfig, 2, hasPropetyIsPrivate);
                        newQuery = EvaluateAuthorization(rowidsUnauthorizedU, newQuery, hasAuthDefaulConfig, 1, hasPropetyIsPrivate);
                    }
                    else
                    {
                        newQuery = newQuery.Where("Rowid == 0");
                    }
                    query = newQuery.Cast<TEntity>();
                }
            }
            if (Utilities.IsAssignableToGenericType
            (typeof(TEntity), typeof(BaseCompanyGroup<>)))
            {
                this.query = this.FilterGroupCompany(query);
            }  else if (Utilities.IsAssignableToGenericType
            (typeof(TEntity), typeof(BaseCompany<>)))
            {
                this.query = this.FilterCompany(query);
            }else if(typeof(TEntity) == typeof(E00201_Company))
            {
                this.query = this.FilterCompanyEntity(query);
            }else
            {
                this.query = query;
            }
        }

        private static bool EvaluateVisibility(Type entityType, bool ignoreVisibility, object[] dataAnnotation)
        {
            bool inheritsFromBaseSdk = InheritsFromBaseSDK(entityType);
            bool result = inheritsFromBaseSdk && !ignoreVisibility && dataAnnotation.Length > 0;
            return result;
        }

        private static List<int?> GetRowidsUnauthorizedU(dynamic dataAuthorizedU, dynamic dataUnauthorizedU)
        {
            List<int?> rowidsUnauthorizedU = ((IEnumerable<dynamic>)dataUnauthorizedU).Where(
            (Func<dynamic, bool>)(x =>
            {
                bool result = true;
                bool hasAuthDefaulConfigUser = ((IEnumerable<dynamic>)dataAuthorizedU).Any(y => y.RowidRecord == null && y.RowidDataVisibilityGroup == null);
                if (hasAuthDefaulConfigUser && x.RowidDataVisibilityGroup != null)
                {
                    return false;
                }

                if (x.RowidRecord == null)
                {
                    return false;
                }

                //result is false if the record is in the authorized list as user
                result = !(((IEnumerable<dynamic>)dataAuthorizedU)
                    .Any(y => y.RowidRecord == x.RowidRecord && y.RowidDataVisibilityGroup == null));
                        
                return result;
            })).Select((Func<dynamic, int?>)(x => x.RowidRecord)).Distinct().ToList();
            return rowidsUnauthorizedU;
        }

        private static List<int?> GetRowidsAuthorizedU(dynamic dataAuthorizedU, dynamic dataUnauthorizedU)
        {
            List<int?> rowidsAuthorizedU = ((IEnumerable<dynamic>)dataAuthorizedU).Where(
            (Func<dynamic, bool>)(x =>
            {
                bool result = true;
                bool hasUnAuthDefaulConfigUser = ((IEnumerable<dynamic>)dataUnauthorizedU).Any(y => y.RowidRecord == null && y.RowidDataVisibilityGroup == null);
                if (hasUnAuthDefaulConfigUser && x.RowidDataVisibilityGroup != null)
                {
                    return false;
                }
                //result is false if the record is in the unauthorized list as user 
                result = !(((IEnumerable<dynamic>)dataUnauthorizedU)
                    .Any(y => y.RowidRecord == x.RowidRecord && y.RowidDataVisibilityGroup == null));
                        
                return result;
            })).Select((Func<dynamic, int?>)(x => x.RowidRecord)).Distinct().ToList();
            return rowidsAuthorizedU;
        }

        private static string GetNameAuthorizationTable(object[] dataAnnotation, Type entytyType)
        {
            string authorizationTableName = ((SDKAuthorization)dataAnnotation[0]).TableName;
            //Check if the table name is not empty
            if (string.IsNullOrEmpty(authorizationTableName))
            {
                //Get table name from the context
                authorizationTableName = entytyType.Name;

                //Replace the first character of the table name with the letter "u"
                if (authorizationTableName.Length > 0)
                {
                    authorizationTableName = string.Concat("U", authorizationTableName.AsSpan(1));
                }
                authorizationTableName = $"{entytyType.Namespace}.{authorizationTableName}";
            }

            return authorizationTableName;
        }

        private static IQueryable EvaluateAuthorization(List<int?> dataAuthorizedU, IQueryable query, bool hasAuthDefaulConfig, int restrictionType, bool hasPropetyIsPrivate)
        {
            if(!dataAuthorizedU.Any()){
                return query;
            }
            string logicOperator = GetLogicOperator(restrictionType);
            string compare = GetComparisonOperator(restrictionType);
            string isPrivateWhere = GetIsPrivateWhere(restrictionType, hasPropetyIsPrivate);
            string restringedWhere = GetRestrictionWhere(hasAuthDefaulConfig);

            bool evaluateIsPrivate = ShouldEvaluateIsPrivate(restrictionType, dataAuthorizedU, hasPropetyIsPrivate);
            StringBuilder whereBuilder = BuildWhereClause(dataAuthorizedU, logicOperator, compare);

            if (whereBuilder.Length > 0)
            {
                AppendIsPrivateAndRestriction(whereBuilder, evaluateIsPrivate, isPrivateWhere, restringedWhere);
                query = query.Where(whereBuilder.ToString());
            }
            else
            {
                query = ApplyIsPrivateAndRestriction(query, evaluateIsPrivate, isPrivateWhere, restringedWhere);
            }

            return query;
        }

        private static string GetLogicOperator(int restrictionType)
        {
            return (restrictionType == 2) ? "||" : "&&";
        }

        private static string GetComparisonOperator(int restrictionType)
        {
            return (restrictionType == 2) ? "==" : "!=";
        }

        private static string GetIsPrivateWhere(int restrictionType, bool hasPropetyIsPrivate)
        {
            return (restrictionType == 2 && hasPropetyIsPrivate) ? "(IsPrivate == false)" : "";
        }

        private static string GetRestrictionWhere(bool hasAuthDefaulConfig)
        {
            return hasAuthDefaulConfig ? "" : "(Rowid == 0)";
        }

        private static bool ShouldEvaluateIsPrivate(int restrictionType, List<int?> dataAuthorizedU, bool hasPropetyIsPrivate)
        {
            return (restrictionType == 2) && dataAuthorizedU.Any(item => item == null) && hasPropetyIsPrivate;
        }

        private static StringBuilder BuildWhereClause(List<int?> dataAuthorizedU, string logicOperator, string compare)
        {
            StringBuilder whereBuilder = new StringBuilder();

            foreach (int? item in dataAuthorizedU)
            {
                if (item != null)
                {
                    if (whereBuilder.Length > 0)
                    {
                        whereBuilder.Append($" {logicOperator} ");
                    }
                    whereBuilder.Append($"(Rowid {compare} {item})");
                }
            }

            return whereBuilder;
        }

        private static void AppendIsPrivateAndRestriction(StringBuilder whereBuilder, bool evaluateIsPrivate, string isPrivateWhere, string restringedWhere)
        {
            if (evaluateIsPrivate)
            {
                whereBuilder.Insert(0, isPrivateWhere + " || (");
                whereBuilder.Append(')');
            }
            if (restringedWhere.Length > 0)
            {
                whereBuilder.Insert(0, restringedWhere + " || (");
                whereBuilder.Append(')');
            }
        }

        private static IQueryable ApplyIsPrivateAndRestriction(IQueryable query, bool evaluateIsPrivate, string isPrivateWhere, string restringedWhere)
        {
            if (evaluateIsPrivate)
            {
                query = query.Where(isPrivateWhere);
            }
            if (restringedWhere.Length > 0)
            {
                query = query.Where(restringedWhere);
            }

            return query;
        }


        private static dynamic GetDataUByRestrictionType(dynamic authSet, int currentUser, int restrictionType, List<int> listUserGroup)
        {
            Assembly assemblyQueryableExtensions = typeof(System.Linq.Dynamic.Core.DynamicQueryableExtensions).Assembly;				
            var whereMethod = typeof(IQueryable).GetExtensionMethod(assemblyQueryableExtensions, "Where", new[] { typeof(IQueryable), typeof(string), typeof(object[])});
            string whereUserStr = $"RowidUser == {currentUser}";
            if (listUserGroup.Any())
            {
                whereUserStr += $"{GetStrigGroup(listUserGroup)}";
            }
            string filter = $"({whereUserStr}) AND (RestrictionType == {restrictionType})";
            var queryUWhere = whereMethod.Invoke(authSet, new object[] { authSet, filter, Array.Empty<object>()});                    
            var selectMethod = typeof(IQueryable).GetExtensionMethod(assemblyQueryableExtensions, "Select", new[] { typeof(IQueryable), typeof(string), typeof(object[]) });                    
            var queryUSelect = selectMethod.Invoke(queryUWhere, new object[] { queryUWhere, $"new (np(RowidRecord) as RowidRecord, np(RestrictionType) as RestrictionType, np(RowidUser) as RowidUser, np(RowidDataVisibilityGroup) as RowidDataVisibilityGroup)", null });
            
            Assembly assemblyDynamic = typeof(System.Linq.Dynamic.Core.DynamicEnumerableExtensions).Assembly;
            var dynamicListMethod = typeof(IEnumerable).GetExtensionMethod(assemblyDynamic, "ToDynamicList", new[] { typeof(IEnumerable) });								
            dynamic rowidsAuthorizationList = dynamicListMethod.Invoke(queryUSelect, new object[] { queryUSelect });
            return rowidsAuthorizationList;
        }

        private static string GetStrigGroup(List<int> listUserGroup)
        {
            StringBuilder resultBuilder = new StringBuilder();
    
            foreach (int item in listUserGroup)
            {
                resultBuilder.Append($" OR RowidDataVisibilityGroup == {item}");
            }
    
            return resultBuilder.ToString();
        }


        public static bool InheritsFromBaseSDK(Type derivedType)
        {
            Type baseSdkType = typeof(BaseSDK<>);

            while (derivedType != null && derivedType != typeof(object))
            {
                if (derivedType.IsGenericType && derivedType.GetGenericTypeDefinition() == baseSdkType)
                {
                    return true;
                }

                derivedType = derivedType.BaseType;
            }

            return false;
        }
        private IQueryable<TEntity> FilterGroupCompany(IQueryable<TEntity> query)
        {
            if(AuthenticationService?.User == null)
            {
                 return query; //TODO: Validar si devolver todo o nada
            }
            var group_company_session = AuthenticationService.User.RowidCompanyGroup;
            var sdk_query = query.Include("CompanyGroup").Where("CompanyGroup.Rowid == @0", group_company_session);
            return sdk_query.Cast<TEntity>();
        }

        private IQueryable<TEntity> FilterCompany(IQueryable<TEntity> query)
        {
            if(AuthenticationService?.User == null)
            {
                 return query; //TODO: Validar si devolver todo o nada
            }
            var group_company_session = AuthenticationService.User.RowidCompanyGroup;
            var sdk_query = query.Include("Company").Where("Company.RowidCompanyGroup == @0", group_company_session);
            return sdk_query.Cast<TEntity>();
        }

        private IQueryable<TEntity> FilterCompanyEntity(IQueryable<TEntity> query)
        {
            if(AuthenticationService?.User == null)
            {
                 return query; //TODO: Validar si devolver todo o nada
            }
            var group_company_session = AuthenticationService.User.RowidCompanyGroup;
            var sdk_query = query.Where("RowidCompanyGroup == @0", group_company_session);
            return sdk_query.Cast<TEntity>();
        }


        IEnumerator<TEntity> IEnumerable<TEntity>.GetEnumerator()
        {
            return query.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return query.GetEnumerator();
        }

        Expression IQueryable.Expression
        {
            get { return query.Expression; }
        }

        IQueryProvider IQueryable.Provider
        {
            get { return query.Provider; }
        }


        IAsyncEnumerator<TEntity> IAsyncEnumerable<TEntity>.GetAsyncEnumerator(CancellationToken cancellationToken)
        {
            return query.AsAsyncEnumerable().GetAsyncEnumerator(cancellationToken);
        }

        public override TEntity? Find(params object?[]? keyValues)
        {
            if (FirstOrDefault(keyValues, out var tEntity)) return tEntity;
            return set.Find(keyValues);
        }

        public override ValueTask<TEntity?> FindAsync(params object?[]? keyValues)
        {
            if(FirstOrDefault(keyValues, out var tEntity)) return new ValueTask<TEntity?>(tEntity);
            return set.FindAsync(keyValues);
        }


        public override ValueTask<TEntity?> FindAsync(object?[]? keyValues, CancellationToken cancellationToken)
        {
            if(FirstOrDefault(keyValues, out var tEntity)) return new ValueTask<TEntity?>(tEntity);
            return set.FindAsync(keyValues, cancellationToken);
        }
        
        private bool FirstOrDefault(object[] keyValues, out TEntity tEntity)
        {
            if (keyValues != null && keyValues.Length == 1 && keyValues[0] != null)
            {
                {
                    tEntity = query.Where("Rowid == @0", keyValues[0]).FirstOrDefault();
                    return true;
                }
            }

            tEntity = null;
            return false;
        }

        public override LocalView<TEntity> Local => set.Local;
        //Type IQueryable.ElementType => ;
        Type IQueryable.ElementType
        {
            get { return query.ElementType; }
        }

        public override EntityEntry<TEntity> Remove(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var entry = EntryWithoutDetectChanges(entity);

            var initialState = entry.State;
            if (initialState == EntityState.Detached)
            {
                SetEntityState(entry.GetInfrastructure(), EntityState.Unchanged);
            }

            // An Added entity does not yet exist in the database. If it is then marked as deleted there is
            // nothing to delete because it was not yet inserted, so just make sure it doesn't get inserted.
            entry.State =
                initialState == EntityState.Added
                    ? EntityState.Detached
                    : EntityState.Deleted;

            return entry;
        }

    }
}