using Newtonsoft.Json;
using Siesa.SDK.Shared.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siesa.SDK.Shared.Logs
{
    public class LogService
    {
        public void SaveDataEntityLog(List<DataEntityLog> logs)
        {
            foreach(var log in logs) { 
                string result = JsonConvert.SerializeObject(log);
                Console.WriteLine(result);
                //TODO Call web api to storage in NOSQL Database
            }
        }

    }
}
