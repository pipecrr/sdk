using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;
using Siesa.SDK.Entities;
using Siesa.SDK.Entities.Converters;
using Siesa.SDK.Shared.Logs.DataChangeLog;
using Siesa.SDK.Shared.Services;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Siesa.SDK.Shared.Utilities;
using Siesa.SDK.Shared.DTOS;
using Siesa.SDK.Backend.Extensions;
using System.Reflection;
using Siesa.SDK.Shared.Criptography;
using Siesa.SDK.Shared.DataAnnotations;
using Microsoft.Extensions.Configuration;
using System.Threading;

namespace Siesa.SDK.Backend.Access
{
    public class SDKContext : DbContext
    {
	public DbSet<E00231_FlexDelta>? E00231_FlexDelta { get; set; }

	public DbSet<E00222_UserAccessSchedulingJournal>? E00222_UserAccessSchedulingJournal { get; set; }

	public DbSet<U00250_DynamicEntity>? U00250_DynamicEntity { get; set; }

	public DbSet<E00240_Report>? E00240_Report { get; set; }

	public DbSet<E00043_ChildFeature>? E00043_ChildFeature { get; set; }

	public DbSet<E00251_DynamicEntityColumn>? E00251_DynamicEntityColumn { get; set; }

	public DbSet<E00250_DynamicEntity>? E00250_DynamicEntity { get; set; }

	public DbSet<E00230_Flex>? E00230_Flex { get; set; }

        public DbSet<E00271_AttachmentDetail>? E00271_AttachmentDetail { get; set; }

        public DbSet<E00270_Attachment>? E00270_Attachment { get; set; }

        public DbSet<E00025_EnumValue>? E00025_EnumValue { get; set; }

        public DbSet<U00225_UserDataVisibilityGroup>? U00225_UserDataVisibilityGroup { get; set; }

        public DbSet<U00224_DataVisibilityGroup>? U00224_DataVisibilityGroup { get; set; }

        public DbSet<U00223_UserAccountPolicy>? U00223_UserAccountPolicy { get; set; }

        public DbSet<U00220_User>? U00220_User { get; set; }
        // public DbSet<E00223_UserAccountPolicy>? E00223_UserAccountPolicy { get; set; }

        public DbSet<E00224_DataVisibilityGroup>? E00224_DataVisibilityGroup { get; set; }

        public DbSet<E00221_UserPasswordHistory>? E00221_UserPasswordHistory { get; set; }

        public DbSet<E00225_UserDataVisibilityGroup>? E00225_UserDataVisibilityGroup { get; set; }

        public DbSet<E00220_User>? E00220_User { get; set; }

        public DbSet<E00226_Rol>? E00226_Rol { get; set; }

        public DbSet<E00227_UserRol>? E00227_UserRol { get; set; }

        public DbSet<E00210_Session>? E00210_Session { get; set; }

        public DbSet<E00228_RolAuthorizedOperation>? E00228_RolAuthorizedOperation { get; set; }

        public DbSet<E00229_OverwriteRolAuthorizedOperation>? E00229_OverwriteRolAuthorizedOperation { get; set; }

        public DbSet<E00201_Company>? E00201_Company { get; set; }

        public DbSet<E00200_CompanyGroup>? E00200_CompanyGroup { get; set; }

        public DbSet<U00065_SuiteCustom>? U00065_SuiteCustom { get; set; }

        public DbSet<U00067_SuiteMenuCustom>? U00067_SuiteMenuCustom { get; set; }

        public DbSet<U00066_MenuCustom>? U00066_MenuCustom { get; set; }

        public DbSet<E00061_Menu>? E00061_Menu { get; set; }

        public DbSet<E00062_SuiteMenu>? E00062_SuiteMenu { get; set; }

        public DbSet<E00063_FavoritesMenu>? E00063_FavoritesMenu { get; set; }

        public DbSet<E00064_RecentMenu>? E00064_RecentMenu { get; set; }

        public DbSet<E00065_SuiteCustom>? E00065_SuiteCustom { get; set; }

        public DbSet<E00060_Suite>? E00060_Suite { get; set; }

        public DbSet<E00046_ModuleFeature>? E00046_ModuleFeature { get; set; }

        public DbSet<E00042_Operation>? E00042_Operation { get; set; }

        public DbSet<E00041_Action>? E00041_Action { get; set; }

        public DbSet<E00066_MenuCustom>? E00066_MenuCustom { get; set; }

        public DbSet<E00040_Feature>? E00040_Feature { get; set; }

        public DbSet<E00067_SuiteMenuCustom>? E00067_SuiteMenuCustom { get; set; }

        public DbSet<E00024_Enum>? E00024_Enum { get; set; }

        public DbSet<E00023_ResourceCustomDescription>? E00023_ResourceCustomDescription { get; set; }

        public DbSet<E00022_ResourceDescription>? E00022_ResourceDescription { get; set; }

        public DbSet<E00021_Culture>? E00021_Culture { get; set; }

        public DbSet<E00010_Module>? E00010_Module { get; set; }

        public DbSet<E00020_Resource>? E00020_Resource { get; set; }

        private IServiceProvider ServiceProvider { get; set; }

        public SDKContext(DbContextOptions options) : base(options)
        {
            ChangeTracker.LazyLoadingEnabled = false;
        }

        public void SetProvider(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public void ResetConcurrencyValues(Object entity, Object UpdatedEntity)
        {
            var lEntry = this.Entry(entity);
            foreach (var lProperty in lEntry.Metadata.GetProperties().Where(x => x.IsConcurrencyToken))
            {
                lEntry.OriginalValues[lProperty] = UpdatedEntity.GetType().GetProperty(lProperty.Name).GetValue(UpdatedEntity);
            }
        }

        public EnumDBType GetEnumDBType()
        {
            var dataBaseProvider = this.Database.ProviderName;
            if (dataBaseProvider.Contains("SqlServer"))
            {
                return EnumDBType.SQLServer;
            }
            else if (dataBaseProvider.Contains("PostgreSQL"))
            {
                return EnumDBType.PostgreSQL;
            }
            else
            {
                return EnumDBType.Unknown;
            }
        }
        public int SaveSystemChanges()
        {
            return InternalSaveChanges(false);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(InternalSaveChanges(true));
        }

        public override int SaveChanges()
        {
            return InternalSaveChanges(true);
        }

        internal int InternalSaveChanges(bool ValidateUser = true)
        {
            JwtUserData CurrentUser = null;

            
            if (ValidateUser)
            {
                if (ServiceProvider != null)
                {
                    IAuthenticationService authService = (IAuthenticationService)ServiceProvider.GetService(typeof(IAuthenticationService));
                    if (authService?.User != null)
                    {
                        CurrentUser = authService.User;
                    }
                }
                if (CurrentUser == null)
                {
                    throw new Exception("Invalid User");
                }

                foreach (var entry in ChangeTracker.Entries())
                {
                    //Check if the entry inherits from the BaseAudit<> class
                    if (Utilities.IsAssignableToGenericType(entry.Entity.GetType(), typeof(BaseAudit<>)))
                    {
                        var loggedUser = CurrentUser.Rowid; //TODO: Get logged user

                        entry.Context.Entry(entry.Entity).Property("LastUpdateDate").CurrentValue = DateTime.Now;
                        entry.Context.Entry(entry.Entity).Property("RowidUserLastUpdate").CurrentValue = loggedUser;

                        if (entry.State == EntityState.Added)
                        {
                            entry.Context.Entry(entry.Entity).Property("RowidUserCreates").CurrentValue = loggedUser;
                            //if created date is not set, set it to now
                            if (entry.Context.Entry(entry.Entity).Property("CreationDate").CurrentValue == null)
                            {
                                entry.Context.Entry(entry.Entity).Property("CreationDate").CurrentValue = DateTime.Now;
                            }
                        }
                    }
                    else if (Utilities.IsAssignableToGenericType(entry.Entity.GetType(), typeof(BaseCompanyGroup<>)))
                    {
                        entry.Context.Entry(entry.Entity).Property("RowidCompanyGroup").CurrentValue = CurrentUser.RowidCompanyGroup;
                    }
                }
            }
            else
            {
                foreach (var entry in ChangeTracker.Entries())
                {
                    if (Utilities.IsAssignableToGenericType(entry.Entity.GetType(), typeof(BaseAudit<>)))
                    {
                        entry.Context.Entry(entry.Entity).Property("LastUpdateDate").CurrentValue = DateTime.Now;
                        if (entry.State == EntityState.Added && entry.Context.Entry(entry.Entity).Property("CreationDate").CurrentValue == null)
                        {
                            entry.Context.Entry(entry.Entity).Property("CreationDate").CurrentValue = DateTime.Now;
                        }
                    }
                }
            }

            foreach (var Field in ChangeTracker.Entries())
            {
                var Properties = Field.Entity.GetType().GetProperties()
                    .Where(x=> x.GetCustomAttributes(typeof(SDKDataEncrypt)).Any())
                    .Select(x=> x);
                try
                {
                    if (Properties.Count() > 0)
                    {
                        var originalEntity = Field.OriginalValues.ToObject();
                        foreach (var Property in Properties)
                        {
                            var propertyValue =  Property.GetValue(Field.Entity);
                            if (propertyValue == null || (propertyValue is string && string.IsNullOrEmpty(propertyValue.ToString())))
                            {
                                var originalValue = originalEntity.GetType().GetProperty(Property.Name).GetValue(originalEntity);
                                if (originalValue != null)
                                {
                                    Field.Context.Entry(Field.Entity).Property(Property.Name).CurrentValue = originalValue;
                                }
                                continue;
                            }
                            var EncryptValue = SDKDataEncrypt.FieldEncrypt(propertyValue);
                            Field.Context.Entry(Field.Entity).Property(Property.Name).CurrentValue = EncryptValue;
                        }
                    }
                }
                catch (System.Exception)
                {  
                    
                }
                
            }
            LogCreator logCreator = ActivatorUtilities.CreateInstance<LogCreator>(ServiceProvider, ChangeTracker.Entries());
            //LogCreator logCreator = new(ChangeTracker.Entries());
            logCreator.ProccessBeforeSaveChanges();
            var result = base.SaveChanges();
            logCreator.ProccessAfterSaveChanges();
            _ = CollectChanges(logCreator);
            return result;
        }


        private async Task CollectChanges(LogCreator logCreator)
        {
            try
            {
                if (ServiceProvider != null)
                {
                    IConfiguration configuration = (IConfiguration)ServiceProvider.GetService(typeof(IConfiguration));
                    await Task.Run(() => LogService.SaveDataEntityLog(logCreator.DataEntityLogs, configuration));
                }
            }
            catch (Exception) { }
        }

        public DbSet<TEntity> AllSet<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }

        public override DbSet<TEntity> Set<TEntity>()
        {
            if (ServiceProvider != null)
            {
                try
                {
                    return ActivatorUtilities.CreateInstance(ServiceProvider, typeof(DbSetProxy<TEntity>), new object[] { this, base.Set<TEntity>() }) as DbSet<TEntity>;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{e.Message}***********");
                }
            }
            return new DbSetProxy<TEntity>(null, this, base.Set<TEntity>());
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
            if (modelBuilder == null)
                throw new ArgumentNullException("modelBuilder");

            modelBuilder.AddNamingConvention();
            modelBuilder.AddRemoveOneToManyCascadeConvention();
            var dbEnumType = GetEnumDBType();
            if (dbEnumType == EnumDBType.PostgreSQL)
            {
                modelBuilder.AddConcurrencyTokenConvention();
                modelBuilder.RemoveRowVersionConvention();
            }
            else if (dbEnumType == EnumDBType.SQLServer)
            {
                modelBuilder.RemoveXMINConvention();
            }
            modelBuilder.ApplyConventions();
            base.OnModelCreating(modelBuilder);
        }

    }
}
