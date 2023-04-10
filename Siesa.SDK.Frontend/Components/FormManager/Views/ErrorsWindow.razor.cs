using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Frontend.Components.FormManager.Model;
using Siesa.SDK.Shared.DTOS;

namespace Siesa.SDK.Frontend.Components.FormManager.Views
{
    public partial class ErrorsWindow {
        [Inject] public IAuthenticationService AuthenticationService { get; set; }
        [Inject] public UtilsManager UtilsManager { get; set; }
        [Parameter] public EditContext EditFormContext { get; set; }
        [Parameter] public string ErrorMsg { get; set; }
        private bool _detailVisible = false;
        private int _errorCount = 0;
        private string ClassError = "sdk_error_log_box_sup";

        private List<SDKErrorsWindowDTO> _errors = new List<SDKErrorsWindowDTO>();

        private List<string> _errorgGeneral = new List<string>();
        protected override async Task OnParametersSetAsync(){
            if (EditFormContext != null){
                _errorCount = EditFormContext.GetValidationMessages().Count();
                var groupErrors = EditFormContext.GetValidationMessages().GroupBy(x => x.Split("//")[1]);
                foreach (var item in groupErrors){
                    var field = UtilsManager.GetResource(item.Key).Result;
                    var listErrors = item.Select(x => {
                        var errorTag = x.Split("//");
                        var resourceTag = errorTag[0];
                        var resourceTagValue = UtilsManager.GetResource(resourceTag).Result;
                        var errorSkip = errorTag.Skip(1).ToArray();
                        if(errorSkip[0].Equals(item.Key)){
                            errorSkip[0] = field;
                        }
                        string errorFormat = errorFormat = string.Format(resourceTagValue, errorSkip);
                        return errorFormat;
                    }).ToList();
                    _errors.Add(new SDKErrorsWindowDTO(){
                        Field = field,
                        Errors = listErrors
                    });
                }
            }
            if(!string.IsNullOrEmpty(ErrorMsg)) {
                _errorCount += 1;
            }
            if (_errorCount > 0){
                ClassError = "sdk_error_log_box_sup sdk_error_log_show";
            }else{
                ClassError = "sdk_error_log_box_sup";
            }
            base.OnParametersSetAsync();
        }

        public void showDedtail(){
            if(_detailVisible){
                ClassError = "sdk_error_log_box_sup sdk_error_log_show";
                _detailVisible = false;
            }else{
                ClassError = "sdk_error_log_box_sup sdk_error_log_show sdk_error_log_show_detail";
                _detailVisible = true;
            }
        }
    }
}
