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
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Shared.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Siesa.SDK.Frontend.Report.Controllers
{
    public class SaveController : IResourcesService
    {
        public IBackendRouterService _backendRouterService;
        public IAuthenticationService _authenticationService;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public SaveController(IBackendRouterService backendRouterService, IAuthenticationService authenticationService, IHttpContextAccessor httpContextAccessor)
        {
            _backendRouterService = backendRouterService;
            _authenticationService = authenticationService;
            _httpContextAccessor = httpContextAccessor;
        }

        private async Task _setValidUserToken()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            
            var sessionId = "";
            string authToken = "";

            httpContext.Request.Cookies.TryGetValue("sdksession", out sessionId);
            if (!string.IsNullOrEmpty(sessionId))
            {
                var BLSession = _backendRouterService.GetSDKBusinessModel("BLSession", _authenticationService);
                var response = await BLSession.Call("GetSession", sessionId);
                if (response.Success)
                {
                    authToken = response.Data;
                }
            }
            if (!string.IsNullOrEmpty(authToken))
            {
                await _authenticationService.SetToken(authToken, false);
            }
        }

        private void ValidateUser()
        {
            _setValidUserToken().GetAwaiter().GetResult();
            /*if (string.IsNullOrEmpty(_authenticationService.UserToken))
            {
                 _setValidUserToken().GetAwaiter().GetResult();
            }else
            {
                if (_authenticationService.User == null)
                {
                    _setValidUserToken().GetAwaiter().GetResult();
                }
            }*/
        }

        public GrapeCity.ActiveReports.PageReportModel.Report GetReport(string id)
        {
            ValidateUser();
            GrapeCity.ActiveReports.PageReportModel.Report report = new GrapeCity.ActiveReports.PageReportModel.Report();

            var BL = _backendRouterService.GetSDKBusinessModel("BLSDKReport", _authenticationService);

            if (BL != null)
            {
                var result = BL.Call("GetReport", id).GetAwaiter().GetResult();
                if (result.Data != null)
                {
                    string content = result.Data.Content;

                    byte[] ContentArray = Encoding.UTF8.GetBytes(content);
                    report = ReportConverter.FromXML(ContentArray);
                }
            }
            return report;
        }


        public IReportInfo[] GetReportsList()
        {
            ValidateUser();
            var reportsList = new List<ReportInfo>();
            var BL = _backendRouterService.GetSDKBusinessModel("BLSDKReport", _authenticationService);

            if (BL != null)
            {
                var result = BL.Call("GetReportList").GetAwaiter().GetResult();
                if (result.Data != null)
                {
                    foreach (var item in result.Data)
                    {
                        reportsList.Add(new ReportInfo
                        {
                            Id = item.Id,
                            Name = item.Id,
                            Type = "rdlx",
                        });
                    }
                }
            }

            return reportsList.ToArray();
        }

        public string SaveReport(string name, GrapeCity.ActiveReports.PageReportModel.Report report, bool isTemporary = false)
        {
             ValidateUser();
            byte[] xmlContent = ReportConverter.ToXml(report);

            var BL = _backendRouterService.GetSDKBusinessModel("BLSDKReport", _authenticationService);
            if (BL != null)
            {
                if (isTemporary)
                {
                    name = $"temp_{Guid.NewGuid().ToString()}.rdlx";
                }
                var result = BL.Call("SaveReport", xmlContent, name, isTemporary).GetAwaiter().GetResult();

            }

            return name;

        }

        public string UpdateReport(string id, GrapeCity.ActiveReports.PageReportModel.Report report)
        {
             ValidateUser();

            byte[] xmlContent = ReportConverter.ToXml(report);

            var BL = _backendRouterService.GetSDKBusinessModel("BLSDKReport", _authenticationService);
            if (BL != null)
            {
                BL.Call("UpdateReport", xmlContent, id).GetAwaiter().GetResult();
            }

            return id;
        }

        public void DeleteReport(string id)
        {
            ValidateUser();
            if (id.StartsWith("temp_"))
            {
                var BL = _backendRouterService.GetSDKBusinessModel("BLSDKReport", _authenticationService);
                if (BL != null)
                {
                    var result = BL.Call("DeleteReport", id,_authenticationService.User.Rowid).GetAwaiter().GetResult();
                }
            }
        }

        public Uri GetBaseUri()
        {

            return new Uri("https://example.com/reports");
        }


        public Theme GetTheme(string id)
        {

            return new Theme();
        }


        public IThemeInfo[] GetThemesList()
        {
            return new IThemeInfo[0];
        }


        public byte[] GetImage(string id, out string mimeType)
        {
            mimeType = "image/png";

            return new byte[0];
        }


        public IImageInfo[] GetImagesList()
        {

            return new IImageInfo[0];
        }
    }

    public class ReportInfo : IReportInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }

}
