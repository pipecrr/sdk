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

        public DbSetProxy(IAuthenticationService authenticationService, SDKContext context, DbSet<TEntity> set)
        : this(authenticationService, context, set, set)
        {
        }

        public DbSetProxy(IAuthenticationService authenticationService, SDKContext context, DbSet<TEntity> set, IQueryable<TEntity> query)
        {
            AuthenticationService = authenticationService;
            this.set = set;
            this._context = context;
            this.query = query;
            //Check if the entity is a BaseSDK
            bool inheritsFromBaseSDK = InheritsFromBaseSDK(typeof(TEntity));
            if (inheritsFromBaseSDK)
            {
                //Check if the entity has a dataannotation named "SDKAuthorization"
                var dataAnnotation = typeof(TEntity).GetCustomAttributes(typeof(SDKAuthorization), false);
                if (dataAnnotation.Length > 0)
                {
                    int current_user = 0;
                    if (AuthenticationService != null && AuthenticationService.User != null)
                    {
                        current_user = AuthenticationService.User.Rowid;
                    }

                    //Get the table name
                    var authorizationTableName = ((SDKAuthorization)dataAnnotation[0]).TableName;
                    //Check if the table name is not empty
                    if (string.IsNullOrEmpty(authorizationTableName))
                    {
                        //Get table name from the context
                        authorizationTableName = typeof(TEntity).Name;

                        //Replace the first character of the table name with the letter "u"
                        if (authorizationTableName.Length > 0)
                        {
                            authorizationTableName = "U" + authorizationTableName.Substring(1);
                        }
                        authorizationTableName = $"{typeof(TEntity).Namespace}.{authorizationTableName}";
                    }

                    //Get the type of the authorization table
                    Type authEntityType = typeof(TEntity).Assembly.GetType(authorizationTableName);
                    dynamic authSet = context.GetType().GetMethod("Set", types: Type.EmptyTypes).MakeGenericMethod(authEntityType).Invoke(context, null);

                    dynamic dataAuthorizedU = GetDataUByRestrictionType(authSet, current_user, 2);
                    dynamic dataUnauthorizedU = GetDataUByRestrictionType(authSet, current_user, 1);
                    
                    var newQuery = ((IQueryable<BaseSDK<int>>)query);

                    bool hasAuthDefaulConfig = ((IEnumerable<dynamic>)dataAuthorizedU).Where((Func<dynamic, bool>)(x => x.RowidRecord == null)).Any();
                    bool hasUnAuthDefaulConfig = ((IEnumerable<dynamic>)dataUnauthorizedU).Where((Func<dynamic, bool>)(x => x.RowidRecord == null)).Any();
                    
                    newQuery = EvaluateAuthorization(dataAuthorizedU, newQuery, hasAuthDefaulConfig, hasUnAuthDefaulConfig, 2);
                    newQuery = EvaluateAuthorization(dataUnauthorizedU, newQuery, hasAuthDefaulConfig, hasUnAuthDefaulConfig, 1);
                    
                    this.query = newQuery.Cast<TEntity>();
                }
            }

            if (Utilities.IsAssignableToGenericType
            (typeof(TEntity), typeof(BaseCompanyGroup<>)))
            {
                Type rowidType = typeof(TEntity).GetProperty("Rowid").GetType();

                switch (rowidType.Name)
                {
                    case "Int64":
                        this.query = this.FilterGroupCompany<long>(query);
                        break;
                    case "Int16":
                        this.query = this.FilterGroupCompany<short>(query);
                        break;
                    case "Int32":
                    default:
                        this.query = this.FilterGroupCompany<int>(query);
                        break;
                }
            }  else if (Utilities.IsAssignableToGenericType
            (typeof(TEntity), typeof(BaseCompany<>)))
            {
                Type rowidType = typeof(TEntity).GetProperty("Rowid").GetType();

                switch (rowidType.Name)
                {
                    case "Int64":
                        this.query = this.FilterCompany<long>(query);
                        break;
                    case "Int16":
                        this.query = this.FilterCompany<short>(query);
                        break;
                    case "Int32":
                    default:
                        this.query = this.FilterCompany<int>(query);
                        break;
                }
            }else if(typeof(TEntity) == typeof(E00201_Company)){
                this.query = this.FilterCompanyEntity(query);
            }
        }

        private IQueryable<BaseSDK<int>> EvaluateAuthorization(dynamic dataAuthorizedU, IQueryable<BaseSDK<int>> query, bool hasDefaulConfig, bool hasUnAuthDefaulConfig, int restrictionType)
        {
            string logicOperator = "&&";
            string compare = "!=";
            string where = "";
            string isPrivateWhere = "";
            if (restrictionType == 2)
            {
                logicOperator = "||";
                compare = "==";
                isPrivateWhere = "(IsPrivate == false)";
            }
            bool evaluateIsPrivate = false;
            foreach (dynamic item in dataAuthorizedU)
            {
                if (where.Length > 0)
                {
                    where += $" {logicOperator} ";
                }
                if(item.RowidRecord != null){                    
                    where += $"(Rowid {compare} {item.RowidRecord})";
                }
                if(restrictionType == 2 && item.RowidRecord == null){
                    evaluateIsPrivate = true;
                }
            }            
            if(where.Length > 0){
                if(evaluateIsPrivate){
                    where = $"{isPrivateWhere} || ({where})";
                }
                where = $"({where})";
                query = query.Where(where);
            }else{
                if(evaluateIsPrivate){
                    query = query.Where(isPrivateWhere);
                }
            }
            return query;
        }

        private dynamic GetDataUByRestrictionType(dynamic authSet, int currentUser, int restrictionType)
        {
            Assembly _assemblyQueryableExtensions = typeof(System.Linq.Dynamic.Core.DynamicQueryableExtensions).Assembly;				
            var whereMethod = typeof(IQueryable).GetExtensionMethod(_assemblyQueryableExtensions, "Where", new[] { typeof(IQueryable), typeof(string), typeof(object[])});
            string filter = $"(RowidUser == {currentUser}) AND (RestrictionType == {restrictionType})";
            var queryUWhere = whereMethod.Invoke(authSet, new object[] { authSet, filter, new object[]{}});                    
            var selectMethod = typeof(IQueryable).GetExtensionMethod(_assemblyQueryableExtensions, "Select", new[] { typeof(IQueryable), typeof(string), typeof(object[]) });                    
            var queryUSelect = selectMethod.Invoke(queryUWhere, new object[] { queryUWhere, $"new (np(RowidRecord) as RowidRecord, np(RestrictionType) as RestrictionType)", null });                    
            Assembly _assemblyDynamic = typeof(System.Linq.Dynamic.Core.DynamicEnumerableExtensions).Assembly;
            var dynamicListMethod = typeof(IEnumerable).GetExtensionMethod(_assemblyDynamic, "ToDynamicList", new[] { typeof(IEnumerable) });								
            dynamic rowidsAuthorizationList = dynamicListMethod.Invoke(queryUSelect, new object[] { queryUSelect });
            return rowidsAuthorizationList;
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
        private IQueryable<TEntity> FilterGroupCompany<T>(IQueryable<TEntity> query)
        {
            if(AuthenticationService?.User == null)
            {
                 return query; //TODO: Validar si devolver todo o nada
            }
            var group_company_session = AuthenticationService.User.RowidCompanyGroup;
            var sdk_query = (IQueryable<BaseCompanyGroup<T>>)query;
            sdk_query = sdk_query.Where(x => x.RowidCompanyGroup == group_company_session);
            return sdk_query.Cast<TEntity>();
        }

        private IQueryable<TEntity> FilterCompany<T>(IQueryable<TEntity> query)
        {
            if(AuthenticationService?.User == null)
            {
                 return query; //TODO: Validar si devolver todo o nada
            }
            var group_company_session = AuthenticationService.User.RowidCompanyGroup;
            var sdk_query = (IQueryable<BaseCompany<T>>)query;
            sdk_query = sdk_query.Include("Company").Where(x => x.Company.RowidCompanyGroup == group_company_session);
            return sdk_query.Cast<TEntity>();
        }

        private IQueryable<TEntity> FilterCompanyEntity(IQueryable<TEntity> query)
        {
            if(AuthenticationService?.User == null)
            {
                 return query; //TODO: Validar si devolver todo o nada
            }
            var group_company_session = AuthenticationService.User.RowidCompanyGroup;
            var sdk_query = (IQueryable<E00201_Company>)query;
            sdk_query = sdk_query.Where(x => x.RowidCompanyGroup == group_company_session);
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
            return set.GetAsyncEnumerator(cancellationToken);
        }

        public override TEntity? Find(params object?[]? keyValues)
        {
            return set.Find(keyValues);
        }


        public override ValueTask<TEntity?> FindAsync(params object?[]? keyValues)
        {
            return set.FindAsync(keyValues);
        }


        public override ValueTask<TEntity?> FindAsync(object?[]? keyValues, CancellationToken cancellationToken)
        {
            return set.FindAsync(keyValues, cancellationToken);
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