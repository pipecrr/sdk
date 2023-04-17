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
        [Parameter] public List<string> GeneralErrors { get; set; }
        [Parameter] public List<string> ErrorMsg { get; set; }
        private bool _detailVisible = false;
        private int _errorCount = 0;
        private string ClassError = "sdk_error_log_box_sup";

        private List<SDKErrorsWindowDTO> _errors = new List<SDKErrorsWindowDTO>();

        private List<string> _generalErrors = new List<string>();
        protected override async Task OnParametersSetAsync(){
            _errorCount = 0;
            _generalErrors = new List<string>();
            _errors = new List<SDKErrorsWindowDTO>();
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
            if(ErrorMsg?.Count() > 0) {
                _errorCount += ErrorMsg.Count();
                var groupErrors = ErrorMsg.GroupBy(x => x.Split("//")[1]);
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
            if(GeneralErrors?.Count() > 0) {
                _errorCount += GeneralErrors.Count();
                foreach (var err in GeneralErrors){
                    if(err.StartsWith("Exception: ")){
                        _generalErrors.Add(err.Replace("Exception: ", ""));
                    }else{
                        var errorMsg = await UtilsManager.GetResource(err);
                        _generalErrors.Add(errorMsg);
                    }
                }
            }
            if (_errorCount > 0){
                ClassError = "sdk_error_log_box_sup sdk_error_log_show sdk_error_log_show_detail";
                _detailVisible = true;
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
