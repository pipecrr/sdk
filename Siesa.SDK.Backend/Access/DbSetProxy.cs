﻿using Microsoft.EntityFrameworkCore;
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

        public DbSetProxy(IAuthenticationService authenticationService, SDKContext context, DbSet<TEntity> set, bool ignoreVisibility = false)
        : this(authenticationService, context, set, set, ignoreVisibility)
        {
        }

        public DbSetProxy(IAuthenticationService authenticationService, SDKContext context, DbSet<TEntity> set, IQueryable<TEntity> query, bool ignoreVisibility = false)
        {
            AuthenticationService = authenticationService;
            this.set = set;
            this._context = context;
            this.query = query;
            //Check if the entity is a BaseSDK
            bool inheritsFromBaseSdk = InheritsFromBaseSDK(typeof(TEntity));
            if (inheritsFromBaseSdk && !ignoreVisibility)
            {
                //Check if the entity has a dataannotation named "SDKAuthorization"
                var dataAnnotation = typeof(TEntity).GetCustomAttributes(typeof(SDKAuthorization), false);
                if (dataAnnotation.Length > 0)
                {
                    int currentUser = 0;
                    if (AuthenticationService != null && AuthenticationService.User != null)
                    {
                        currentUser = AuthenticationService.User.Rowid;
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

                    List<int> listUserGroup = context.Set<E00225_UserDataVisibilityGroup>().Include("DataVisibilityGroup").Where(x => x.RowidUser == currentUser && x.DataVisibilityGroup.Status == enumStatusBaseMaster.Active).Select(x => x.RowidDataVisibilityGroup).ToList();

                    //Get the type of the authorization table
                    Type authEntityType = typeof(TEntity).Assembly.GetType(authorizationTableName);
                    dynamic authSet = context.GetType().GetMethod("Set", types: Type.EmptyTypes)?.MakeGenericMethod(authEntityType).Invoke(context, null);

                    dynamic dataAuthorizedU = GetDataUByRestrictionType(authSet, currentUser, 2, listUserGroup);
                    dynamic dataUnauthorizedU = GetDataUByRestrictionType(authSet, currentUser, 1, listUserGroup);

                    bool hasAuthDefaulConfig = ((IEnumerable<dynamic>)dataAuthorizedU).Where((Func<dynamic, bool>)(x => x.RowidRecord == null)).Any();
                    bool hasUnAuthDefaulConfig = ((IEnumerable<dynamic>)dataUnauthorizedU).Where((Func<dynamic, bool>)(x => x.RowidRecord == null)).Any();

                    List<int?> rowidsAuthorizedU = ((IEnumerable<dynamic>)dataAuthorizedU).Where(
                        (Func<dynamic, bool>)(x =>
                        {
                            bool result = true;
                            bool isGroup = x.RowidDataVisibilityGroup != null;
                            if (isGroup)
                            {
                                bool hasUnAuthDefaulConfigGroup = ((IEnumerable<dynamic>)dataUnauthorizedU).Where((Func<dynamic, bool>)(x => x.RowidRecord == null && x.RowidDataVisibilityGroup == null)).Any();
                                if (hasUnAuthDefaulConfigGroup)
                                {
                                    result = false;
                                }
                                else
                                {
                                    bool existInUnAuthorized = ((IEnumerable<dynamic>)dataUnauthorizedU)
                                        .Where((Func<dynamic, bool>)(y =>
                                            y.RowidRecord == x.RowidRecord && x.RowidDataVisibilityGroup == null)).Any();
                                    result = !existInUnAuthorized;
                                }
                            }

                            return result;
                        })).Select((Func<dynamic, int?>)(x => x.RowidRecord)).Distinct().ToList();
                    
                    List<int?> rowidsUnauthorizedU = ((IEnumerable<dynamic>)dataUnauthorizedU).Where(
                        (Func<dynamic, bool>)(x =>
                        {
                            bool result = true;
                            bool isGroup = x.RowidDataVisibilityGroup != null;
                            if (isGroup)
                            {
                                bool hasAuthDefaulConfigGroup = ((IEnumerable<dynamic>)dataAuthorizedU).Where((Func<dynamic, bool>)(x => x.RowidRecord == null && x.RowidDataVisibilityGroup == null)).Any();
                                if (hasAuthDefaulConfigGroup)
                                {
                                    result = false;
                                }
                                else
                                {
                                    bool existInAuthorized = ((IEnumerable<dynamic>)dataAuthorizedU)
                                        .Where((Func<dynamic, bool>)(y =>
                                            y.RowidRecord == x.RowidRecord && x.RowidDataVisibilityGroup == null)).Any();
                                    result = !existInAuthorized;
                                }
                            }
                            return result;
                        })).Select((Func<dynamic, int?>)(x => x.RowidRecord)).Distinct().ToList();

                    var newQuery = ((IQueryable<BaseSDK<int>>)query);                    
                    
                    newQuery = EvaluateAuthorization(rowidsAuthorizedU, newQuery, hasAuthDefaulConfig, hasUnAuthDefaulConfig, 2);
                    newQuery = EvaluateAuthorization(rowidsUnauthorizedU, newQuery, hasAuthDefaulConfig, hasUnAuthDefaulConfig, 1);
                    
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

        private IQueryable<BaseSDK<int>> EvaluateAuthorization(List<int?> dataAuthorizedU, IQueryable<BaseSDK<int>> query, bool hasAuthDefaulConfig, bool hasUnAuthDefaulConfig, int restrictionType)
        {
            string logicOperator = "&&";
            string compare = "!=";
            string where = "";
            string isPrivateWhere = "";
            string restringedWhere = "(Rowid == 0)";
            if (restrictionType == 2)
            {
                logicOperator = "||";
                compare = "==";
                isPrivateWhere = "(IsPrivate == false)";
            }
            if (hasAuthDefaulConfig)
            {
                restringedWhere = "";
            }

            bool evaluateIsPrivate = false;
            foreach (int? item in dataAuthorizedU)
            {
                if(item != null){
                    if (where.Length > 0)
                    {
                        where += $" {logicOperator} ";
                    }
                    where += $"(Rowid {compare} {item})";
                }
                if(restrictionType == 2 && item == null){
                    evaluateIsPrivate = true;
                }
            }
            if(where.Length > 0){
                if(evaluateIsPrivate){
                    where = $"{isPrivateWhere} || ({where})";
                }
                if(restringedWhere.Length > 0){
                    where = $"{restringedWhere} || ({where})";
                }
                where = $"({where})";
                query = query.Where(where);
            }else{
                if(evaluateIsPrivate){
                    query = query.Where(isPrivateWhere);
                }
                if(restringedWhere.Length > 0){
                    query = query.Where(restringedWhere);
                }
            }
            return query;
        }

        private dynamic GetDataUByRestrictionType(dynamic authSet, int currentUser, int restrictionType, List<int> listUserGroup)
        {
            Assembly assemblyQueryableExtensions = typeof(System.Linq.Dynamic.Core.DynamicQueryableExtensions).Assembly;				
            var whereMethod = typeof(IQueryable).GetExtensionMethod(assemblyQueryableExtensions, "Where", new[] { typeof(IQueryable), typeof(string), typeof(object[])});
            string whereUserStr = $"RowidUser == {currentUser}";
            if (listUserGroup.Any())
            {
                whereUserStr += $"{GetStrigGroup(listUserGroup)}";
            }
            string filter = $"({whereUserStr}) AND (RestrictionType == {restrictionType})";
            var queryUWhere = whereMethod.Invoke(authSet, new object[] { authSet, filter, new object[]{}});                    
            var selectMethod = typeof(IQueryable).GetExtensionMethod(assemblyQueryableExtensions, "Select", new[] { typeof(IQueryable), typeof(string), typeof(object[]) });                    
            var queryUSelect = selectMethod.Invoke(queryUWhere, new object[] { queryUWhere, $"new (np(RowidRecord) as RowidRecord, np(RestrictionType) as RestrictionType, np(RowidUser) as RowidUser, np(RowidDataVisibilityGroup) as RowidDataVisibilityGroup)", null });
            
            var orderByMethod = typeof(IQueryable).GetExtensionMethod(assemblyQueryableExtensions, "OrderBy", new[] { typeof(IQueryable), typeof(string), typeof(object[]) });
            //order by 
            
            Assembly assemblyDynamic = typeof(System.Linq.Dynamic.Core.DynamicEnumerableExtensions).Assembly;
            var dynamicListMethod = typeof(IEnumerable).GetExtensionMethod(assemblyDynamic, "ToDynamicList", new[] { typeof(IEnumerable) });								
            dynamic rowidsAuthorizationList = dynamicListMethod.Invoke(queryUSelect, new object[] { queryUSelect });
            return rowidsAuthorizationList;
        }

        private string GetStrigGroup(List<int> listUserGroup)
        {
            string result = "";
            foreach (int item in listUserGroup)
            {
                result += $" OR RowidDataVisibilityGroup == {item}";
            }
            return result;
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