using Siesa.SDK.Shared.DataAnnotations;

namespace Siesa.SDK.Frontend.Pages.Login
{
    public class Model
    {
        [SDKRequired]
        public string Username { get; set; }

        [SDKRequired]
        public string Password { get; set; }
    }

}