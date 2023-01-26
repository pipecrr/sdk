using System;
using System.Collections.Generic;

using System.Globalization;
namespace Siesa.SDK.Frontend.Report.Controllers
{

    public partial class DataSetModel
    {

        public DataSet DataSet { get; set; }


        public DataSource DataSource { get; set; }
    }

    public partial class DataSet
    {

        public string Name { get; set; }


        public Query Query { get; set; }


        public Field[] Fields { get; set; }
    }

    public partial class Field
    {

        public string Name { get; set; }


        public string DataField { get; set; }


        public string DataType { get; set; }


        public string Aggregate { get; set; }
    }

    public partial class Query
    {

        public string CommandText { get; set; }


        public string DataSourceName { get; set; }
    }

    public partial class DataSource
    {

        public string Name { get; set; }


        public ConnectionProperties ConnectionProperties { get; set; }
    }

    public partial class ConnectionProperties
    {

        public string ConnectString { get; set; }


        public string DataProvider { get; set; }
    }
    public class Parameter
    {
        public string name { get; set; }
        public string type { get; set; }
        public bool nullable { get; set; }
        public Parameter(string name, string type, bool nullable)
        {

        this.name = name;

        this.type = type;

        this.nullable = nullable;
        }
    }
}
