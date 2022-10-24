using System;
using Siesa.SDK.Entities;

namespace Siesa.SDK.Shared.Logs.DataEventLog
{
    public class EventLogDTO: IBaseSDK
    {
        public DateTime Timestamp { get; set; }
        public int Level { get; set; }
        public string MessageTemplate { get; set; }
        public string ContextType { get; set; }
        public string NewLine { get; set; }
        public string Error { get; set; }
        public string EventId { get; set; }
        public string SourceContext { get; set; }
        public string RequestId { get; set; }
        public string RequestPath { get; set; }
        public string ConnectionId { get; set; }
        public string ExcClassName { get; set; }
        public string ExcMessage { get; set; }
        public string ExcData { get; set; }
        public string ExcInnerException { get; set; }
        public string ExcHelpUrl { get; set; }
        public string ExcStackTrace { get; set; }
        public string ExcRemoteStackTrace { get; set; }
        public string ExcRemoteStackIndex { get; set; }
        public string ExcMethod { get; set; }
        public string ExcHResult { get; set; }
        public string ExcSource { get; set; }
        public string ExcWatsonBuckets { get; set; }
        public string ExcErrors { get; set; }
        public string ExcClientConnectionId { get; set; }
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
