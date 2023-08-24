using System;
using System.Text;

namespace Siesa.SDK.Shared.DTOS
{
    public class QueueMessageDTO 
    {
        public string QueueName { get; set; }
        public string Message { get; set; }
        public Int64 Rowid { get; set; }
    }
}