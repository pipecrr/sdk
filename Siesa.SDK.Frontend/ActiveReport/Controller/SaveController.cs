using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;
using Newtonsoft.Json;
using System.Linq;
using System.Xml.Serialization;
using GrapeCity.ActiveReports.Aspnetcore.Designer.Services;
using GrapeCity.ActiveReports.PageReportModel;
using GrapeCity.ActiveReports.Rdl.Json;
using GrapeCity.ActiveReports.Rdl.Persistence;
using GrapeCity.ActiveReports.Aspnetcore.Designer.Utilities;



namespace Siesa.SDK.Frontend.Report.Controllers
{
    public class SaveController : IResourcesService
    {
        private GrapeCity.ActiveReports.PageReportModel.Report _report = new GrapeCity.ActiveReports.PageReportModel.Report();
        readonly DirectoryInfo _rootFolder;

        [Inject]
        public IBackendRouterService BackendRouterService { get; set; }

        public SaveController(DirectoryInfo rootFolder)
        {
            _rootFolder = rootFolder;
            _report = new GrapeCity.ActiveReports.PageReportModel.Report();
        }

        public GrapeCity.ActiveReports.PageReportModel.Report GetReport(string id)
        {

             //var report = _report.GetReport(id);
             return _report;
        }

        public IReportInfo[] GetReportsList()
        {
            var reportsList = _rootFolder.GetFiles("*.*", SearchOption.TopDirectoryOnly)
            .Select(fileInfo => fileInfo.Name)
            .Where(x => string.Equals(Path.GetExtension(x), ".rdlx", StringComparison.InvariantCultureIgnoreCase))
            .Select(name => new ReportInfo
            {
                Id = name,
                Name = name,
                Type = "rdlx",
            }).ToArray();
            return reportsList;
           
        }
        
        public string SaveReport(string name, GrapeCity.ActiveReports.PageReportModel.Report report, bool isTemporary = false)
        {
           if (isTemporary)
            {
                string path = Path.Combine(_rootFolder.FullName, "$temp");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                string reportPath = Path.Combine(path, name + ".rdlx");
                byte[] xmlContent = ReportConverter.ToXml(report);
                File.WriteAllBytes(reportPath, xmlContent);

                return reportPath;
            }
            else
            {
                // guardado en BD
                string path = Path.Combine(_rootFolder.FullName, "$temp");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                string reportPath = Path.Combine(path, name + ".rdlx");
                byte[] xmlContent = ReportConverter.ToXml(report);
                File.WriteAllBytes(reportPath, xmlContent);
                return "zzzz";
            }
        }


        public string UpdateReport(string id, GrapeCity.ActiveReports.PageReportModel.Report report)
        {
            
            throw new NotImplementedException();
        }

        public void DeleteReport(string id)
        {
            try
            {
                File.Delete(id);
            }
            catch (Exception)
            {
                //archivo no encontrado
            }
        }

        public Uri GetBaseUri()
        {
            
            return new Uri("https://example.com/reports");
        }


        public Theme GetTheme(string id)
        {

            throw new NotImplementedException();
        }


        public IThemeInfo[] GetThemesList()
        {
            throw new NotImplementedException();
        }


        public byte[] GetImage(string id, out string mimeType)
        {
            
            throw new NotImplementedException();
        }


        public IImageInfo[] GetImagesList()
        {
        
            throw new NotImplementedException();
        }
    }

    public class ReportInfo : IReportInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }

}
