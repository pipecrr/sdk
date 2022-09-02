
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Siesa.SDK.Shared.Services;

namespace Siesa.SDK.Backend.Services
{

    public class EmailService
    {

        private IBackendRouterService _BackendRouter;
        private IAuthenticationService _AuthenticationService;

        public EmailService(IBackendRouterService backendRouter, IAuthenticationService authenticationService)
        {
            _BackendRouter = backendRouter;
            _AuthenticationService = authenticationService;
        }

        public async Task<bool> SendEmail(string subject, string body, List<string> recipients, List<string> cc = null, List<string> bcc = null)
        {
            if (cc == null)
            {
                cc = new List<string>();
            }
            if (bcc == null)
            {
                bcc = new List<string>();
            }

            var request = await _BackendRouter.GetSDKBusinessModel("BLSDKEmail", _AuthenticationService).Call("SendEmail", subject, body, recipients, cc, bcc);

            try
            {
                if (request.Success)
                {
                    return true;
                }
                else
                {
                    throw new Exception(string.Join(" ", request.Errors));
                }
            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}