namespace Siesa.SDK.Shared.Logs.DataEventLog
{
    public class LogPropertyEvent
    {
        public string SourceContext { get; set; }
        public string RequestId { get; set; }
        public string RequestPath { get; set; }
        public string ConnectionId { get; set; }
    }
}