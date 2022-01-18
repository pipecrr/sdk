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

namespace Siesa.SDK.Backend.Access
{
    public class DbSetProxy<TEntity> : DbSet<TEntity>, IQueryable<TEntity>,
                                   IAsyncEnumerable<TEntity> where TEntity : class
    {

        public override IEntityType EntityType => set.EntityType;

        private readonly DbSet<TEntity> set;
        private readonly IQueryable<TEntity> query;
        private SDKContext _context;


        public DbSetProxy(SDKContext context, DbSet<TEntity> set)
        : this(context, set, set)
        {
        }

        public DbSetProxy(SDKContext context, DbSet<TEntity> set, IQueryable<TEntity> query)
        {
            this._context = context;
            this.query = query;
            //Check if the entity is a BaseEntity
            if (typeof(BaseEntity).IsAssignableFrom(typeof(TEntity)))
            {
                //Check if the entity has a dataannotation named "SDKAuthorization"
                var dataAnnotation = typeof(TEntity).GetCustomAttributes(typeof(SDKAuthorization), false);
                if (dataAnnotation.Length > 0)
                {
                    var prueba_rowid_user = 3; //TODO: Cambiar por el usuario logueado

                    //Get the table name
                    var authorizationTableName = ((SDKAuthorization)dataAnnotation[0]).TableName;
                    //Check if the table name is not empty
                    if (string.IsNullOrEmpty(authorizationTableName))
                    {
                        //Get table name from the context
                        var tableNameFromContext = _context.Model.FindEntityType(typeof(TEntity)).GetTableName();
                        authorizationTableName = tableNameFromContext;

                        //Replace the first character of the table name with the letter "u"
                        if (authorizationTableName.Length > 0)
                        {
                            authorizationTableName = "U" + authorizationTableName.Substring(1);
                        }
                    }

                    //IQueryable<BaseUserPermissionEntity> prueba = from permission in _context.Set<BaseUserPermissionEntity>(authorizationTableName)


                    var sdk_query = ((IQueryable<BaseEntity>)query);
                    sdk_query = sdk_query.Where(x => x.Source == authorizationTableName);
                    this.query = sdk_query.Cast<TEntity>();
                }

                
            }
        }

        IEnumerator<TEntity>  IEnumerable<TEntity>.GetEnumerator()
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

    }
}