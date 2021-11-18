using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Siesa.SDK.Shared.Log;
using Microsoft.EntityFrameworkCore;

namespace Siesa.SDK.Backend.Access
{
    internal class LogCreator
    {
        
        
        private readonly List<EntityEntry> _entityEntriesAdded;
        private readonly List<EntityEntry> _entityEntriesModified;
        private readonly List<EntityEntry> _entityEntriesDeleted;
        private readonly List<DataEntityLog> _dataEntityLogs;

        public List<DataEntityLog> DataEntityLogs
        {
            get
            {
                return _dataEntityLogs;
            }
        }

        private enum LogType
        {
            Add,
            Modify,
            Delete
        }

        private LogCreator() { }

        public LogCreator(List<EntityEntry> entityEntries)
        {
            _entityEntriesAdded = entityEntries.Where(e => e.State == EntityState.Added).ToList();
            _entityEntriesModified = entityEntries.Where(e => e.State == EntityState.Modified).ToList();
            _entityEntriesDeleted = entityEntries.Where(e => e.State == EntityState.Deleted).ToList();
            _dataEntityLogs = new List<DataEntityLog>();
        }

        internal void ProccessAfterSaveChanges()
        {
            _dataEntityLogs.AddRange(ProccessList(LogType.Add));
        }

        internal void ProccessBeforeSaveChanges()
        {
            _dataEntityLogs.AddRange(ProccessList(LogType.Modify));
            _dataEntityLogs.AddRange(ProccessList(LogType.Delete));
        }

        private List<DataEntityLog> ProccessList(LogType type)
        {
            var result = new List<DataEntityLog>();
            var listToProcess = GetListToProcess(type);
            foreach (var change in listToProcess)
            {
                result.Add(
                    new DataEntityLog()
                    {
                        ID = Guid.NewGuid(),
                        UserID = "undefined",
                        SessionID = "undefined",
                        Operation = type.ToString(),
                        KeyValues = GetKeyValues(change),
                        Properties = GetProperties(change, type)
                    });
            }
            return result;
        }

        private static List<LogProperty> GetProperties(EntityEntry change, LogType type)
        {
            var result = new List<LogProperty>();
            var propertiesList = FilterProperties(change, type);
            foreach(var property in propertiesList)
            {
                result.Add(GetLogPropertyFromPropertyEntry(property, type));
            }
            return result;
        }

        private static LogProperty GetLogPropertyFromPropertyEntry(PropertyEntry property, LogType type)
        {
            switch (type)
            {
                case LogType.Add:
                    var logPropertyAdded = new LogPropertyAdded
                    {
                        Name = property.Metadata.Name,
                        Value = property.CurrentValue?.ToString()
                    };
                    return logPropertyAdded;
                case LogType.Modify:
                    var logPropertyModified = new LogPropertyModified
                    {
                        Name = property.Metadata.Name,
                        OldValue = property.OriginalValue?.ToString(),
                        CurrentValue = property.CurrentValue?.ToString()
                    };
                    return logPropertyModified;
                case LogType.Delete:
                    var logPropertyDeleted = new LogPropertyDeleted
                    {
                        Name = property.Metadata.Name,
                        Value = property.CurrentValue?.ToString()
                    };
                    return logPropertyDeleted;
            }
            return new LogProperty();
        }

        private static IEnumerable<PropertyEntry> FilterProperties(EntityEntry change, LogType type)
        {
            if (type == LogType.Modify)
            {
                return change.Properties.Where(x => x.IsModified);
            }
            return change.Properties;
        }

        private static List<KeyValue> GetKeyValues(EntityEntry change)
        {
            var keyValues = new List<KeyValue>();
            var keyFields = change.Metadata.FindPrimaryKey().Properties;
            foreach(var field in keyFields)
            {
                keyValues.Add(new KeyValue()
                {
                    PropertyName = field.Name,
                    PropertyValue = change.OriginalValues[field]?.ToString()
                });
            }
            return keyValues;
        }

        private List<EntityEntry> GetListToProcess(LogType type)
        {
            return type switch
            {
                LogType.Add => _entityEntriesAdded,
                LogType.Modify => _entityEntriesModified,
                LogType.Delete => _entityEntriesDeleted,
                _ => new List<EntityEntry>(),
            };
        }


    }
}
