using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Siesa.SDK.Shared.DataAnnotations;
using Siesa.SDK.Shared.Logs.DataChangeLog;

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

        public enum LogType
        {
            Add,
            Modify,
            Delete
        }

        private LogCreator() { }

        public LogCreator(IEnumerable<EntityEntry> entityEntries)
        {
            _entityEntriesAdded = entityEntries.Where(e => e.State == EntityState.Added && e.Entity.GetType().GetCustomAttributes(typeof(SDKLogEntity), false).Any()).ToList();
            _entityEntriesModified = entityEntries.Where(e => e.State == EntityState.Modified && e.Entity.GetType().GetCustomAttributes(typeof(SDKLogEntity), false).Any()).ToList();
            _entityEntriesDeleted = entityEntries.Where(e => e.State == EntityState.Deleted && e.Entity.GetType().GetCustomAttributes(typeof(SDKLogEntity), false).Any()).ToList();
            _dataEntityLogs = new List<DataEntityLog>();
        }

        internal void ProccessAfterSaveChanges()
        {
            if (!AreThereEntitiesToProcessAfterSaveChanges())
            {
                return;
            }
            _dataEntityLogs.AddRange(ProccessList(LogType.Add));
        }

        internal void ProccessBeforeSaveChanges()
        {
            if (!AreThereEntitiesToProcessBeforeSaveChanges())
            {
                return;
            }
            _dataEntityLogs.AddRange(ProccessList(LogType.Modify));
            _dataEntityLogs.AddRange(ProccessList(LogType.Delete));
        }

        private List<DataEntityLog> ProccessList(LogType type)
        {
            var result = new List<DataEntityLog>();
            var listToProcess = GetListToProcess(type);
            foreach (var change in listToProcess)
            {
                var properties = GetProperties(change, type);
                if (properties.Count == 0)
                {
                    continue;
                }
                result.Add(CreateDataEntityLogFromChange(change, type, properties));
            }
            return result;
        }

        private static DataEntityLog CreateDataEntityLogFromChange(EntityEntry change, LogType type, List<LogProperty> logProperties)
        {
            return new DataEntityLog()
            {
                GUID = Guid.NewGuid().ToString(),
                EntityName = change.Metadata.Name,
                UserID = "undefined",
                SessionID = "undefined",
                Operation = type.ToString(),
                OperationDate = DateTime.Now,
                KeyValues = GetKeyValues(change),
                Properties = logProperties
            };
        }

        private static List<LogProperty> GetProperties(EntityEntry change, LogType type)
        {
            var result = new List<LogProperty>();
            var propertiesList = FilterProperties(change, type);
            foreach (var property in propertiesList)
            {
                result.Add(GetLogPropertyFromPropertyEntry(property, type));
            }
            return result;
        }

        private static LogProperty GetLogPropertyFromPropertyEntry(PropertyEntry property, LogType type)
        {
            var logProperty = new LogProperty
            {
                Name = property.Metadata.Name
            };
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
                    break;
            }
            return logProperty;
        }

        private static IEnumerable<PropertyEntry> FilterProperties(EntityEntry change, LogType type)
        {
            var result = change.Properties;
            if (type == LogType.Modify)
            {
                result = result.Where(p => p.IsModified);
            }
            var logEntity = change.Entity.GetType().GetCustomAttributes(typeof(SDKLogEntity), false).FirstOrDefault() as SDKLogEntity;
            if (logEntity != null && logEntity.Fields.Length > 0)
            {
                result = result.Where(p => logEntity.Fields.Contains(p.Metadata.Name));
            }

            return result;
        }

        private static List<KeyValue> GetKeyValues(EntityEntry change)
        {
            var keyValues = new List<KeyValue>();
            var keyFields = change.Metadata.FindPrimaryKey().Properties;
            foreach (var field in keyFields)
            {
                keyValues.Add(new KeyValue()
                {
                    PropertyName = field.Name,
                    PropertyValue = change.OriginalValues[field]?.ToString()
                });
            }
            return keyValues;
        }

        public List<EntityEntry> GetListToProcess(LogType type)
        {
            return type switch
            {
                LogType.Add => _entityEntriesAdded,
                LogType.Modify => _entityEntriesModified,
                LogType.Delete => _entityEntriesDeleted,
                _ => new List<EntityEntry>(),
            };
        }

        private bool AreThereEntitiesToProcessAfterSaveChanges()
        {
            return _entityEntriesAdded.Count > 0;
        }

        private bool AreThereEntitiesToProcessBeforeSaveChanges()
        {
            return _entityEntriesDeleted.Count > 0
                || _entityEntriesModified.Count > 0;
        }


    }
}
