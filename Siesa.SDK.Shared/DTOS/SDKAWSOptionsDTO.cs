
using Siesa.SDK.Backend.Access;

namespace Siesa.SDK.Shared.DTOS
{
    public class SDKAWSOptionsDTO
    {
        public string AccessKeyId { get; set; }
        public string SecretAccessKey { get; set; }
        public string Region { get; set; }
        public bool UseS3 { get; set; }
        public string BucketName { get; set; }
        public int TimeoutDuration { get; set; }
    }
}