using AuditAppGrpcClient;
using Newtonsoft.Json;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using Siesa.SDK.Shared.Services;

namespace Siesa.SDK.Shared.Logs.DataEventLog
{
    /*public class SDKLogEvent: LogEvent
    {
        [JsonIgnore]
        IAuthenticationService _authenticationService;
        
        public int RowidUserLogged {get {
            return  _authenticationService.User.Rowid;
        }} 


        public string UserNameLogged {get{

            return _authenticationService.User.Name;

        }}

        public SDKLogEvent(IAuthenticationService authenticationService): base(new DateTimeOffset(),LogEventLevel.Debug, null,null,null)
        {
        }

        internal void SetAuthenticationService(IAuthenticationService authService)
        {
            _authenticationService = authService;
        }
    }*/
}
