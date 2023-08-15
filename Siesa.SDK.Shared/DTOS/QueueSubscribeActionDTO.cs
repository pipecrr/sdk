using System;
using System.Text;

namespace Siesa.SDK.Shared.DTOS
{
    public class QueueSubscribeActionDTO 
    {
        public string Target { get; set; }
        public string MethodName { get; set; }
    }
}