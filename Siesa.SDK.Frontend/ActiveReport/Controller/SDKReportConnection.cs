
using System;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using GrapeCity.BI.Data.DataProviders;
using GrapeCity.BI.Data.DataProviders.DataProviders;
using GrapeCity.BI.Data.DataProviders.TextProvidersBase;
using Siesa.SDK.Entities;

namespace Siesa.SDK.Frontend.Report.Controllers
{

    public class SDKReportDataReader : TextDbDataReader
    {
        public override int FieldCount => _entityType.GetProperties().Length;
        protected IEnumerable<object> _data { get; private set;}
        protected IEnumerator<object> _enumerator { get; private set;}
        protected Type _entityType { get; private set;}

        protected object Current => _enumerator.Current;

        public SDKReportDataReader(IEnumerable<object> _data, Type EntityType)
        {
            _data = _data;
            _enumerator = _data.GetEnumerator();
            _entityType = EntityType;
        }

        public override Type GetFieldType(int ordinal)
        {
            return _entityType.GetProperties()[ordinal].PropertyType;
        }

        public override string GetName(int ordinal)
        {
            return _entityType.GetProperties()[ordinal].Name;
        }

        public override int GetOrdinal(string name)
        {
            return _entityType.GetProperties().ToList().FindIndex(x => x.Name == name);
        }

        public override object GetValue(int ordinal)
        {
            return _entityType.GetProperties()[ordinal].GetValue(Current);
        }

        public override bool Read()
        {
            return _enumerator.MoveNext();
        }

        public override bool IsClosed 
        {
            get
            {
                return _enumerator == null;
            }
        }


        public override void Close()
        {
            _enumerator = null;
            _data = null;
        }
    }

    public class SDKReportCommand : TextDbCommand, IExDbCommand
    {
        private readonly string _connectionString;
        private InternalSDKReportProvider _internalSDKReportProvider;


        private readonly Func<string, Stream> _resourceLocator;

        private Func<object> _locateDataSource;

        public Func<object> LocateDataSource
        {
            get
            {
                return _locateDataSource;
            }
            set
            {
                _locateDataSource = value;
            }
        }

        public bool CanBeCached => false;

        public SDKReportCommand(string pconnectionString, Func<string, Stream> presourceLocator, InternalSDKReportProvider internalSDKReportProvider)
        {
            _resourceLocator = presourceLocator;
            _connectionString = pconnectionString;
            _internalSDKReportProvider = internalSDKReportProvider;

        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior commandBehavior)
        {
            IEnumerable<object> data = _internalSDKReportProvider.GetBLData(CommandText);
            Type type = _internalSDKReportProvider.GetBLType(CommandText);


            return new SDKReportDataReader(data, type);
        }
    }

    public class SDKReportConnection : TextDbConnection, IResourceLocatorEnabled
    {
        private string _connectionString = string.Empty;
        private Func<string, Stream> _resourceLocator;
        private InternalSDKReportProvider internalSDKReportProvider;

        public SDKReportConnection(InternalSDKReportProvider internalSDKReportProvider)
        {
            this.internalSDKReportProvider = internalSDKReportProvider;
        }

        public override string ConnectionString
        {
            get
            {
                return _connectionString;
            }
            set
            {
                _connectionString = value ?? string.Empty;
            }
        }

        public Func<string, Stream> ResourceLocator
        {
            get
            {
                return _resourceLocator;
            }
            set
            {
                _resourceLocator = value;
            }
        }

        protected override DbCommand CreateDbCommand()
        {
            return new SDKReportCommand(_connectionString, ResourceLocator, internalSDKReportProvider)
            {
                Connection = this
            };
        }
    }


}