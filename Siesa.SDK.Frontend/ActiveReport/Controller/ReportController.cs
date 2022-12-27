using System;
using System.Data.Common;
namespace SDK.Frontend.ReportDesigner.Controllers
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