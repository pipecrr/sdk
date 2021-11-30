using System;
using System.Collections.Generic;

namespace Siesa.SDK.Shared.Logs.DataChangeLog
{
    public class DataEntityLog
    {
        public string GUID { get; set; } = string.Empty;
        public string UserID { get; set; } 
        public string SessionID { get; set; }
        public string Operation { get; set; }
        public DateTime OperationDate { get; set; }
        public string EntityName { get; set; }
        public List<KeyValue> KeyValues { get; set; } 
        public List<LogProperty> Properties { get; set; }

    }
}
