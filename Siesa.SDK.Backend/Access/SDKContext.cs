using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siesa.SDK.Backend.Access
{
    public abstract class SDKContext: DbContext
    {
        private string ToSnakeCase(string input)
        {
            var returnString = "";
            foreach (var chr in input)
            {
                if (chr.ToString().ToUpper() == chr.ToString())
                {
                    // is uppercase character
                    returnString += '_' + chr.ToString().ToLower();
                }
                else
                {
                    returnString += chr;
                }
            }

            returnString = returnString.TrimStart('_');
            return returnString;
        }
        public SDKContext(DbContextOptions options) : base(options)
        {

        }

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
                //replace first character with a "c"
                prefix = "c" + prefix.Substring(1);


                // properties
                foreach (var property in entity.GetProperties())
                {
                    var column_name = property.GetColumnName().Trim().ToLower();
                    if (table_name_parts.Length > 1 && column_name == "rowid" || column_name == "id") {
                        column_name += "_" + String.Join("_", table_name_parts.Skip(1));
                    }
                    property.SetColumnName(prefix + "_" + column_name);
                }

            }
        }  
    }
}
