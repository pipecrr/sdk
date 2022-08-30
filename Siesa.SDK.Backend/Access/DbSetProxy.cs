using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Siesa.SDK.Entities;
using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Siesa.SDK.Shared.DataAnnotations;
using Siesa.SDK.Shared.Services;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Siesa.SDK.Entities.Enums;
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
            if (typeof(BaseSDK<>).IsAssignableFrom(typeof(TEntity)))
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

                    Type authEntityType = typeof(TEntity).Assembly.GetType(authorizationTableName);
                    var authSet = (IQueryable<BaseUserPermission<TEntity, Int64>>)_context.GetType().GetMethod("Set", types: Type.EmptyTypes).MakeGenericMethod(authEntityType).Invoke(_context, null);

                    var sdk_query = ((IQueryable<BaseSDK<int>>)query); //TODO: Cambiar tipo de dato en el generic, según la clase
                    var join_sql = sdk_query.Join(authSet,
                        e => e.Rowid,
                        u => u.RowidRecord,
                        (e, u) => new { e, u })
                        .Where(
                            x => ((
                                x.u.UserType == PermissionUserTypes.User && x.u.RowidUser == current_user
                                && x.u.AuthorizationType == PermissionAuthTypes.Query_Tx
                            )
                            || false //TODO: Add other authorization types
                            )
                        );
                    sdk_query = join_sql.Select(x => x.e);
                    this.query = sdk_query.Cast<TEntity>();
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