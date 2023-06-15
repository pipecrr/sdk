using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using Siesa.SDK.Frontend.Components;
using Siesa.SDK.Frontend.Components.FormManager.Model;
using Siesa.SDK.Frontend.Components.Visualization;

namespace Siesa.SDK.Frontend.Services
{
    public class SDKDialogService
    {
        private UtilsManager UtilsManager;
        public DialogService ds;
        public SDKDialogService(DialogService dialogService,UtilsManager utilsManager)
        {
             UtilsManager = utilsManager;
             ds = dialogService;
        }

        private RenderFragment<DialogService> GetConfirmComponent(RenderFragment childContent, string ConfirmationButtonTag, string CancelationButtonTag)
        {
            return (ds) => builder =>
            {
                builder.OpenComponent<SDKConfirmDialog>(0);
                builder.AddAttribute(1, "ChildContent", childContent);
                builder.AddAttribute(2, "DialogService", ds);
                builder.AddAttribute(3, "ConfirmationButtonTag", ConfirmationButtonTag);
                builder.AddAttribute(4, "CancelationButtonTag", CancelationButtonTag);
                builder.CloseComponent();
            };
        }

         public async Task<dynamic> ShowConfirmDialog (RenderFragment childContent,string width="400px", string ConfirmationButtonTag="Action.Confirm", string CancelationButtonTag="Action.Cancel",string title="", string ResourceTag="", SDKModalWidth standardWidth=SDKModalWidth.Undefined)
         {
            var _title = title;
            if(!string.IsNullOrEmpty(ResourceTag)){
                _title = await UtilsManager.GetResource(ResourceTag);
            }
            string _width = width;
            if(standardWidth != SDKModalWidth.Undefined){
                if(standardWidth == SDKModalWidth.Small){
                    _width = "300px";
                }else if(standardWidth == SDKModalWidth.Medium){
                    _width = "600px";
                }else{
                    _width = "90%";
                }
            }
            return await ds.OpenAsync(_title,GetConfirmComponent(childContent,ConfirmationButtonTag,CancelationButtonTag),
            new SDKDialogOption(){ShowTitle=false, Style=$"min-width:300px; width:{_width};", CssClass="whcm_modal_relacion"});
         }

         public async Task<dynamic> ShowCustomDialog (RenderFragment<DialogService> childContent,string width="600px", string title="",bool ShowTitle=true , bool showClose=true, string height="", string ResourceTag="", SDKModalWidth standardWidth=SDKModalWidth.Undefined, string CssClass="")
         {
            if(!string.IsNullOrEmpty(ResourceTag)){
                title = await UtilsManager.GetResource(ResourceTag);
            }
            string TitleTag = await UtilsManager.GetResource(title);
            string _width = width;
            if(standardWidth != SDKModalWidth.Undefined){
                if(standardWidth == SDKModalWidth.Small){
                    _width = "300px";
                }else if(standardWidth == SDKModalWidth.Medium){
                    _width = "600px";
                }else{
                    _width = "90%";
                }
            }
            string style = $"min-width:300px; width:{_width}";
            if(!string.IsNullOrEmpty(height)){
                style += $"; height:{height}";
            }
            SDKDialogOption customDialogOption = new SDKDialogOption {
                ShowTitle = ShowTitle,
                Style=style,
                ShowClose = showClose,
                Resizable = true,
                CssClass=$"whcm_modal_relacion {CssClass}"
            };
           return await ds.OpenAsync(TitleTag, childContent,customDialogOption);
         }

         public virtual void Close(dynamic result = null){
            ds.Close(result);
         }

    }


}