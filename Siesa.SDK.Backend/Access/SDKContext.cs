using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
//using Siesa.SDK.Backend.Interceptors;
using Siesa.SDK.Entities;
using Siesa.SDK.Shared.Logs.DataChangeLog;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Siesa.SDK.Backend.Access
{
    public abstract class SDKContext: DbContext
    {
        public SDKContext(DbContextOptions options) : base(options)
        {
        }

        private string ToSnakeCase(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var sb = new StringBuilder();
            sb.Append(text[0].ToString().ToLower());
            var lastUpperIndex = -1;
            //check if first character is upper case
            if (char.IsUpper(text[0]))
            {
                lastUpperIndex = 0;
            }
            for (var i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]))
                {
                    if ((i - lastUpperIndex) > 1)
                    {
                        sb.Append("_");
                    }
                    lastUpperIndex = i;
                }
                    
                sb.Append(text[i].ToString().ToLower());
            }
            return sb.ToString();
        }
        

        public override int SaveChanges()
        {
            LogCreator logCreator = new(ChangeTracker.Entries());
            logCreator.ProccessBeforeSaveChanges();
            var result = base.SaveChanges();
            logCreator.ProccessAfterSaveChanges();
            CollectChanges(logCreator);
            return result;
        }

        private static void CollectChanges(LogCreator logCreator)
        {
            try
            {
                LogService.SaveDataEntityLog(logCreator.DataEntityLogs);
            }
            catch (Exception) { }
        }

        /*public override DbSet<TEntity> Set<TEntity>()
        {
            return new DbSetProxy<TEntity>(this, base.Set<TEntity>());
        }*/

        protected override void OnModelCreating(ModelBuilder modelBuilder)  
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                string table_name = entity.ShortName().Trim().ToLower();
                // table names
                modelBuilder.Entity(entity.Name).ToTable(table_name);
                var table_name_parts = table_name.Split('_');
                //get text before first underscore
                var prefix = table_name_parts[0];
                //if first character is an "U" then replaceit with "CU"
                if (prefix.StartsWith("U"))
                {
                    prefix = "cu" + prefix.Substring(1);
                }else{
                    prefix = "c" + prefix.Substring(1);
                }

                // properties
                foreach (var property in entity.GetProperties())
                {
                    var column_name = property.GetColumnName(StoreObjectIdentifier.Table(table_name, entity.GetSchema())).Trim();
                    column_name = ToSnakeCase(column_name);
                    
                    //Check if property is a foreign key and it ends with "rowid"
                    if (property.IsForeignKey() && column_name.EndsWith("rowid")) {
                        column_name = column_name.Substring(0, column_name.Length - 6);
                        column_name = "rowid_" + column_name;
                    }



                    
                    property.SetColumnName(prefix + "_" + column_name);
                }

                //check if entity inherits from BaseEntity
                if (typeof(BaseEntity).IsAssignableFrom(entity.ClrType))
                {
                    /*
                    var newParam = Expression.Parameter(entity.ClrType);
                    Expression<Func<BaseEntity, bool>> test_delegate = x => x.Source == "SDK2";
                    var newBody = ReplacingExpressionVisitor.Replace(test_delegate.Parameters.Single(), newParam, test_delegate.Body);
                    var filter = Expression.Lambda(newBody, newParam);
                    modelBuilder.Entity(entity.Name).HasQueryFilter(filter);*/
                    
                }

                //remove cascade delete
                //entity.GetForeignKeys().Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade).ToList().ForEach(fk => fk.DeleteBehavior = DeleteBehavior.Restrict);

                

            }
        }

        public DbSet<E00001_Resource> E00001_Resource { get; set; }
        public DbSet<E00002_ResourceValue> E00002_ResourceValue { get; set; }
        public DbSet<E00003_Language> E00003_Language { get; set; }
        public DbSet<E00102_User> E00102_User { get; set; }
        public DbSet<E00110_Team> E00110_Team { get; set; }
        public DbSet<E00103_Role> E00103_Role { get; set; }
        public DbSet<E00108_AuthorizedOperation> E00108_AuthorizedOperation { get; set; }
        public DbSet<E00105_Feature> E00105_Feature { get; set; }
        public DbSet<E00109_OverwriteAuthorizedOperation> E00109_OverwriteAuthorizedOperation { get; set; }
        public DbSet<U00102_User> U00102_User { get; set; }

        public DbSet<E00130_MenuGroup> E00130_MenuGroup { get; set; }
        public DbSet<U00130_MenuGroup> U00130_MenuGroup { get; set; }

        public DbSet<E00131_Menu> E00131_Menu { get; set; }
        public DbSet<E00132_MenuGroupDetail> E00132_MenuGroupDetail { get; set; }
        public DbSet<U00132_MenuGroupDetail> U00132_MenuGroupDetail { get; set; }
        public DbSet<E00133_FavoritesMenu> E00133_FavoritesMenu { get; set; }

    }
}
