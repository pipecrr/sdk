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

namespace Siesa.SDK.Frontend.Components.FormManager.Views
{
    public partial class ErrorsWindow {
        [Inject] public IAuthenticationService AuthenticationService { get; set; }
        [Inject] public UtilsManager UtilsManager { get; set; }
        [Parameter] public EditContext EditFormContext { get; set; }
        [Parameter] public List<string> GeneralErrors { get; set; }
        [Parameter] public List<string> ErrorMsg { get; set; }
        [Parameter] public bool VerifyContext { get; set; }
        [Parameter] public List<string> StackTrace { get; set; } = new List<string>();
        private bool _detailVisible = false;
        private int _errorCount = 0;
        private string ClassError = "sdk_error_log_box_sup";

        private List<SDKErrorsWindowDTO> _errors = new List<SDKErrorsWindowDTO>();

        private Dictionary<string, object[]> _generalerrorsFormat = new Dictionary<string, object[]>();

        private List<string> _generalErrors = new List<string>();

        private string? _environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"); 
        protected override async Task OnParametersSetAsync(){
            _errorCount = 0;
            _generalErrors = new List<string>();
            _errors = new List<SDKErrorsWindowDTO>();

            if (_environment == Environments.Development && StackTrace.Any())
                _generalErrors.AddRange(StackTrace);
            
            
            if (EditFormContext != null && EditFormContext.GetValidationMessages().Count() > 0 && VerifyContext){
                var groupErrors = EditFormContext.GetValidationMessages().GroupBy(x => {
                    var errorsSplit = x.Split("//");
                    if(errorsSplit.Count() > 1){
                        return errorsSplit[1];
                    }else{
                        return "General";
                    }
                });
                foreach (var item in groupErrors){
                    
                    if(!item.Key.Equals("General")){
                        //var field = await UtilsManager.GetResource(item.Key);
                        var field = item.Key;                        
                        Dictionary<string, object[]> errorsFormat = new Dictionary<string, object[]>();
                        var listErrors = item.Select(x => {
                            var errorTag = x.Split("//");
                            var resourceTag = errorTag[0];
                            //var resourceTagValue = UtilsManager.GetResource(resourceTag).Result;
                            //var resourceTagValue = resourceTag;
                            var errorSkip = errorTag.Skip(1);
                            // if(errorSkip[0].Equals(item.Key)){
                            //     errorSkip[0] = field;
                            // }
                            errorsFormat.Add(resourceTag, errorSkip.ToArray());
                            //string errorFormat = errorFormat = string.Format(resourceTagValue, errorSkip);
                            return resourceTag;
                        }).ToList();                        
                        _errors.Add(new SDKErrorsWindowDTO(){
                            Field = field,
                            Errors = errorsFormat
                        });
                    }else{
                        _generalErrors.AddRange(item);
                    }
                }
                _errorCount = _errors.Count();
            }
            if(ErrorMsg?.Count() > 0) {
                _errorCount += ErrorMsg.Count();
                var groupErrors = ErrorMsg.GroupBy(x => x.Split("//")[1]);
                foreach (var item in groupErrors){
                    //var field = UtilsManager.GetResource(item.Key).Result;
                    var field = item.Key;
                    Dictionary<string, object[]> errorsFormat = new Dictionary<string, object[]>();
                    var listErrors = item.Select(x => {
                        var errorTag = x.Split("//");
                        var resourceTag = errorTag[0];
                        //var resourceTagValue = UtilsManager.GetResource(resourceTag).Result;
                        //var resourceTagValue = resourceTag;
                        var errorSkip = errorTag.Skip(1).ToArray();
                        // if(errorSkip[0].Equals(item.Key)){
                        //     errorSkip[0] = field;
                        // }
                        errorsFormat.Add(resourceTag, errorSkip.ToArray());
                        //string errorFormat = errorFormat = string.Format(resourceTagValue, errorSkip);
                        return resourceTag;
                    }).ToList();
                    _errors.Add(new SDKErrorsWindowDTO(){
                        Field = field,
                        Errors = errorsFormat
                    });
                }
            }
            if(GeneralErrors?.Count() > 0) {
                _errorCount += GeneralErrors.Count();
                foreach (var err in GeneralErrors){
                    var errorMsg = "";
                    if(err.StartsWith("Exception: ")){
                         errorMsg = err.Replace("Exception: ", "");
                         if(_generalErrors.Contains(errorMsg)){
                            _generalErrors.Remove(errorMsg);
                            //errorMsg = await UtilsManager.GetResource(errorMsg);
                            errorMsg = errorMsg;
                         }
                         if (errorMsg.Split("//").Count() > 1)
                         {
                                var errorSplit = errorMsg.Split("//");
                                var resourceTag = errorSplit[0];

                                var errorFormat = errorSplit.Skip(1).ToArray();

                                if(!_generalerrorsFormat.ContainsKey(resourceTag))
                                {
                                    _generalerrorsFormat.Add(resourceTag, errorFormat);
                                }
                         }else
                         {
                            _generalErrors.Add(errorMsg);
                         }
                    }else{
                        //errorMsg = await UtilsManager.GetResource(err);
                        errorMsg = err;
                        _generalErrors.Add(errorMsg);
                        _generalErrors = _generalErrors.Distinct().ToList();
                    }
                }
            }
            if (_errorCount > 0){
                ClassError = "sdk_error_log_box_sup sdk_error_log_show sdk_error_log_show_detail";
                _detailVisible = true;
            }else{
                ClassError = "sdk_error_log_box_sup";
            }
            await base.OnParametersSetAsync();
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
