using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using Siesa.SDK.Frontend.Components;
using Siesa.SDK.Frontend.Components.FormManager.Model;
using Siesa.SDK.Frontend.Components.Visualization;

namespace Siesa.SDK.Frontend.Services
{
    public class SDKGuidedTourService
    {
        private UtilsManager UtilsManager;
        public DialogService ds;
        public SDKGuidedTourService(DialogService dialogService,UtilsManager utilsManager)
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

         public async Task<dynamic> ShowConfirmDialog (RenderFragment childContent,string width="400px", string ConfirmationButtonTag="Action.Confirm", string CancelationButtonTag="Action.Cancel",string title="")
         {
             
            return await ds.OpenAsync(title,GetConfirmComponent(childContent,ConfirmationButtonTag,CancelationButtonTag),
            new SDKDialogOption(){ShowTitle=false, Style=$"min-width:300px; max-width:300px; width:{width};"});
         }

         public async Task<dynamic> ShowCustomDialog (RenderFragment<DialogService> childContent,string width="400px", string title="",bool ShowTitle=true , bool showClose=true)
         {

           SDKDialogOption customDialogOption = new SDKDialogOption {
            ShowTitle = ShowTitle,
            Style=$"min-width:600px; width:{width};",
            ShowClose = showClose
           };
           return await ds.OpenAsync(title, childContent,customDialogOption);
         }

    }


}