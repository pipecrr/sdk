using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Siesa.SDK.Backend.Extensions
{
    public static class ContextExtensions
    {
        private static string ToSnakeCase(string text)
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

        private static List<Action<IMutableEntityType>> Conventions=new List<Action<IMutableEntityType>>();
        public static void AddNamingConvention(this ModelBuilder builder)
        {
            Conventions.Add(et => {
                string table_name = ToSnakeCase(et.ShortName().Trim());
                // table names
                builder.Entity(et.Name).ToTable(table_name);
                var table_name_parts = table_name.Split('_');
                //get text before first underscore
                var prefix = table_name_parts[0];
                //if first character is an "U" then replaceit with "CU"
                if (prefix.StartsWith("u"))
                {
                    prefix = "cu" + prefix.Substring(1);
                }else if (prefix.StartsWith("d"))
                {
                    prefix = "cd" + prefix.Substring(1);
                }else{
                    prefix = "c" + prefix.Substring(1);
                }

                // properties
                foreach (var property in et.GetProperties())
                {
                    var column_name = property.GetColumnName(StoreObjectIdentifier.Table(table_name, et.GetSchema())).Trim();
                    column_name = ToSnakeCase(column_name);
                    
                    //Check if property is a foreign key and it ends with "rowid"
                    if (property.IsForeignKey() && column_name.EndsWith("rowid")) {
                        column_name = column_name.Substring(0, column_name.Length - 6);
                        column_name = "rowid_" + column_name;
                    }
                    property.SetColumnName(prefix + "_" + column_name);
                }
            });

        }

        public static void AddRemoveOneToManyCascadeConvention(this ModelBuilder builder)
        {
            Conventions.Add(et => et.GetForeignKeys()
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade)
                .ToList()
                .ForEach(fk => fk.DeleteBehavior = DeleteBehavior.Restrict));
        }

        public static void RemoveXMINConvention(this ModelBuilder builder)
        {
            Conventions.Add(et => {
                var xmin_property = et.FindProperty("xmin");
                if(xmin_property != null)
                {
                    builder.Entity(et.Name).Ignore(xmin_property.Name);
                }
            });
        }
        public static void RemoveRowVersionConvention(this ModelBuilder builder)
        {
            Conventions.Add(et => {
                var rowversion_property = et.FindProperty("RowVersion");
                if(rowversion_property != null)
                {
                    builder.Entity(et.Name).Ignore(rowversion_property.Name);
                }
            });
        }

        public static void AddConcurrencyTokenConvention(this ModelBuilder builder)
        {
            Conventions.Add(et => {
                var xmin_property = et.FindProperty("xmin");
                if(xmin_property != null)
                {
                    // xmin_property.IsConcurrencyToken = true;
                    // xmin_property.SetColumnType("xid");
                    // xmin_property.ValueGenerated = ValueGenerated.OnAddOrUpdate;

                    builder.Entity(et.Name).Property(xmin_property.Name).HasColumnName("xmin").HasColumnType("xid").ValueGeneratedOnAddOrUpdate().IsConcurrencyToken();
                }
            });
        }
        
        public static void ApplyConventions(this ModelBuilder builder)
        {
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                foreach(Action<IMutableEntityType> action in Conventions)
                    action(entityType);
            }

            Conventions.Clear();
        }
    }
    
}