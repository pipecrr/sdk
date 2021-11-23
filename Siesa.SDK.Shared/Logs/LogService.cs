using Newtonsoft.Json;
using Siesa.SDK.Shared.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siesa.SDK.Shared.Logs
{
    public static class LogService
    {
        public static void SaveDataEntityLog(List<DataEntityLog> logs)
        {
            Console.WriteLine("Saving DataEntityLogs");
            foreach(var log in logs) { 
                string result = JsonConvert.SerializeObject(log);
                Console.WriteLine($"LogService: {result}");
                //TODO Call web api to storage in NOSQL Database
            }
        }

    }
}
