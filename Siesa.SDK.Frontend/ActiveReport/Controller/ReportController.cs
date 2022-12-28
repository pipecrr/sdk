using System;
using System.Data.Common;
namespace Siesa.SDK.Frontend.Report.Controllers
{
    public class SDKReportProvider: DbProviderFactory {
        

        public static SDKReportProvider Instance{get;} = new SDKReportProvider();
        private SDKReportProvider()
        {
            Console.WriteLine("prueba");
        }

        public override DbConnection CreateConnection()
        {
            return new SDKReportConnection();
        }

    }
}