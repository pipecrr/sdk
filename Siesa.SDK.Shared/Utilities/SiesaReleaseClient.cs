using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web;
using HtmlAgilityPack;
using System.Net.Http;
using System.Threading.Tasks;

namespace Siesa.SDK.Shared.Utilities
{
    public class SiesaReleaseUser
    {
        public string Username { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
    public class SiesaReleaseClient
    {
        private string url;
        private HttpClient client;

        public SiesaReleaseClient(string? url = null)
        {
            if(string.IsNullOrEmpty(url))
            {
                url = "https://srelease.siesadev.com";
            }
            if (url.EndsWith("/"))
            {
                url = url.Substring(0, url.Length - 1);
            }
            this.url = url;
            this.client = new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                AllowAutoRedirect = true
            });
            this.client.DefaultRequestVersion = new Version(2, 0);
            this.client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
        }

        public async Task<SiesaReleaseUser?> LoginAsync(string username, string password)
        {          
            string loginUrl = $"{url}/common/c_noauthorized.aspx";
            string response = await client.GetStringAsync(loginUrl);
            HtmlDocument soup = new HtmlDocument();
            soup.LoadHtml(response);

            // Parse the HTML response to extract the viewstate and any other hidden fields
            string viewstate = soup.DocumentNode.SelectSingleNode("//input[@name='__VIEWSTATE']").Attributes["value"].Value;
            string viewstateGenerator = soup.DocumentNode.SelectSingleNode("//input[@name='__VIEWSTATEGENERATOR']").Attributes["value"].Value;

            // Build the login request payload
            var payload = new Dictionary<string, string>
            {
                {"__VIEWSTATE", viewstate},
                {"__VIEWSTATEGENERATOR", viewstateGenerator},
                {"ctl00$ContaninerLayout$ds_login", username},
                {"ctl00$ContaninerLayout$ds_password", password},
                {"ajaxaction", "login"},
                {"ajaxtarget", "WebPage_Content"},
                {"ajaxUrl", loginUrl},
                {"isModal", "False"}
            };

            // Send the login request
            HttpResponseMessage loginResponse = await client.PostAsync(loginUrl, new FormUrlEncodedContent(payload));
            string loginContent = await loginResponse.Content.ReadAsStringAsync();
            // follow redirects if any
            int redirectCount = 0;
            while (loginResponse.StatusCode == HttpStatusCode.Redirect)
            {
                if (redirectCount > 5)
                {
                    throw new Exception("Too many redirects");
                }
                loginResponse = await client.GetAsync(loginResponse.Headers.Location);
                loginContent = await loginResponse.Content.ReadAsStringAsync();
            }

            HtmlDocument loginSoup = new HtmlDocument();
            loginSoup.LoadHtml(loginContent);

            // Check if login was successful
            //"><li><a href="closesession.aspx"><span class="glyphicon glyphicon-log-out">" must be present in the response
            if (loginSoup.DocumentNode.SelectSingleNode("//input[@class='form-control-login']") == null &&
                loginSoup.DocumentNode.SelectSingleNode("//li/a[@href='closesession.aspx']") != null)
            {
                SiesaReleaseUser siesaReleaseUser = new();
                siesaReleaseUser.Username = username;
                //extract Name "<input type="hidden" id="username" name="username" value="Jonathan Toledo" />"
                siesaReleaseUser.Name = loginSoup.DocumentNode.SelectSingleNode("//input[@id='username']")
                    .Attributes["value"].Value;
                //add @siesa.com to username
                siesaReleaseUser.Email = $"{username}@siesa.com";
                return siesaReleaseUser;
            }
            return null;
        }

        // public Dictionary<string, object> GetRq(int id)
        // {
        //     string rqDetailUrl = $"{url}/users/req.aspx?i={id}";
        //     string response = session.DownloadString(rqDetailUrl);
        //     HtmlDocument soup = new HtmlDocument();
        //     soup.LoadHtml(response);

        //     // Extracting the requirement ID
        //     string requirementId = soup.GetElementbyId("id_req").Attributes["value"].Value;
        //     Console.WriteLine($"Requirement ID: {requirementId}");

        //     // Other extraction logic...

        //     return new Dictionary<string, object>
        //     {
        //         {"requirement_id", requirementId},
        //         // Other extracted data...
        //     };
        // }

        // public void AddTaskToRq(string rqId, Dictionary<string, string> task)
        // {
        //     string endpoint = $"{url}/users/req.aspx?t=Q&i={rqId}";
        //     string response = session.DownloadString(endpoint);
        //     HtmlDocument soup = new HtmlDocument();
        //     soup.LoadHtml(response);

        //     // Extracting the viewstate and viewstategenerator
        //     string viewstate = soup.DocumentNode.SelectSingleNode("//input[@name='__VIEWSTATE']").Attributes["value"].Value;
        //     string viewstateGenerator = soup.DocumentNode.SelectSingleNode("//input[@name='__VIEWSTATEGENERATOR']").Attributes["value"].Value;

        //     var payload = new Dictionary<string, string>
        //     {
        //         {"__VIEWSTATE", viewstate},
        //         {"__VIEWSTATEGENERATOR", viewstateGenerator},
        //         // Other payload data...
        //     };

        //     // Make ajax post request
        //     session.Headers.Add("X-Requested-With", "XMLHttpRequest");
        //     session.Headers.Add("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
        //     session.UploadValues(endpoint, "POST", payload);
        // }
    }
}
