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
        private DialogService ds;
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

         public async Task<dynamic> ShowConfirmDialog (RenderFragment childContent,string ConfirmationButtonTag="", string CancelationButtonTag="",string title="")
         {
             
            return await ds.OpenAsync(title,GetConfirmComponent(childContent,ConfirmationButtonTag,CancelationButtonTag),
            new SDKDialogOption(){ShowTitle=false, Style="max-width:400px;"});
         }

         public async Task<dynamic> ShowCustomDialog (RenderFragment<DialogService> childContent,string title="",bool ShowTitle=true , bool showClose=true)
         {

           SDKDialogOption customDialogOption = new SDKDialogOption {
            ShowTitle = ShowTitle,
            Style="max-width:400px;",
            ShowClose = showClose
           };
           return await ds.OpenAsync(title, childContent,customDialogOption);
         }

    }


}