using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Siesa.SDK.Frontend.Utils;

public static class FrontendUtils
{
    public static async Task<IJSObjectReference> ImportJsModule(this IJSRuntime jsRuntime, string modulePath)
    {
        string localResourceRoot = string.Empty;
        try
        { 
            localResourceRoot = await jsRuntime.InvokeAsync<string>("getLocalResourceRoot").ConfigureAwait(true);
        }catch(Exception)
        {
        }
        if (string.IsNullOrEmpty(localResourceRoot))
        {
            localResourceRoot = ".";
        }
        //remove last slash if exists
        if (localResourceRoot.EndsWith('/'))
        {
            localResourceRoot = localResourceRoot.Substring(0, localResourceRoot.Length - 1);
        }
        
        //remove "./" if exists in modulePath
        if (modulePath != null && modulePath.StartsWith("./", StringComparison.InvariantCulture))
        {
            modulePath = modulePath.Substring(2);
        }
        
        //remove first slash if exists
        if (modulePath != null && modulePath.StartsWith("/", StringComparison.InvariantCulture))
        {
            modulePath = modulePath.Substring(1);
        }
        var response = jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", $"{localResourceRoot}/{modulePath}");
        return await response.ConfigureAwait(true);
    }
}