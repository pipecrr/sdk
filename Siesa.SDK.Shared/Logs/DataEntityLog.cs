using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siesa.SDK.Shared.Log
{
    public class DataEntityLog
    {
        public string ID { get; set; } = string.Empty;
        public string UserID { get; set; } 
        public string SessionID { get; set; }
        public string Operation { get; set; }
        public string EntityName { get; set; }
        public List<KeyValue> KeyValues { get; set; } 
        public List<LogProperty> Properties { get; set; }

    }
}
