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

namespace Siesa.SDK.Frontend.Report.Controllers
{
    public class SaveController : IResourcesService
    {
        private GrapeCity.ActiveReports.PageReportModel.Report _report = new GrapeCity.ActiveReports.PageReportModel.Report();
        readonly DirectoryInfo _rootFolder;

        public IBackendRouterService _backendRouterService;
        public IAuthenticationService _authenticationService;

        public SaveController(IBackendRouterService backendRouterService, IAuthenticationService authenticationService)
        {
            _backendRouterService = backendRouterService;
            _authenticationService = authenticationService;
            _report = new GrapeCity.ActiveReports.PageReportModel.Report();
            
        }

        private async Task CheckUser()
        {
            if (_authenticationService.User == null)
            {
                //await _authenticationService.Initialize();
                _authenticationService.SetToken("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VyIjoie1xyXG4gIFwiUm93aWRcIjogMSxcclxuICBcIlBhdGhcIjogXCJkZW1vXCIsXHJcbiAgXCJQYXNzd29yZFJlY292ZXJ5RW1haWxcIjogXCJkZW1vQHNpZXNhLmNvbVwiLFxyXG4gIFwiTmFtZVwiOiBcIlVzdWFyaW8gZGVtb1wiLFxyXG4gIFwiSWRcIjogXCJkZW1vXCIsXHJcbiAgXCJEZXNjcmlwdGlvblwiOiBcImVzdGFzIHNvbiBsYXMgbm90YXMgZGVsIHVzYXVyaW8gZGVtb1wiLFxyXG4gIFwiUm93aWRDdWx0dXJlXCI6IDEsXHJcbiAgXCJSb2xlc1wiOiBbXHJcbiAgICB7XHJcbiAgICAgIFwiUm93aWRcIjogMixcclxuICAgICAgXCJOYW1lXCI6IFwiU2llc2EuU0RLLkVudGl0aWVzLkUwMDIyN19Vc2VyUm9sXCJcclxuICAgIH0sXHJcbiAgICB7XHJcbiAgICAgIFwiUm93aWRcIjogMyxcclxuICAgICAgXCJOYW1lXCI6IFwiU2llc2EuU0RLLkVudGl0aWVzLkUwMDIyN19Vc2VyUm9sXCJcclxuICAgIH1cclxuICBdLFxyXG4gIFwiUm93aWRDb21wYW55R3JvdXBcIjogMSxcclxuICBcIlJvd0lkREJDb25uZWN0aW9uXCI6IDEsXHJcbiAgXCJGZWF0dXJlUGVybWlzc2lvbnNcIjoge1xyXG4gICAgXCIyXCI6IHtcclxuICAgICAgXCIxXCI6IHRydWUsXHJcbiAgICAgIFwiMlwiOiB0cnVlLFxyXG4gICAgICBcIjNcIjogdHJ1ZSxcclxuICAgICAgXCI0XCI6IHRydWUsXHJcbiAgICAgIFwiNVwiOiB0cnVlXHJcbiAgICB9LFxyXG4gICAgXCIzXCI6IHtcclxuICAgICAgXCIxXCI6IHRydWUsXHJcbiAgICAgIFwiMlwiOiB0cnVlLFxyXG4gICAgICBcIjNcIjogdHJ1ZVxyXG4gICAgfSxcclxuICAgIFwiNFwiOiB7XHJcbiAgICAgIFwiMVwiOiB0cnVlLFxyXG4gICAgICBcIjJcIjogdHJ1ZSxcclxuICAgICAgXCIzXCI6IHRydWUsXHJcbiAgICAgIFwiNFwiOiB0cnVlLFxyXG4gICAgICBcIjVcIjogdHJ1ZVxyXG4gICAgfSxcclxuICAgIFwiNVwiOiB7XHJcbiAgICAgIFwiMVwiOiB0cnVlLFxyXG4gICAgICBcIjJcIjogdHJ1ZSxcclxuICAgICAgXCIzXCI6IHRydWUsXHJcbiAgICAgIFwiNFwiOiB0cnVlLFxyXG4gICAgICBcIjVcIjogdHJ1ZVxyXG4gICAgfSxcclxuICAgIFwiNlwiOiB7XHJcbiAgICAgIFwiMVwiOiB0cnVlLFxyXG4gICAgICBcIjJcIjogdHJ1ZSxcclxuICAgICAgXCIzXCI6IHRydWUsXHJcbiAgICAgIFwiNFwiOiB0cnVlLFxyXG4gICAgICBcIjVcIjogdHJ1ZVxyXG4gICAgfSxcclxuICAgIFwiN1wiOiB7XHJcbiAgICAgIFwiMVwiOiB0cnVlLFxyXG4gICAgICBcIjJcIjogdHJ1ZSxcclxuICAgICAgXCIzXCI6IHRydWUsXHJcbiAgICAgIFwiNFwiOiB0cnVlLFxyXG4gICAgICBcIjVcIjogdHJ1ZVxyXG4gICAgfSxcclxuICAgIFwiOFwiOiB7XHJcbiAgICAgIFwiMVwiOiB0cnVlLFxyXG4gICAgICBcIjJcIjogdHJ1ZSxcclxuICAgICAgXCIzXCI6IHRydWUsXHJcbiAgICAgIFwiNFwiOiB0cnVlLFxyXG4gICAgICBcIjVcIjogdHJ1ZVxyXG4gICAgfSxcclxuICAgIFwiOVwiOiB7XHJcbiAgICAgIFwiMVwiOiB0cnVlLFxyXG4gICAgICBcIjJcIjogdHJ1ZSxcclxuICAgICAgXCIzXCI6IHRydWUsXHJcbiAgICAgIFwiNFwiOiB0cnVlLFxyXG4gICAgICBcIjVcIjogdHJ1ZVxyXG4gICAgfSxcclxuICAgIFwiMTBcIjoge1xyXG4gICAgICBcIjFcIjogdHJ1ZSxcclxuICAgICAgXCIyXCI6IHRydWUsXHJcbiAgICAgIFwiM1wiOiB0cnVlLFxyXG4gICAgICBcIjRcIjogdHJ1ZSxcclxuICAgICAgXCI1XCI6IHRydWVcclxuICAgIH0sXHJcbiAgICBcIjExXCI6IHtcclxuICAgICAgXCIxXCI6IHRydWUsXHJcbiAgICAgIFwiMlwiOiB0cnVlLFxyXG4gICAgICBcIjNcIjogdHJ1ZSxcclxuICAgICAgXCI0XCI6IHRydWUsXHJcbiAgICAgIFwiNVwiOiB0cnVlXHJcbiAgICB9LFxyXG4gICAgXCIxMlwiOiB7XHJcbiAgICAgIFwiMVwiOiB0cnVlLFxyXG4gICAgICBcIjJcIjogdHJ1ZSxcclxuICAgICAgXCIzXCI6IHRydWUsXHJcbiAgICAgIFwiNFwiOiB0cnVlLFxyXG4gICAgICBcIjVcIjogdHJ1ZVxyXG4gICAgfSxcclxuICAgIFwiMTNcIjoge1xyXG4gICAgICBcIjFcIjogdHJ1ZSxcclxuICAgICAgXCIyXCI6IHRydWUsXHJcbiAgICAgIFwiM1wiOiB0cnVlLFxyXG4gICAgICBcIjRcIjogdHJ1ZSxcclxuICAgICAgXCI1XCI6IHRydWVcclxuICAgIH0sXHJcbiAgICBcIjE0XCI6IHtcclxuICAgICAgXCIxXCI6IHRydWUsXHJcbiAgICAgIFwiMlwiOiB0cnVlLFxyXG4gICAgICBcIjNcIjogdHJ1ZSxcclxuICAgICAgXCI0XCI6IHRydWUsXHJcbiAgICAgIFwiNVwiOiB0cnVlXHJcbiAgICB9LFxyXG4gICAgXCIxNlwiOiB7XHJcbiAgICAgIFwiMVwiOiB0cnVlLFxyXG4gICAgICBcIjJcIjogdHJ1ZSxcclxuICAgICAgXCIzXCI6IHRydWUsXHJcbiAgICAgIFwiNFwiOiB0cnVlLFxyXG4gICAgICBcIjVcIjogdHJ1ZVxyXG4gICAgfSxcclxuICAgIFwiMThcIjoge1xyXG4gICAgICBcIjFcIjogdHJ1ZSxcclxuICAgICAgXCIyXCI6IHRydWUsXHJcbiAgICAgIFwiM1wiOiB0cnVlLFxyXG4gICAgICBcIjRcIjogdHJ1ZSxcclxuICAgICAgXCI1XCI6IHRydWVcclxuICAgIH0sXHJcbiAgICBcIjE5XCI6IHtcclxuICAgICAgXCIxXCI6IHRydWUsXHJcbiAgICAgIFwiMlwiOiB0cnVlLFxyXG4gICAgICBcIjNcIjogdHJ1ZSxcclxuICAgICAgXCI0XCI6IHRydWUsXHJcbiAgICAgIFwiNVwiOiB0cnVlXHJcbiAgICB9LFxyXG4gICAgXCIyMFwiOiB7XHJcbiAgICAgIFwiMVwiOiB0cnVlLFxyXG4gICAgICBcIjJcIjogdHJ1ZSxcclxuICAgICAgXCIzXCI6IHRydWUsXHJcbiAgICAgIFwiNFwiOiB0cnVlLFxyXG4gICAgICBcIjVcIjogdHJ1ZVxyXG4gICAgfSxcclxuICAgIFwiMjFcIjoge1xyXG4gICAgICBcIjJcIjogdHJ1ZSxcclxuICAgICAgXCI0XCI6IHRydWVcclxuICAgIH0sXHJcbiAgICBcIjMzXCI6IHtcclxuICAgICAgXCIxXCI6IHRydWUsXHJcbiAgICAgIFwiMlwiOiB0cnVlLFxyXG4gICAgICBcIjNcIjogdHJ1ZSxcclxuICAgICAgXCI0XCI6IHRydWUsXHJcbiAgICAgIFwiNVwiOiB0cnVlXHJcbiAgICB9LFxyXG4gICAgXCIzNFwiOiB7XHJcbiAgICAgIFwiMVwiOiB0cnVlLFxyXG4gICAgICBcIjJcIjogdHJ1ZSxcclxuICAgICAgXCIzXCI6IHRydWUsXHJcbiAgICAgIFwiNFwiOiB0cnVlLFxyXG4gICAgICBcIjVcIjogdHJ1ZVxyXG4gICAgfSxcclxuICAgIFwiNDZcIjoge1xyXG4gICAgICBcIjFcIjogdHJ1ZSxcclxuICAgICAgXCIyXCI6IHRydWUsXHJcbiAgICAgIFwiM1wiOiB0cnVlLFxyXG4gICAgICBcIjRcIjogdHJ1ZSxcclxuICAgICAgXCI1XCI6IHRydWVcclxuICAgIH0sXHJcbiAgICBcIjUyXCI6IHtcclxuICAgICAgXCIxXCI6IHRydWUsXHJcbiAgICAgIFwiMlwiOiB0cnVlLFxyXG4gICAgICBcIjNcIjogdHJ1ZSxcclxuICAgICAgXCI0XCI6IHRydWUsXHJcbiAgICAgIFwiNVwiOiB0cnVlXHJcbiAgICB9LFxyXG4gICAgXCI1M1wiOiB7XHJcbiAgICAgIFwiMVwiOiB0cnVlLFxyXG4gICAgICBcIjJcIjogdHJ1ZSxcclxuICAgICAgXCIzXCI6IHRydWUsXHJcbiAgICAgIFwiNFwiOiB0cnVlLFxyXG4gICAgICBcIjVcIjogdHJ1ZVxyXG4gICAgfSxcclxuICAgIFwiNTRcIjoge1xyXG4gICAgICBcIjFcIjogdHJ1ZSxcclxuICAgICAgXCIyXCI6IHRydWUsXHJcbiAgICAgIFwiM1wiOiB0cnVlLFxyXG4gICAgICBcIjRcIjogdHJ1ZSxcclxuICAgICAgXCI1XCI6IHRydWVcclxuICAgIH0sXHJcbiAgICBcIjU1XCI6IHtcclxuICAgICAgXCIxXCI6IHRydWUsXHJcbiAgICAgIFwiMlwiOiB0cnVlLFxyXG4gICAgICBcIjNcIjogdHJ1ZSxcclxuICAgICAgXCI0XCI6IHRydWUsXHJcbiAgICAgIFwiNVwiOiB0cnVlXHJcbiAgICB9LFxyXG4gICAgXCI1NlwiOiB7XHJcbiAgICAgIFwiMVwiOiB0cnVlLFxyXG4gICAgICBcIjJcIjogdHJ1ZSxcclxuICAgICAgXCIzXCI6IHRydWUsXHJcbiAgICAgIFwiNFwiOiB0cnVlXHJcbiAgICB9LFxyXG4gICAgXCI1N1wiOiB7XHJcbiAgICAgIFwiMVwiOiB0cnVlLFxyXG4gICAgICBcIjJcIjogdHJ1ZSxcclxuICAgICAgXCIzXCI6IHRydWUsXHJcbiAgICAgIFwiNFwiOiB0cnVlLFxyXG4gICAgICBcIjVcIjogdHJ1ZVxyXG4gICAgfSxcclxuICAgIFwiNThcIjoge1xyXG4gICAgICBcIjFcIjogdHJ1ZSxcclxuICAgICAgXCIyXCI6IHRydWUsXHJcbiAgICAgIFwiM1wiOiB0cnVlLFxyXG4gICAgICBcIjRcIjogdHJ1ZSxcclxuICAgICAgXCI1XCI6IHRydWVcclxuICAgIH0sXHJcbiAgICBcIjU5XCI6IHtcclxuICAgICAgXCIxXCI6IHRydWUsXHJcbiAgICAgIFwiMlwiOiB0cnVlLFxyXG4gICAgICBcIjNcIjogdHJ1ZSxcclxuICAgICAgXCI0XCI6IHRydWUsXHJcbiAgICAgIFwiNVwiOiB0cnVlXHJcbiAgICB9LFxyXG4gICAgXCI2MVwiOiB7XHJcbiAgICAgIFwiMVwiOiB0cnVlLFxyXG4gICAgICBcIjJcIjogdHJ1ZSxcclxuICAgICAgXCIzXCI6IHRydWUsXHJcbiAgICAgIFwiNFwiOiB0cnVlLFxyXG4gICAgICBcIjVcIjogdHJ1ZVxyXG4gICAgfSxcclxuICAgIFwiNjNcIjoge1xyXG4gICAgICBcIjFcIjogdHJ1ZSxcclxuICAgICAgXCIyXCI6IHRydWUsXHJcbiAgICAgIFwiM1wiOiB0cnVlLFxyXG4gICAgICBcIjRcIjogdHJ1ZSxcclxuICAgICAgXCI1XCI6IHRydWVcclxuICAgIH0sXHJcbiAgICBcIjY2XCI6IHtcclxuICAgICAgXCIxXCI6IHRydWUsXHJcbiAgICAgIFwiMlwiOiB0cnVlLFxyXG4gICAgICBcIjNcIjogdHJ1ZSxcclxuICAgICAgXCI0XCI6IHRydWUsXHJcbiAgICAgIFwiNVwiOiB0cnVlXHJcbiAgICB9LFxyXG4gICAgXCI2N1wiOiB7XHJcbiAgICAgIFwiMVwiOiB0cnVlLFxyXG4gICAgICBcIjJcIjogdHJ1ZSxcclxuICAgICAgXCIzXCI6IHRydWUsXHJcbiAgICAgIFwiNFwiOiB0cnVlLFxyXG4gICAgICBcIjVcIjogdHJ1ZVxyXG4gICAgfSxcclxuICAgIFwiNjhcIjoge1xyXG4gICAgICBcIjFcIjogdHJ1ZSxcclxuICAgICAgXCIyXCI6IHRydWUsXHJcbiAgICAgIFwiM1wiOiB0cnVlLFxyXG4gICAgICBcIjRcIjogdHJ1ZSxcclxuICAgICAgXCI1XCI6IHRydWVcclxuICAgIH0sXHJcbiAgICBcIjY5XCI6IHtcclxuICAgICAgXCIxXCI6IHRydWUsXHJcbiAgICAgIFwiMlwiOiB0cnVlLFxyXG4gICAgICBcIjNcIjogdHJ1ZSxcclxuICAgICAgXCI0XCI6IHRydWUsXHJcbiAgICAgIFwiNVwiOiB0cnVlXHJcbiAgICB9LFxyXG4gICAgXCI3MFwiOiB7XHJcbiAgICAgIFwiMVwiOiB0cnVlLFxyXG4gICAgICBcIjJcIjogdHJ1ZSxcclxuICAgICAgXCIzXCI6IHRydWUsXHJcbiAgICAgIFwiNFwiOiB0cnVlLFxyXG4gICAgICBcIjVcIjogdHJ1ZVxyXG4gICAgfSxcclxuICAgIFwiNzFcIjoge1xyXG4gICAgICBcIjFcIjogdHJ1ZSxcclxuICAgICAgXCIyXCI6IHRydWUsXHJcbiAgICAgIFwiM1wiOiB0cnVlLFxyXG4gICAgICBcIjRcIjogdHJ1ZSxcclxuICAgICAgXCI1XCI6IHRydWVcclxuICAgIH0sXHJcbiAgICBcIjcyXCI6IHtcclxuICAgICAgXCIxXCI6IHRydWUsXHJcbiAgICAgIFwiMlwiOiB0cnVlLFxyXG4gICAgICBcIjNcIjogdHJ1ZSxcclxuICAgICAgXCI0XCI6IHRydWUsXHJcbiAgICAgIFwiNVwiOiB0cnVlXHJcbiAgICB9LFxyXG4gICAgXCI4MFwiOiB7XHJcbiAgICAgIFwiMVwiOiB0cnVlLFxyXG4gICAgICBcIjJcIjogdHJ1ZSxcclxuICAgICAgXCIzXCI6IHRydWUsXHJcbiAgICAgIFwiNFwiOiB0cnVlLFxyXG4gICAgICBcIjVcIjogdHJ1ZVxyXG4gICAgfSxcclxuICAgIFwiODFcIjoge1xyXG4gICAgICBcIjFcIjogdHJ1ZSxcclxuICAgICAgXCIyXCI6IHRydWUsXHJcbiAgICAgIFwiM1wiOiB0cnVlLFxyXG4gICAgICBcIjRcIjogdHJ1ZSxcclxuICAgICAgXCI1XCI6IHRydWVcclxuICAgIH0sXHJcbiAgICBcIjgyXCI6IHtcclxuICAgICAgXCIxXCI6IHRydWUsXHJcbiAgICAgIFwiMlwiOiB0cnVlLFxyXG4gICAgICBcIjNcIjogdHJ1ZSxcclxuICAgICAgXCI0XCI6IHRydWUsXHJcbiAgICAgIFwiNVwiOiB0cnVlXHJcbiAgICB9LFxyXG4gICAgXCI4NlwiOiB7XHJcbiAgICAgIFwiMVwiOiB0cnVlLFxyXG4gICAgICBcIjJcIjogdHJ1ZSxcclxuICAgICAgXCIzXCI6IHRydWUsXHJcbiAgICAgIFwiNFwiOiB0cnVlLFxyXG4gICAgICBcIjVcIjogdHJ1ZVxyXG4gICAgfSxcclxuICAgIFwiODdcIjoge1xyXG4gICAgICBcIjFcIjogdHJ1ZSxcclxuICAgICAgXCIyXCI6IHRydWUsXHJcbiAgICAgIFwiM1wiOiB0cnVlLFxyXG4gICAgICBcIjRcIjogdHJ1ZSxcclxuICAgICAgXCI1XCI6IHRydWVcclxuICAgIH0sXHJcbiAgICBcIjkwXCI6IHtcclxuICAgICAgXCIxXCI6IHRydWUsXHJcbiAgICAgIFwiMlwiOiB0cnVlLFxyXG4gICAgICBcIjNcIjogdHJ1ZSxcclxuICAgICAgXCI0XCI6IHRydWUsXHJcbiAgICAgIFwiNVwiOiB0cnVlXHJcbiAgICB9LFxyXG4gICAgXCI5MVwiOiB7XHJcbiAgICAgIFwiMVwiOiB0cnVlLFxyXG4gICAgICBcIjJcIjogdHJ1ZSxcclxuICAgICAgXCIzXCI6IHRydWUsXHJcbiAgICAgIFwiNFwiOiB0cnVlLFxyXG4gICAgICBcIjVcIjogdHJ1ZVxyXG4gICAgfSxcclxuICAgIFwiOTJcIjoge1xyXG4gICAgICBcIjFcIjogdHJ1ZSxcclxuICAgICAgXCIyXCI6IHRydWUsXHJcbiAgICAgIFwiM1wiOiB0cnVlLFxyXG4gICAgICBcIjRcIjogdHJ1ZSxcclxuICAgICAgXCI1XCI6IHRydWVcclxuICAgIH0sXHJcbiAgICBcIjkzXCI6IHtcclxuICAgICAgXCIxXCI6IHRydWUsXHJcbiAgICAgIFwiMlwiOiB0cnVlLFxyXG4gICAgICBcIjNcIjogdHJ1ZSxcclxuICAgICAgXCI0XCI6IHRydWUsXHJcbiAgICAgIFwiNVwiOiB0cnVlXHJcbiAgICB9LFxyXG4gICAgXCI5NVwiOiB7XHJcbiAgICAgIFwiMVwiOiB0cnVlLFxyXG4gICAgICBcIjJcIjogdHJ1ZSxcclxuICAgICAgXCIzXCI6IHRydWUsXHJcbiAgICAgIFwiNFwiOiB0cnVlLFxyXG4gICAgICBcIjVcIjogdHJ1ZVxyXG4gICAgfSxcclxuICAgIFwiMTA4XCI6IHtcclxuICAgICAgXCIxXCI6IHRydWUsXHJcbiAgICAgIFwiMlwiOiB0cnVlLFxyXG4gICAgICBcIjNcIjogdHJ1ZSxcclxuICAgICAgXCI0XCI6IHRydWUsXHJcbiAgICAgIFwiNVwiOiB0cnVlXHJcbiAgICB9LFxyXG4gICAgXCIxMDlcIjoge1xyXG4gICAgICBcIjFcIjogdHJ1ZSxcclxuICAgICAgXCIyXCI6IHRydWUsXHJcbiAgICAgIFwiM1wiOiB0cnVlLFxyXG4gICAgICBcIjRcIjogdHJ1ZSxcclxuICAgICAgXCI1XCI6IHRydWVcclxuICAgIH0sXHJcbiAgICBcIjExNVwiOiB7XHJcbiAgICAgIFwiMVwiOiB0cnVlLFxyXG4gICAgICBcIjJcIjogdHJ1ZSxcclxuICAgICAgXCIzXCI6IHRydWUsXHJcbiAgICAgIFwiNFwiOiB0cnVlLFxyXG4gICAgICBcIjVcIjogdHJ1ZVxyXG4gICAgfSxcclxuICAgIFwiMTE4XCI6IHtcclxuICAgICAgXCIxXCI6IHRydWUsXHJcbiAgICAgIFwiMlwiOiB0cnVlLFxyXG4gICAgICBcIjNcIjogdHJ1ZSxcclxuICAgICAgXCI0XCI6IHRydWUsXHJcbiAgICAgIFwiNVwiOiB0cnVlXHJcbiAgICB9LFxyXG4gICAgXCIxMTlcIjoge1xyXG4gICAgICBcIjFcIjogdHJ1ZSxcclxuICAgICAgXCIyXCI6IHRydWUsXHJcbiAgICAgIFwiM1wiOiB0cnVlLFxyXG4gICAgICBcIjRcIjogdHJ1ZSxcclxuICAgICAgXCI1XCI6IHRydWVcclxuICAgIH0sXHJcbiAgICBcIjEyMlwiOiB7XHJcbiAgICAgIFwiMVwiOiB0cnVlLFxyXG4gICAgICBcIjJcIjogdHJ1ZSxcclxuICAgICAgXCIzXCI6IHRydWUsXHJcbiAgICAgIFwiNFwiOiB0cnVlLFxyXG4gICAgICBcIjVcIjogdHJ1ZVxyXG4gICAgfSxcclxuICAgIFwiMTIzXCI6IHtcclxuICAgICAgXCIxXCI6IHRydWUsXHJcbiAgICAgIFwiMlwiOiB0cnVlLFxyXG4gICAgICBcIjNcIjogdHJ1ZSxcclxuICAgICAgXCI0XCI6IHRydWUsXHJcbiAgICAgIFwiNVwiOiB0cnVlXHJcbiAgICB9LFxyXG4gICAgXCIxMjRcIjoge1xyXG4gICAgICBcIjFcIjogdHJ1ZSxcclxuICAgICAgXCIyXCI6IHRydWUsXHJcbiAgICAgIFwiM1wiOiB0cnVlLFxyXG4gICAgICBcIjRcIjogdHJ1ZSxcclxuICAgICAgXCI1XCI6IHRydWVcclxuICAgIH0sXHJcbiAgICBcIjEyN1wiOiB7XHJcbiAgICAgIFwiMVwiOiB0cnVlLFxyXG4gICAgICBcIjJcIjogdHJ1ZSxcclxuICAgICAgXCIzXCI6IHRydWUsXHJcbiAgICAgIFwiNFwiOiB0cnVlLFxyXG4gICAgICBcIjVcIjogdHJ1ZVxyXG4gICAgfSxcclxuICAgIFwiMTMyXCI6IHtcclxuICAgICAgXCIxXCI6IHRydWUsXHJcbiAgICAgIFwiMlwiOiB0cnVlLFxyXG4gICAgICBcIjNcIjogdHJ1ZSxcclxuICAgICAgXCI0XCI6IHRydWUsXHJcbiAgICAgIFwiNVwiOiB0cnVlXHJcbiAgICB9LFxyXG4gICAgXCIxMzNcIjoge1xyXG4gICAgICBcIjFcIjogdHJ1ZSxcclxuICAgICAgXCIyXCI6IHRydWUsXHJcbiAgICAgIFwiM1wiOiB0cnVlLFxyXG4gICAgICBcIjRcIjogdHJ1ZSxcclxuICAgICAgXCI1XCI6IHRydWVcclxuICAgIH0sXHJcbiAgICBcIjEzNFwiOiB7XHJcbiAgICAgIFwiMVwiOiB0cnVlLFxyXG4gICAgICBcIjJcIjogdHJ1ZSxcclxuICAgICAgXCIzXCI6IHRydWUsXHJcbiAgICAgIFwiNFwiOiB0cnVlLFxyXG4gICAgICBcIjVcIjogdHJ1ZVxyXG4gICAgfSxcclxuICAgIFwiMTM1XCI6IHtcclxuICAgICAgXCIxXCI6IHRydWUsXHJcbiAgICAgIFwiMlwiOiB0cnVlLFxyXG4gICAgICBcIjNcIjogdHJ1ZSxcclxuICAgICAgXCI0XCI6IHRydWUsXHJcbiAgICAgIFwiNVwiOiB0cnVlXHJcbiAgICB9LFxyXG4gICAgXCIxMzZcIjoge1xyXG4gICAgICBcIjFcIjogdHJ1ZSxcclxuICAgICAgXCIyXCI6IHRydWUsXHJcbiAgICAgIFwiM1wiOiB0cnVlLFxyXG4gICAgICBcIjRcIjogdHJ1ZSxcclxuICAgICAgXCI1XCI6IHRydWVcclxuICAgIH0sXHJcbiAgICBcIjEzN1wiOiB7XHJcbiAgICAgIFwiMVwiOiB0cnVlLFxyXG4gICAgICBcIjJcIjogdHJ1ZSxcclxuICAgICAgXCIzXCI6IHRydWUsXHJcbiAgICAgIFwiNFwiOiB0cnVlLFxyXG4gICAgICBcIjVcIjogdHJ1ZVxyXG4gICAgfSxcclxuICAgIFwiMTM5XCI6IHtcclxuICAgICAgXCIxXCI6IHRydWUsXHJcbiAgICAgIFwiMlwiOiB0cnVlLFxyXG4gICAgICBcIjNcIjogdHJ1ZSxcclxuICAgICAgXCI0XCI6IHRydWUsXHJcbiAgICAgIFwiNVwiOiB0cnVlXHJcbiAgICB9LFxyXG4gICAgXCIxNDBcIjoge1xyXG4gICAgICBcIjFcIjogdHJ1ZSxcclxuICAgICAgXCIyXCI6IHRydWUsXHJcbiAgICAgIFwiM1wiOiB0cnVlLFxyXG4gICAgICBcIjRcIjogdHJ1ZSxcclxuICAgICAgXCI1XCI6IHRydWVcclxuICAgIH0sXHJcbiAgICBcIjE0MVwiOiB7XHJcbiAgICAgIFwiMVwiOiB0cnVlLFxyXG4gICAgICBcIjJcIjogdHJ1ZSxcclxuICAgICAgXCIzXCI6IHRydWUsXHJcbiAgICAgIFwiNFwiOiB0cnVlLFxyXG4gICAgICBcIjVcIjogdHJ1ZVxyXG4gICAgfSxcclxuICAgIFwiMTQyXCI6IHtcclxuICAgICAgXCIxXCI6IHRydWUsXHJcbiAgICAgIFwiMlwiOiB0cnVlLFxyXG4gICAgICBcIjNcIjogdHJ1ZSxcclxuICAgICAgXCI0XCI6IHRydWUsXHJcbiAgICAgIFwiNVwiOiB0cnVlXHJcbiAgICB9LFxyXG4gICAgXCIxNDNcIjoge1xyXG4gICAgICBcIjFcIjogdHJ1ZSxcclxuICAgICAgXCIyXCI6IHRydWUsXHJcbiAgICAgIFwiM1wiOiB0cnVlLFxyXG4gICAgICBcIjRcIjogdHJ1ZSxcclxuICAgICAgXCI1XCI6IHRydWVcclxuICAgIH0sXHJcbiAgICBcIjE0NFwiOiB7XHJcbiAgICAgIFwiMVwiOiB0cnVlLFxyXG4gICAgICBcIjJcIjogdHJ1ZSxcclxuICAgICAgXCIzXCI6IHRydWUsXHJcbiAgICAgIFwiNFwiOiB0cnVlLFxyXG4gICAgICBcIjVcIjogdHJ1ZVxyXG4gICAgfSxcclxuICAgIFwiMTQ1XCI6IHtcclxuICAgICAgXCIxXCI6IHRydWUsXHJcbiAgICAgIFwiMlwiOiB0cnVlLFxyXG4gICAgICBcIjNcIjogdHJ1ZSxcclxuICAgICAgXCI0XCI6IHRydWUsXHJcbiAgICAgIFwiNVwiOiB0cnVlXHJcbiAgICB9LFxyXG4gICAgXCIxNDlcIjoge1xyXG4gICAgICBcIjFcIjogdHJ1ZSxcclxuICAgICAgXCIyXCI6IHRydWUsXHJcbiAgICAgIFwiM1wiOiB0cnVlLFxyXG4gICAgICBcIjRcIjogdHJ1ZSxcclxuICAgICAgXCI1XCI6IHRydWVcclxuICAgIH0sXHJcbiAgICBcIjE1MFwiOiB7XHJcbiAgICAgIFwiMVwiOiB0cnVlLFxyXG4gICAgICBcIjJcIjogdHJ1ZSxcclxuICAgICAgXCIzXCI6IHRydWUsXHJcbiAgICAgIFwiNFwiOiB0cnVlLFxyXG4gICAgICBcIjVcIjogdHJ1ZVxyXG4gICAgfSxcclxuICAgIFwiMTUyXCI6IHtcclxuICAgICAgXCIxXCI6IHRydWUsXHJcbiAgICAgIFwiMlwiOiB0cnVlLFxyXG4gICAgICBcIjNcIjogdHJ1ZSxcclxuICAgICAgXCI0XCI6IHRydWUsXHJcbiAgICAgIFwiNVwiOiB0cnVlXHJcbiAgICB9LFxyXG4gICAgXCIxNTNcIjoge1xyXG4gICAgICBcIjFcIjogdHJ1ZSxcclxuICAgICAgXCIyXCI6IHRydWUsXHJcbiAgICAgIFwiM1wiOiB0cnVlLFxyXG4gICAgICBcIjRcIjogdHJ1ZSxcclxuICAgICAgXCI1XCI6IHRydWVcclxuICAgIH0sXHJcbiAgICBcIjE1NFwiOiB7XHJcbiAgICAgIFwiMVwiOiB0cnVlLFxyXG4gICAgICBcIjJcIjogdHJ1ZSxcclxuICAgICAgXCIzXCI6IHRydWUsXHJcbiAgICAgIFwiNFwiOiB0cnVlLFxyXG4gICAgICBcIjVcIjogdHJ1ZVxyXG4gICAgfSxcclxuICAgIFwiMTU1XCI6IHtcclxuICAgICAgXCIxXCI6IHRydWUsXHJcbiAgICAgIFwiMlwiOiB0cnVlLFxyXG4gICAgICBcIjNcIjogdHJ1ZSxcclxuICAgICAgXCI0XCI6IHRydWUsXHJcbiAgICAgIFwiNVwiOiB0cnVlXHJcbiAgICB9LFxyXG4gICAgXCIxNTZcIjoge1xyXG4gICAgICBcIjFcIjogdHJ1ZSxcclxuICAgICAgXCIyXCI6IHRydWUsXHJcbiAgICAgIFwiM1wiOiB0cnVlLFxyXG4gICAgICBcIjRcIjogdHJ1ZSxcclxuICAgICAgXCI1XCI6IHRydWVcclxuICAgIH0sXHJcbiAgICBcIjE1OFwiOiB7XHJcbiAgICAgIFwiMVwiOiB0cnVlLFxyXG4gICAgICBcIjJcIjogdHJ1ZSxcclxuICAgICAgXCIzXCI6IHRydWUsXHJcbiAgICAgIFwiNFwiOiB0cnVlLFxyXG4gICAgICBcIjVcIjogdHJ1ZVxyXG4gICAgfSxcclxuICAgIFwiMTU5XCI6IHtcclxuICAgICAgXCIxXCI6IHRydWUsXHJcbiAgICAgIFwiMlwiOiB0cnVlLFxyXG4gICAgICBcIjNcIjogdHJ1ZSxcclxuICAgICAgXCI0XCI6IHRydWUsXHJcbiAgICAgIFwiNVwiOiB0cnVlXHJcbiAgICB9LFxyXG4gICAgXCIxNjFcIjoge1xyXG4gICAgICBcIjFcIjogdHJ1ZSxcclxuICAgICAgXCIyXCI6IHRydWUsXHJcbiAgICAgIFwiM1wiOiB0cnVlLFxyXG4gICAgICBcIjRcIjogdHJ1ZSxcclxuICAgICAgXCI1XCI6IHRydWVcclxuICAgIH0sXHJcbiAgICBcIjE2MlwiOiB7XHJcbiAgICAgIFwiMVwiOiB0cnVlLFxyXG4gICAgICBcIjJcIjogdHJ1ZSxcclxuICAgICAgXCIzXCI6IHRydWUsXHJcbiAgICAgIFwiNFwiOiB0cnVlLFxyXG4gICAgICBcIjVcIjogdHJ1ZVxyXG4gICAgfSxcclxuICAgIFwiMTY5XCI6IHtcclxuICAgICAgXCIxXCI6IHRydWUsXHJcbiAgICAgIFwiMlwiOiB0cnVlLFxyXG4gICAgICBcIjNcIjogdHJ1ZSxcclxuICAgICAgXCI0XCI6IHRydWUsXHJcbiAgICAgIFwiNVwiOiB0cnVlXHJcbiAgICB9LFxyXG4gICAgXCIxNzNcIjoge1xyXG4gICAgICBcIjFcIjogdHJ1ZSxcclxuICAgICAgXCIyXCI6IHRydWUsXHJcbiAgICAgIFwiM1wiOiB0cnVlLFxyXG4gICAgICBcIjRcIjogdHJ1ZSxcclxuICAgICAgXCI1XCI6IHRydWVcclxuICAgIH0sXHJcbiAgICBcIjE3NVwiOiB7XHJcbiAgICAgIFwiMVwiOiB0cnVlLFxyXG4gICAgICBcIjJcIjogdHJ1ZSxcclxuICAgICAgXCIzXCI6IHRydWUsXHJcbiAgICAgIFwiNFwiOiB0cnVlLFxyXG4gICAgICBcIjVcIjogdHJ1ZVxyXG4gICAgfSxcclxuICAgIFwiMTc3XCI6IHtcclxuICAgICAgXCIxXCI6IHRydWUsXHJcbiAgICAgIFwiMlwiOiB0cnVlLFxyXG4gICAgICBcIjNcIjogdHJ1ZSxcclxuICAgICAgXCI0XCI6IHRydWUsXHJcbiAgICAgIFwiNVwiOiB0cnVlXHJcbiAgICB9LFxyXG4gICAgXCIxODdcIjoge1xyXG4gICAgICBcIjFcIjogdHJ1ZSxcclxuICAgICAgXCIyXCI6IHRydWUsXHJcbiAgICAgIFwiM1wiOiB0cnVlLFxyXG4gICAgICBcIjRcIjogdHJ1ZSxcclxuICAgICAgXCI1XCI6IHRydWVcclxuICAgIH0sXHJcbiAgICBcIjE4OFwiOiB7XHJcbiAgICAgIFwiMVwiOiB0cnVlLFxyXG4gICAgICBcIjJcIjogdHJ1ZSxcclxuICAgICAgXCIzXCI6IHRydWUsXHJcbiAgICAgIFwiNFwiOiB0cnVlLFxyXG4gICAgICBcIjVcIjogdHJ1ZVxyXG4gICAgfSxcclxuICAgIFwiMTk3XCI6IHtcclxuICAgICAgXCIxXCI6IHRydWUsXHJcbiAgICAgIFwiMlwiOiB0cnVlLFxyXG4gICAgICBcIjNcIjogdHJ1ZSxcclxuICAgICAgXCI0XCI6IHRydWUsXHJcbiAgICAgIFwiNVwiOiB0cnVlXHJcbiAgICB9LFxyXG4gICAgXCIxOTlcIjoge1xyXG4gICAgICBcIjFcIjogdHJ1ZSxcclxuICAgICAgXCIyXCI6IHRydWUsXHJcbiAgICAgIFwiM1wiOiB0cnVlLFxyXG4gICAgICBcIjRcIjogdHJ1ZSxcclxuICAgICAgXCI1XCI6IHRydWVcclxuICAgIH0sXHJcbiAgICBcIjIwMFwiOiB7XHJcbiAgICAgIFwiMVwiOiB0cnVlLFxyXG4gICAgICBcIjJcIjogdHJ1ZSxcclxuICAgICAgXCIzXCI6IHRydWUsXHJcbiAgICAgIFwiNFwiOiB0cnVlLFxyXG4gICAgICBcIjVcIjogdHJ1ZVxyXG4gICAgfSxcclxuICAgIFwiMjAxXCI6IHtcclxuICAgICAgXCIxXCI6IHRydWUsXHJcbiAgICAgIFwiMlwiOiB0cnVlLFxyXG4gICAgICBcIjNcIjogdHJ1ZSxcclxuICAgICAgXCI0XCI6IHRydWUsXHJcbiAgICAgIFwiNVwiOiB0cnVlXHJcbiAgICB9LFxyXG4gICAgXCIyNVwiOiB7XHJcbiAgICAgIFwiMVwiOiB0cnVlLFxyXG4gICAgICBcIjJcIjogdHJ1ZSxcclxuICAgICAgXCIzXCI6IHRydWUsXHJcbiAgICAgIFwiNFwiOiB0cnVlLFxyXG4gICAgICBcIjVcIjogdHJ1ZVxyXG4gICAgfSxcclxuICAgIFwiMjZcIjoge1xyXG4gICAgICBcIjFcIjogdHJ1ZSxcclxuICAgICAgXCIyXCI6IHRydWUsXHJcbiAgICAgIFwiM1wiOiB0cnVlLFxyXG4gICAgICBcIjRcIjogdHJ1ZSxcclxuICAgICAgXCI1XCI6IHRydWVcclxuICAgIH0sXHJcbiAgICBcIjI3XCI6IHtcclxuICAgICAgXCIxXCI6IHRydWUsXHJcbiAgICAgIFwiMlwiOiB0cnVlLFxyXG4gICAgICBcIjNcIjogdHJ1ZSxcclxuICAgICAgXCI0XCI6IHRydWUsXHJcbiAgICAgIFwiNVwiOiB0cnVlXHJcbiAgICB9LFxyXG4gICAgXCIzMFwiOiB7XHJcbiAgICAgIFwiMVwiOiB0cnVlLFxyXG4gICAgICBcIjJcIjogdHJ1ZSxcclxuICAgICAgXCIzXCI6IHRydWUsXHJcbiAgICAgIFwiNFwiOiB0cnVlLFxyXG4gICAgICBcIjVcIjogdHJ1ZVxyXG4gICAgfSxcclxuICAgIFwiMzFcIjoge1xyXG4gICAgICBcIjFcIjogdHJ1ZSxcclxuICAgICAgXCIyXCI6IHRydWUsXHJcbiAgICAgIFwiM1wiOiB0cnVlLFxyXG4gICAgICBcIjRcIjogdHJ1ZSxcclxuICAgICAgXCI1XCI6IHRydWVcclxuICAgIH0sXHJcbiAgICBcIjMyXCI6IHtcclxuICAgICAgXCIxXCI6IHRydWUsXHJcbiAgICAgIFwiMlwiOiB0cnVlLFxyXG4gICAgICBcIjNcIjogdHJ1ZSxcclxuICAgICAgXCI0XCI6IHRydWUsXHJcbiAgICAgIFwiNVwiOiB0cnVlXHJcbiAgICB9LFxyXG4gICAgXCIzNVwiOiB7XHJcbiAgICAgIFwiMVwiOiB0cnVlLFxyXG4gICAgICBcIjJcIjogdHJ1ZSxcclxuICAgICAgXCIzXCI6IHRydWUsXHJcbiAgICAgIFwiNFwiOiB0cnVlLFxyXG4gICAgICBcIjVcIjogdHJ1ZVxyXG4gICAgfSxcclxuICAgIFwiMzZcIjoge1xyXG4gICAgICBcIjFcIjogdHJ1ZSxcclxuICAgICAgXCIyXCI6IHRydWUsXHJcbiAgICAgIFwiM1wiOiB0cnVlLFxyXG4gICAgICBcIjRcIjogdHJ1ZSxcclxuICAgICAgXCI1XCI6IHRydWVcclxuICAgIH0sXHJcbiAgICBcIjM4XCI6IHtcclxuICAgICAgXCIxXCI6IHRydWUsXHJcbiAgICAgIFwiMlwiOiB0cnVlLFxyXG4gICAgICBcIjNcIjogdHJ1ZSxcclxuICAgICAgXCI0XCI6IHRydWUsXHJcbiAgICAgIFwiNVwiOiB0cnVlXHJcbiAgICB9LFxyXG4gICAgXCI0MFwiOiB7XHJcbiAgICAgIFwiMVwiOiB0cnVlLFxyXG4gICAgICBcIjJcIjogdHJ1ZSxcclxuICAgICAgXCIzXCI6IHRydWUsXHJcbiAgICAgIFwiNFwiOiB0cnVlLFxyXG4gICAgICBcIjVcIjogdHJ1ZVxyXG4gICAgfSxcclxuICAgIFwiNDFcIjoge1xyXG4gICAgICBcIjFcIjogdHJ1ZSxcclxuICAgICAgXCIyXCI6IHRydWUsXHJcbiAgICAgIFwiM1wiOiB0cnVlLFxyXG4gICAgICBcIjRcIjogdHJ1ZSxcclxuICAgICAgXCI1XCI6IHRydWVcclxuICAgIH0sXHJcbiAgICBcIjQzXCI6IHtcclxuICAgICAgXCIxXCI6IHRydWUsXHJcbiAgICAgIFwiMlwiOiB0cnVlLFxyXG4gICAgICBcIjNcIjogdHJ1ZSxcclxuICAgICAgXCI0XCI6IHRydWUsXHJcbiAgICAgIFwiNVwiOiB0cnVlXHJcbiAgICB9LFxyXG4gICAgXCI0NVwiOiB7XHJcbiAgICAgIFwiMVwiOiB0cnVlLFxyXG4gICAgICBcIjJcIjogdHJ1ZSxcclxuICAgICAgXCIzXCI6IHRydWUsXHJcbiAgICAgIFwiNFwiOiB0cnVlLFxyXG4gICAgICBcIjVcIjogdHJ1ZVxyXG4gICAgfSxcclxuICAgIFwiNDdcIjoge1xyXG4gICAgICBcIjFcIjogdHJ1ZSxcclxuICAgICAgXCIyXCI6IHRydWUsXHJcbiAgICAgIFwiM1wiOiB0cnVlLFxyXG4gICAgICBcIjRcIjogdHJ1ZSxcclxuICAgICAgXCI1XCI6IHRydWVcclxuICAgIH0sXHJcbiAgICBcIjQ4XCI6IHtcclxuICAgICAgXCIxXCI6IHRydWUsXHJcbiAgICAgIFwiMlwiOiB0cnVlLFxyXG4gICAgICBcIjNcIjogdHJ1ZSxcclxuICAgICAgXCI0XCI6IHRydWUsXHJcbiAgICAgIFwiNVwiOiB0cnVlXHJcbiAgICB9LFxyXG4gICAgXCI0OVwiOiB7XHJcbiAgICAgIFwiMVwiOiB0cnVlLFxyXG4gICAgICBcIjJcIjogdHJ1ZSxcclxuICAgICAgXCIzXCI6IHRydWUsXHJcbiAgICAgIFwiNFwiOiB0cnVlLFxyXG4gICAgICBcIjVcIjogdHJ1ZVxyXG4gICAgfSxcclxuICAgIFwiMTExXCI6IHtcclxuICAgICAgXCIxXCI6IHRydWUsXHJcbiAgICAgIFwiMlwiOiB0cnVlLFxyXG4gICAgICBcIjNcIjogdHJ1ZSxcclxuICAgICAgXCI0XCI6IHRydWUsXHJcbiAgICAgIFwiNVwiOiB0cnVlXHJcbiAgICB9LFxyXG4gICAgXCIxMTJcIjoge1xyXG4gICAgICBcIjFcIjogdHJ1ZSxcclxuICAgICAgXCIyXCI6IHRydWUsXHJcbiAgICAgIFwiM1wiOiB0cnVlLFxyXG4gICAgICBcIjRcIjogdHJ1ZSxcclxuICAgICAgXCI1XCI6IHRydWVcclxuICAgIH0sXHJcbiAgICBcIjExNlwiOiB7XHJcbiAgICAgIFwiMVwiOiB0cnVlLFxyXG4gICAgICBcIjJcIjogdHJ1ZSxcclxuICAgICAgXCIzXCI6IHRydWUsXHJcbiAgICAgIFwiNFwiOiB0cnVlLFxyXG4gICAgICBcIjVcIjogdHJ1ZVxyXG4gICAgfSxcclxuICAgIFwiMTE3XCI6IHtcclxuICAgICAgXCIxXCI6IHRydWUsXHJcbiAgICAgIFwiMlwiOiB0cnVlLFxyXG4gICAgICBcIjNcIjogdHJ1ZSxcclxuICAgICAgXCI0XCI6IHRydWUsXHJcbiAgICAgIFwiNVwiOiB0cnVlXHJcbiAgICB9LFxyXG4gICAgXCIxMjBcIjoge1xyXG4gICAgICBcIjFcIjogdHJ1ZSxcclxuICAgICAgXCIyXCI6IHRydWUsXHJcbiAgICAgIFwiM1wiOiB0cnVlLFxyXG4gICAgICBcIjRcIjogdHJ1ZSxcclxuICAgICAgXCI1XCI6IHRydWVcclxuICAgIH0sXHJcbiAgICBcIjEyMVwiOiB7XHJcbiAgICAgIFwiMVwiOiB0cnVlLFxyXG4gICAgICBcIjJcIjogdHJ1ZSxcclxuICAgICAgXCIzXCI6IHRydWUsXHJcbiAgICAgIFwiNFwiOiB0cnVlLFxyXG4gICAgICBcIjVcIjogdHJ1ZVxyXG4gICAgfSxcclxuICAgIFwiMTI2XCI6IHtcclxuICAgICAgXCIxXCI6IHRydWUsXHJcbiAgICAgIFwiMlwiOiB0cnVlLFxyXG4gICAgICBcIjNcIjogdHJ1ZSxcclxuICAgICAgXCI0XCI6IHRydWUsXHJcbiAgICAgIFwiNVwiOiB0cnVlXHJcbiAgICB9LFxyXG4gICAgXCIxMjlcIjoge1xyXG4gICAgICBcIjFcIjogdHJ1ZSxcclxuICAgICAgXCIyXCI6IHRydWUsXHJcbiAgICAgIFwiM1wiOiB0cnVlLFxyXG4gICAgICBcIjRcIjogdHJ1ZSxcclxuICAgICAgXCI1XCI6IHRydWVcclxuICAgIH0sXHJcbiAgICBcIjEzMFwiOiB7XHJcbiAgICAgIFwiMVwiOiB0cnVlLFxyXG4gICAgICBcIjJcIjogdHJ1ZSxcclxuICAgICAgXCIzXCI6IHRydWUsXHJcbiAgICAgIFwiNFwiOiB0cnVlLFxyXG4gICAgICBcIjVcIjogdHJ1ZVxyXG4gICAgfSxcclxuICAgIFwiMTY1XCI6IHtcclxuICAgICAgXCIxXCI6IHRydWUsXHJcbiAgICAgIFwiMlwiOiB0cnVlLFxyXG4gICAgICBcIjNcIjogdHJ1ZSxcclxuICAgICAgXCI0XCI6IHRydWUsXHJcbiAgICAgIFwiNVwiOiB0cnVlXHJcbiAgICB9LFxyXG4gICAgXCIxNjhcIjoge1xyXG4gICAgICBcIjFcIjogdHJ1ZSxcclxuICAgICAgXCIyXCI6IHRydWUsXHJcbiAgICAgIFwiM1wiOiB0cnVlLFxyXG4gICAgICBcIjRcIjogdHJ1ZSxcclxuICAgICAgXCI1XCI6IHRydWVcclxuICAgIH0sXHJcbiAgICBcIjE3MFwiOiB7XHJcbiAgICAgIFwiMVwiOiB0cnVlLFxyXG4gICAgICBcIjJcIjogdHJ1ZSxcclxuICAgICAgXCIzXCI6IHRydWUsXHJcbiAgICAgIFwiNFwiOiB0cnVlLFxyXG4gICAgICBcIjVcIjogdHJ1ZVxyXG4gICAgfSxcclxuICAgIFwiMTc2XCI6IHtcclxuICAgICAgXCIxXCI6IHRydWUsXHJcbiAgICAgIFwiMlwiOiB0cnVlLFxyXG4gICAgICBcIjNcIjogdHJ1ZSxcclxuICAgICAgXCI0XCI6IHRydWUsXHJcbiAgICAgIFwiNVwiOiB0cnVlXHJcbiAgICB9LFxyXG4gICAgXCIxODBcIjoge1xyXG4gICAgICBcIjFcIjogdHJ1ZSxcclxuICAgICAgXCIyXCI6IHRydWUsXHJcbiAgICAgIFwiM1wiOiB0cnVlLFxyXG4gICAgICBcIjRcIjogdHJ1ZSxcclxuICAgICAgXCI1XCI6IHRydWVcclxuICAgIH0sXHJcbiAgICBcIjE4MVwiOiB7XHJcbiAgICAgIFwiMVwiOiB0cnVlLFxyXG4gICAgICBcIjJcIjogdHJ1ZSxcclxuICAgICAgXCIzXCI6IHRydWUsXHJcbiAgICAgIFwiNFwiOiB0cnVlLFxyXG4gICAgICBcIjVcIjogdHJ1ZVxyXG4gICAgfSxcclxuICAgIFwiMTgyXCI6IHtcclxuICAgICAgXCIxXCI6IHRydWUsXHJcbiAgICAgIFwiMlwiOiB0cnVlLFxyXG4gICAgICBcIjNcIjogdHJ1ZSxcclxuICAgICAgXCI0XCI6IHRydWUsXHJcbiAgICAgIFwiNVwiOiB0cnVlXHJcbiAgICB9LFxyXG4gICAgXCIxODNcIjoge1xyXG4gICAgICBcIjFcIjogdHJ1ZSxcclxuICAgICAgXCIyXCI6IHRydWUsXHJcbiAgICAgIFwiM1wiOiB0cnVlLFxyXG4gICAgICBcIjRcIjogdHJ1ZSxcclxuICAgICAgXCI1XCI6IHRydWVcclxuICAgIH0sXHJcbiAgICBcIjE4NFwiOiB7XHJcbiAgICAgIFwiMVwiOiB0cnVlLFxyXG4gICAgICBcIjJcIjogdHJ1ZSxcclxuICAgICAgXCIzXCI6IHRydWUsXHJcbiAgICAgIFwiNFwiOiB0cnVlLFxyXG4gICAgICBcIjVcIjogdHJ1ZVxyXG4gICAgfSxcclxuICAgIFwiMTg1XCI6IHtcclxuICAgICAgXCIxXCI6IHRydWUsXHJcbiAgICAgIFwiMlwiOiB0cnVlLFxyXG4gICAgICBcIjNcIjogdHJ1ZSxcclxuICAgICAgXCI0XCI6IHRydWUsXHJcbiAgICAgIFwiNVwiOiB0cnVlXHJcbiAgICB9LFxyXG4gICAgXCIxODZcIjoge1xyXG4gICAgICBcIjFcIjogdHJ1ZSxcclxuICAgICAgXCIyXCI6IHRydWUsXHJcbiAgICAgIFwiM1wiOiB0cnVlLFxyXG4gICAgICBcIjRcIjogdHJ1ZSxcclxuICAgICAgXCI1XCI6IHRydWVcclxuICAgIH0sXHJcbiAgICBcIjE4OVwiOiB7XHJcbiAgICAgIFwiMVwiOiB0cnVlLFxyXG4gICAgICBcIjJcIjogdHJ1ZSxcclxuICAgICAgXCIzXCI6IHRydWUsXHJcbiAgICAgIFwiNFwiOiB0cnVlLFxyXG4gICAgICBcIjVcIjogdHJ1ZVxyXG4gICAgfSxcclxuICAgIFwiMTkwXCI6IHtcclxuICAgICAgXCIxXCI6IHRydWUsXHJcbiAgICAgIFwiMlwiOiB0cnVlLFxyXG4gICAgICBcIjNcIjogdHJ1ZSxcclxuICAgICAgXCI0XCI6IHRydWUsXHJcbiAgICAgIFwiNVwiOiB0cnVlXHJcbiAgICB9LFxyXG4gICAgXCIxOTFcIjoge1xyXG4gICAgICBcIjFcIjogdHJ1ZSxcclxuICAgICAgXCIyXCI6IHRydWUsXHJcbiAgICAgIFwiM1wiOiB0cnVlLFxyXG4gICAgICBcIjRcIjogdHJ1ZSxcclxuICAgICAgXCI1XCI6IHRydWVcclxuICAgIH0sXHJcbiAgICBcIjE5MlwiOiB7XHJcbiAgICAgIFwiMVwiOiB0cnVlLFxyXG4gICAgICBcIjJcIjogdHJ1ZSxcclxuICAgICAgXCIzXCI6IHRydWUsXHJcbiAgICAgIFwiNFwiOiB0cnVlLFxyXG4gICAgICBcIjVcIjogdHJ1ZVxyXG4gICAgfSxcclxuICAgIFwiMTkzXCI6IHtcclxuICAgICAgXCIxXCI6IHRydWUsXHJcbiAgICAgIFwiMlwiOiB0cnVlLFxyXG4gICAgICBcIjNcIjogdHJ1ZSxcclxuICAgICAgXCI0XCI6IHRydWUsXHJcbiAgICAgIFwiNVwiOiB0cnVlXHJcbiAgICB9LFxyXG4gICAgXCIxOTRcIjoge1xyXG4gICAgICBcIjFcIjogdHJ1ZSxcclxuICAgICAgXCIyXCI6IHRydWUsXHJcbiAgICAgIFwiM1wiOiB0cnVlLFxyXG4gICAgICBcIjRcIjogdHJ1ZSxcclxuICAgICAgXCI1XCI6IHRydWVcclxuICAgIH1cclxuICB9XHJcbn0iLCJuYmYiOjE2NzQ0ODg0ODcsImV4cCI6MTY3NDQ5NTY4NywiaWF0IjoxNjc0NDg4NDg3fQ.UHwBSyMiJBco3gqntrZbYYUWDpKqrkfImFlHQvyUEj4");
            }
        }

        public GrapeCity.ActiveReports.PageReportModel.Report GetReport(string id)
        {
            CheckUser().GetAwaiter().GetResult();
            //return _report; 
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
            CheckUser().GetAwaiter().GetResult();
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
            CheckUser().GetAwaiter().GetResult();
            byte[] xmlContent = ReportConverter.ToXml(report);

            _report = report;

            var BL = _backendRouterService.GetSDKBusinessModel("BLSDKReport", _authenticationService);
            if (BL != null)
            {
                //bool response = false;
                if (isTemporary)
                {
                    name += $"temp_{Guid.NewGuid().ToString()}.rdlx";
                }
                var result = BL.Call("SaveReport", xmlContent, name, isTemporary).GetAwaiter().GetResult();
                //response = result.Data;    

            }

            return name;

        }

        public string UpdateReport(string id, GrapeCity.ActiveReports.PageReportModel.Report report)
        {

            throw new NotImplementedException();
        }

        public void DeleteReport(string id)
        {
            CheckUser().GetAwaiter().GetResult();
            if (!id.StartsWith("temp_"))
            {
                var BL = _backendRouterService.GetSDKBusinessModel("BLSDKReport", _authenticationService);

                if (BL != null)
                {
                    var result = BL.Call("DeleteReport", id).GetAwaiter().GetResult();
                }
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
            return new IThemeInfo[0];
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
