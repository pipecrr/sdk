using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Frontend.Components.FormManager.Model;
using Siesa.SDK.Shared.DTOS;
using Microsoft.Extensions.Hosting;
using Siesa.Global.Enums;
using Siesa.SDK.Frontend.Services;
using Siesa.SDK.Frontend.Components.Fields;
using Siesa.SDK.Components.Visualization;

namespace Siesa.SDK.Frontend.Components.FormManager.Views
{
    public partial class ErrorsWindow {
        [Inject] public IAuthenticationService AuthenticationService { get; set; }
        [Inject] public UtilsManager UtilsManager { get; set; }
        [Inject] public SDKNotificationService _notificationService { get; set; }
        [Inject] public SDKDialogService _sdkDialogService { get; set; }
        [Parameter] public EditContext EditFormContext { get; set; }
        [Parameter] public bool VerifyContext { get; set; }
        [Parameter] public List<ModelMessagesDTO> MessagesDTO { get; set; } = new List<ModelMessagesDTO>();
        private bool _detailVisible = false;
        private int _errorCount = 0;
        private string ClassError = "sdk_error_log_box_sup";

        private List<SDKErrorsWindowDTO> _formErrors = new();

        private readonly string _environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        private void FormErrors()
        {
            var groupErrors = EditFormContext.GetValidationMessages().GroupBy(x => {
                var errorsSplit = x.Split("//");
                if(errorsSplit.Count() > 1)
                {
                    return errorsSplit[1];
                }else{
                    return "General";
                }
            });
            foreach (var item in groupErrors)
            {
                if(!item.Key.Equals("General",StringComparison.Ordinal))
                {
                    var field = item.Key;                        
                    Dictionary<string, object[]> errorsFormat = new();

                    var listErrors = item.Select(x => 
                    {
                        var errorTag = x.Split("//");
                        var resourceTag = errorTag[0];
                        var errorSkip = errorTag.Skip(1);

                        errorsFormat.Add(resourceTag, errorSkip.ToArray());

                        return resourceTag;
                    }).ToList();

                    _formErrors.Add(new SDKErrorsWindowDTO()
                    {
                        Field = field,
                        Errors = errorsFormat
                    });
                }
            }
            _errorCount += _formErrors.Count;
        }

        protected override async Task OnParametersSetAsync()
        {
            _errorCount = 0;
            _formErrors = new List<SDKErrorsWindowDTO>();

            MessagesDTO = MessagesDTO.GroupBy(x => x.StackTrace).Select(x => x.First()).ToList();
                
            if (EditFormContext != null && EditFormContext.GetValidationMessages().Any() && VerifyContext)
            {
                FormErrors();
            }

            _errorCount += MessagesDTO.Count(x => x.TypeMessange == EnumModelMessageType.Error);

            if (_errorCount > 0){
                ClassError = "sdk_error_log_box_sup sdk_error_log_show sdk_error_log_show_detail";
                _detailVisible = true;
            }else{
                ClassError = "sdk_error_log_box_sup";
            }
            
            await base.OnParametersSetAsync().ConfigureAwait(true);
        }

        public void showDedtail()
        {
            if(_detailVisible)
            {
                ClassError = "sdk_error_log_box_sup sdk_error_log_show";
                _detailVisible = false;
            }else
            {
                ClassError = "sdk_error_log_box_sup sdk_error_log_show sdk_error_log_show_detail";
                _detailVisible = true;
            }
        }

        
    }
}