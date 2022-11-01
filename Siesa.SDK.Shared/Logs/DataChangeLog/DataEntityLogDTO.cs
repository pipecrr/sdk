using System;
using System.Collections.Generic;
using Siesa.SDK.Entities;

namespace Siesa.SDK.Shared.Logs.DataChangeLog
{
    public class DataEntityLogDTO: IBaseSDK
    {
        public string GUID { get; set; } = string.Empty;
        public string UserRowId { get; set; }
        public string UserName { get; set; }
        public string SessionID { get; set; }
        public string Operation { get; set; }
        public DateTime OperationDate { get; set; }
        public string EntityName { get; set; }
        public string KeyValue { get; set; }
        public string PropertyName { get; set; }
        public string OldValue { get; set; }
        public string CurrentValue { get; set; }
        public bool CheckRowid(long rowid)
        {
            throw new NotImplementedException();
        }

        public long GetRowid()
        {
            throw new NotImplementedException();
        }

        public void SetRowid(long rowid)
        {
            throw new NotImplementedException();
        }
    }
}
