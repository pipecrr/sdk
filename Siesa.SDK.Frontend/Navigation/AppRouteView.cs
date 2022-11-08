using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Siesa.SDK.Shared.DataAnnotations;
using Siesa.SDK.Shared.Services;
using System;
using System.Net;

namespace Siesa.SDK.Frontend.Navigation
{
    public class AppRouteView : RouteView
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IAuthenticationService AuthenticationService { get; set; }

        protected override void Render(RenderTreeBuilder builder)
        {
            var authorize = Attribute.GetCustomAttribute(RouteData.PageType, typeof(AuthorizeAttribute)) != null;
            if (authorize && AuthenticationService.User == null)
            {
                var externalView = Attribute.GetCustomAttribute(RouteData.PageType, typeof(SDKExternalView)) != null;
                if(externalView)
                {
                    base.Render(builder);
                    return;
                }
                
                var returnUrl = WebUtility.UrlEncode(new Uri(NavigationManager.Uri).PathAndQuery);
                NavigationManager.NavigateTo($"login?returnUrl={returnUrl}");
            }
            else
            {
                base.Render(builder);
            }
        }
    }
}