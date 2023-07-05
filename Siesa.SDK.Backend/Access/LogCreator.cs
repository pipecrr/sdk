using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Siesa.SDK.Shared.DataAnnotations;
using Siesa.SDK.Shared.Logs.DataChangeLog;
using Siesa.SDK.Shared.Services;

namespace Siesa.SDK.Backend.Access
{
    /// <summary>
    /// Helper class to create data change logs for entities that have the SDKLogEntity attribute.
    /// </summary>
    internal class LogCreator
    {
        private readonly List<EntityEntry> _entityEntriesAdded;
        private readonly List<EntityEntry> _entityEntriesModified;
        private readonly List<EntityEntry> _entityEntriesDeleted;
        private readonly List<DataEntityLog> _dataEntityLogs;
        private readonly IAuthenticationService _authenticationService;

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

        /// <summary>
        /// Initializes a new instance of the LogCreator class.
        /// </summary>
        /// <param name="entityEntries">The entity entries to process.</param>
        /// <param name="authenticationService">The authentication service.</param>
        public LogCreator(IEnumerable<EntityEntry> entityEntries, IAuthenticationService authenticationService)
        {
            _entityEntriesAdded = entityEntries.Where(e => e.State == EntityState.Added && e.Entity.GetType().GetCustomAttributes(typeof(SDKLogEntity), false).Any()).ToList();
            _entityEntriesModified = entityEntries.Where(e => e.State == EntityState.Modified && e.Entity.GetType().GetCustomAttributes(typeof(SDKLogEntity), false).Any()).ToList();
            _entityEntriesDeleted = entityEntries.Where(e => e.State == EntityState.Deleted && e.Entity.GetType().GetCustomAttributes(typeof(SDKLogEntity), false).Any()).ToList();
            _dataEntityLogs = new List<DataEntityLog>();
            _authenticationService = authenticationService;
        }

        /// <summary>
        /// Processes the entity entries after changes have been saved to the database.
        /// </summary>
        internal void ProccessAfterSaveChanges()
        {
            if (!AreThereEntitiesToProcessAfterSaveChanges())
            {
                return;
            }
            _dataEntityLogs.AddRange(ProccessList(LogType.Add));
        }

        /// <summary>
        /// Processes the entity entries before changes are saved to the database.
        /// </summary>
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
                if (_authenticationService.User != null)
                {
                    result.Add(CreateDataEntityLogFromChange(change, type, properties, _authenticationService.User.Rowid, _authenticationService.User.Name));
                }
                else
                {
                    result.Add(CreateDataEntityLogFromChange(change, type, properties, 0, "undefined"));
                }

            }
            return result;
        }

        private static DataEntityLog CreateDataEntityLogFromChange(EntityEntry change, LogType type, List<LogProperty> logProperties, int RowidUserLogged, string UserNameLogged)
        {
            return new DataEntityLog()
            {
                GUID = Guid.NewGuid().ToString(),
                EntityName = change.Metadata.Name,
                UserRowId = RowidUserLogged.ToString(),
                UserName = UserNameLogged,
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