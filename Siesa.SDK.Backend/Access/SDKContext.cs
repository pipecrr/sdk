using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;
//using Siesa.SDK.Backend.Interceptors;
using Siesa.SDK.Entities;
using Siesa.SDK.Entities.Converters;
using Siesa.SDK.Shared.Logs.DataChangeLog;
using Siesa.SDK.Shared.Services;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Siesa.SDK.Backend.Access
{
    public abstract class SDKContext: DbContext
    {
		public DbSet<E00040_Module>? E00040_Module { get; set; }

		public DbSet<E00024_GenericEnumCode>? E00024_GenericEnumCode { get; set; }

		public DbSet<E00023_ResourceCustomDescription>? E00023_ResourceCustomDescription { get; set; }

		public DbSet<E00022_ResourceDescription>? E00022_ResourceDescription { get; set; }

		public DbSet<E00021_Culture>? E00021_Culture { get; set; }

		public DbSet<E00020_Resource>? E00020_Resource { get; set; }

        private IServiceProvider ServiceProvider {get; set;}
        public SDKContext(DbContextOptions options) : base(options)
        {
        }

        public void SetProvider(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public void ResetConcurrencyValues(Object entity, Object UpdatedEntity) {
            var lEntry = this.Entry(entity);
            foreach (var lProperty in lEntry.Metadata.GetProperties().Where(x => x.IsConcurrencyToken)) {
                lEntry.OriginalValues[lProperty] = UpdatedEntity.GetType().GetProperty(lProperty.Name).GetValue(UpdatedEntity);
            }
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
                    if ((i - lastUpperIndex) > 1 && (i - 1) >= 0 && text[i - 1] != '_')
                    {
                        sb.Append('_');
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

        public DbSet<TEntity> AllSet<TEntity>() where TEntity : class {
            return base.Set<TEntity>();
        }

        public override DbSet<TEntity> Set<TEntity>()
        {
            if(ServiceProvider != null){
                try {
                    return ActivatorUtilities.CreateInstance(ServiceProvider, typeof(DbSetProxy<TEntity>), new object[] { this, base.Set<TEntity>() }) as DbSet<TEntity>;
                }catch(Exception e){
                    Console.WriteLine($"{e.Message}***********");
                }                
            }
            return new DbSetProxy<TEntity>(null ,this, base.Set<TEntity>()); 
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder builder)
        {
            builder.Properties<DateOnly>()
                .HaveConversion<DateOnlyConverter, DateOnlyComparer>()
                .HaveColumnType("date");

            // builder.Properties<TimeOnly>()
            //     .HaveConversion<TimeOnlyConverter, TimeOnlyComparer>()
            //     .HaveColumnType("time");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)  
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                string table_name = ToSnakeCase(entity.ShortName().Trim());
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
