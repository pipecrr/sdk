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

        private readonly List<string> _entitiesToLog;

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

        public LogCreator(IEnumerable<EntityEntry> entityEntries)
        {
            _entitiesToLog = new List<string> { "Siesa.Demo.Entities.E100_Regional", "Siesa.Demo.Entities.E101_OperationCenter", "Siesa.Demo.Entities.E102_Contact" };
            _entityEntriesAdded = entityEntries.Where(e => e.State == EntityState.Added && _entitiesToLog.Any(entityName => entityName.Equals(e.Metadata.Name))).ToList();
            _entityEntriesModified = entityEntries.Where(e => e.State == EntityState.Modified && _entitiesToLog.Any(entityName => entityName.Equals(e.Metadata.Name))).ToList();
            _entityEntriesDeleted = entityEntries.Where(e => e.State == EntityState.Deleted && _entitiesToLog.Any(entityName => entityName.Equals(e.Metadata.Name))).ToList();
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
                        ID = Guid.NewGuid().ToString(),
                        EntityName = change.Metadata.Name,
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
            var logProperty = new LogProperty();
            logProperty.Name = property.Metadata.Name;
            switch (type)
            {
                case LogType.Add:
                    logProperty.CurrentValue = property.CurrentValue?.ToString();
                    break;
                case LogType.Modify:
                    logProperty.OldValue = property.OriginalValue?.ToString();
                    logProperty.CurrentValue = property.CurrentValue?.ToString();
                    break;
                case LogType.Delete:
                    logProperty.OldValue = property.OriginalValue?.ToString();
                    break ;
            }
            return logProperty;
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
