using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Siesa.SDK.Shared.Logs.DataChangeLog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Siesa.SDK.Backend.Access
{
    public abstract class SDKContext: DbContext
    {
        public SDKContext(DbContextOptions options) : base(options)
        {

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)  
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                string table_name = entity.ShortName().Trim();
                // table names
                modelBuilder.Entity(entity.Name).ToTable(table_name);
                var table_name_parts = table_name.Split('_');
                //get text before first underscore
                var prefix = table_name_parts[0];
                //replace first character with a "c"
                prefix = "c" + prefix.Substring(1);


                // properties
                foreach (var property in entity.GetProperties())
                {
                    var column_name = property.GetColumnName(StoreObjectIdentifier.Table(table_name, entity.GetSchema())).Trim();
                    if (table_name_parts.Length > 1 && (column_name.ToLower() == "rowid" || column_name.ToLower() == "id")) {
                        var tmp_name = column_name;
                        column_name = String.Join("", table_name_parts.Skip(1)) + tmp_name;
                    }
                    property.SetColumnName(prefix + "_" + column_name);
                }

            }
        }  
    }
}
