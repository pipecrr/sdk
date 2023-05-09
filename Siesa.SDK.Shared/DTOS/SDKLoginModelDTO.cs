using Siesa.SDK.Shared.DataAnnotations;
namespace Siesa.SDK.Shared.DTOS
{
    public class SDKLoginModelDTO
    {
        [SDKRequired]
        public string Username { get; set; }

        [SDKRequired]
        public string Password { get; set; }
    }
}