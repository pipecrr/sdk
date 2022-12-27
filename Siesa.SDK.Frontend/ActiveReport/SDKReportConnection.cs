
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

namespace SDK.Frontend.ReportDesigner.Controllers
{

    public class SDKReportDataReader : TextDbDataReader
    {
        public override int FieldCount => 10;
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

        // public override int RecordsAffected
        // {
        //     get
        //     {
        //         return _data.Count();
        //     }
        // }

        // public override bool HasRows
        // {
        //     get
        //     {
        //         return _data != null && _data.Count() > 0;
        //     }
        // }

        public override void Close()
        {
            _enumerator = null;
            _data = null;
        }

        // public override IEnumerator GetEnumerator()
        // {
        //     return _enumerator;
        // }
    }

    public class SDKReportCommand : TextDbCommand, IExDbCommand
    {
        private readonly string _connectionString;


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

        public SDKReportCommand(string pconnectionString, Func<string, Stream> presourceLocator)
        {
            _resourceLocator = presourceLocator;
            _connectionString = pconnectionString;
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior commandBehavior)
        {
            List<E00220_User> Data = new List<E00220_User>();
            Data.Add(new E00220_User() { Rowid = 1, Id = "City 1 id", Name = "City 1 name", Description = "City 1 description"});
            Data.Add(new E00220_User() { Rowid = 2, Id = "City 2 id", Name = "City 2 name", Description = "City 2 description"});
            Data.Add(new E00220_User() { Rowid = 3, Id = "City 3 id", Name = "City 3 name", Description = "City 3 description"});
            Data.Add(new E00220_User() { Rowid = 4, Id = "City 4 id", Name = "City 4 name", Description = "City 4 description"});
            Data.Add(new E00220_User() { Rowid = 5, Id = "City 5 id", Name = "City 5 name", Description = "City 5 description"});
            Data.Add(new E00220_User() { Rowid = 6, Id = "City 6 id", Name = "City 6 name", Description = "City 6 description"});
            Data.Add(new E00220_User() { Rowid = 7, Id = "City 7 id", Name = "City 7 name", Description = "City 7 description"});
            Data.Add(new E00220_User() { Rowid = 8, Id = "City 8 id", Name = "City 8 name", Description = "City 8 description"});


            return new SDKReportDataReader(Data, typeof(E00220_User));
        }
    }

    public class SDKReportConnection : TextDbConnection, IResourceLocatorEnabled
    {
        private string _connectionString = string.Empty;
        private Func<string, Stream> _resourceLocator;
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
            return new SDKReportCommand(_connectionString, ResourceLocator)
            {
                Connection = this
            };
        }
    }


}