using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Siesa.SDK.Frontend.Components;
using Siesa.SDK.Frontend.Components.Visualization;
using Siesa.SDK.Shared.Application;
using Microsoft.Extensions.DependencyInjection;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Shared.Utilities;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading;
using Siesa.SDK.Shared.DTOS;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using DevExpress.Data.Mask.Internal;



namespace Siesa.SDK.Frontend.Components.Fields;

/// <summary>
/// Partial class that inherits from SDKComponent and represents an input file.
/// </summary>
public partial class SDKInputFile : SDKComponent
{
    /// <summary>
    /// Delegate for the event that is triggered when the input file changes.
    /// </summary>
    [Parameter]
    public Action<InputFileChangeEventArgs> OnInputFile { get; set; }

    /// <summary>
    /// Indicates whether selecting multiple files is allowed.
    /// </summary>
    [Parameter]
    public bool IsMultiple { get; set; }

    /// <summary>
    /// File filter type, default is "image/*".
    /// </summary>
    [Parameter]
    public string FilterType { get; set; } = "image/*";

    /// <summary>
    /// CSS class for applying custom styles.
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    /// <summary>
    /// Business name associated with the file.
    /// </summary>
    [Parameter]
    public string BusinessName { get; set; }

    /// <summary>
    /// Row identifier for attachment details.
    /// </summary>
    [Parameter]
    public int RowidAttachmentDetail { get; set; }

    /// <summary>
    /// Row identifier for attachment relationships.
    /// </summary>
    [Parameter]
    public int RowidAttachmentRelationship { get; set; }

    /// <summary>
    /// Indicates whether to save the file bytes.
    /// </summary>
    [Parameter]
    public bool SaveBytes { get; set; } = true;

    /// <summary>
    /// Maximum allowed size for the file, default is 3000000 bytes (3MB).
    /// </summary>
    [Parameter]
    public int MaxSize { get; set; } = 3000000;

    /// <summary>
    /// Indicates whether the file is a detail.
    /// </summary>
    [Parameter]
    public bool IsDetail { get; set; }

    /// <summary>
    /// Indicates whether to show a preview of the file.
    /// </summary>
    [Parameter]
    public bool ShowPreview { get; set; } = true;

    /// <summary>
    /// Indicates whether the file is required.
    /// </summary>
    [Parameter]
    public bool IsRequired { get; set; } = false;

    /// <summary>
    /// Indicates whether to show image information.
    /// </summary>
    [Parameter]
    public bool ShowInfoImg { get; set; }

    /// <summary>
    /// Indicates whether to show the preview in full view.
    /// </summary>
    [Parameter]
    public bool FullViewPreview { get; set; }

    /// <summary>
    /// Reference to the associated InputFile instance.
    /// </summary>
    public InputFile? _refinputFile;

    [Inject]
    private IJSRuntime JSRuntime { get; set; }

    [Inject]
    private IBackendRouterService BackendRouterService { get; set; }

    [Inject]
    private IServiceProvider ServiceProvider { get; set; }

    private ElementReference previewImageElem;

    private InputFileChangeEventArgs InputFile;

    private List<SDKInputFieldDTO> _FilesSelected = new();
    private List<SDKInputFieldDTO> _FilesToSave = new();

    private SDKInputFieldDTO _FileSelected = new();
    private List<int> _FilesDeleted = new();
    private string _UrlImage = "";
    private string _breakpoints => FullViewPreview ? "col-12" : "col-sm-6 col-md-12 col-lg-6";
    private dynamic BusinessObj;
    private bool _IsLoading;

    private long? SingleFileSize;

    private CancellationTokenSource _cancellationToken;

    protected override async Task OnInitializedAsync()
    {
        base.OnInitialized();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (_cancellationToken != null)
        {
            _cancellationToken.Cancel();
        }
        _cancellationToken = new CancellationTokenSource();
        await Task.Delay(500, _cancellationToken.Token);
        _IsLoading = true;
        if (IsMultiple && _FilesSelected.Count == 0)
        {
            var response = await BackendRouterService.GetSDKBusinessModel("BLAttachmentDetail", AuthenticationService).Call("GetAttachmentsDetail", RowidAttachmentRelationship, SaveBytes);
            if (response.Success && response.Data != null && response.Data.Count > 0)
            {
                var attachments = response.Data;
                foreach (var item in attachments)
                {
                    SDKInputFieldDTO inputFieldDTO = new()
                    {
                        Url = item.Url,
                        RowidAttachmentDetail = item.Rowid
                    };
                    _FilesSelected.Add(inputFieldDTO);
                }
                _UrlImage = _FilesSelected[0].Url;
            }
        }
        else if (RowidAttachmentDetail > 0)
        {
            var response = await BackendRouterService.GetSDKBusinessModel("BLAttachmentDetail", AuthenticationService).Call("GetAttachmentDetail", RowidAttachmentDetail);
            if (response.Success && response.Data != null)
            {
                var attachment = response.Data;
                SDKInputFieldDTO inputFieldDTO = new()
                {
                    Url = attachment.Url,
                    RowidAttachmentDetail = attachment.Rowid
                };
                _UrlImage = inputFieldDTO.Url;
            }
        }
        _IsLoading = false;
        StateHasChanged();
        base.OnParametersSetAsync();
    }

    private async Task GetPreviewFile()
    {
        if (_refinputFile != null)
        {
            await JSRuntime.InvokeVoidAsync("previewImage", _refinputFile!.Element, previewImageElem).ConfigureAwait(true);
        }
        StateHasChanged();
    }
    private async Task ClosePreviewFile()
    {
        if (!string.IsNullOrEmpty(previewImageElem.Id))
        {
            await JSRuntime.InvokeVoidAsync("closePreviewImage", previewImageElem).ConfigureAwait(true);
            InputFile = null;
            StateHasChanged();
        }
    }
    private void ClickIcon()
    {
        JSRuntime.InvokeVoidAsync("clickInputFile", _refinputFile.Element);
    }

    private void ClickImg(SDKInputFieldDTO sdkInputFieldDTO)
    {
        var url = sdkInputFieldDTO.Url;
        _UrlImage = url;
        StateHasChanged();
    }

    private async Task _OnChange(InputFileChangeEventArgs _InputFile)
    {
        if (_InputFile != null)
        {
            InputFile = _InputFile;
            if (IsMultiple)
            {
                var files = InputFile.GetMultipleFiles();
                foreach (var itemFile in files)
                {
                    var file = await ConvertToIFormFile(itemFile);
                    var urlImage = await GetFileUrl(file);

                    SDKInputFieldDTO inputFieldDTO = new()
                    {
                        Url = urlImage,
                        File = itemFile,
                        FileName = itemFile.Name,
                        FileSize = itemFile.Size / 1024
                    };
                    _FilesSelected.Add(inputFieldDTO);
                    _FilesToSave.Add(inputFieldDTO);
                }
                _UrlImage = _FilesSelected[0].Url;
            }
            else
            {
                SingleFileSize = InputFile.File.Size / 1024;

                await GetPreviewFile();
            }
            OnInputFile?.Invoke(_InputFile);
        }
    }

    private async Task<string> GetFileUrl(IFormFile file)
    {
        var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        var base64 = Convert.ToBase64String(ms.ToArray());
        var url = $"data:{file.ContentType};base64,{base64}";

        return url;
    }

    private void CloseItem(SDKInputFieldDTO sdkInputFieldDTO)
    {
        var url = sdkInputFieldDTO.Url;
        _FilesSelected.RemoveAll(x => x.Url == url);
        _FilesToSave.RemoveAll(x => x.Url == url);
        if (sdkInputFieldDTO.RowidAttachmentDetail > 0)
        {
            _FilesDeleted.Add(sdkInputFieldDTO.RowidAttachmentDetail);
        }
        if (_FilesSelected.Count > 0)
        {
            _UrlImage = _FilesSelected[0].Url;
        }
        else
        {
            _UrlImage = "";
        }
        StateHasChanged();
    }

    public async Task SDKUploadFile()
    {
        try
        {
            await IntanceBusinessObj();
            if (BusinessObj == null)
            {
                return;
            }

            if (IsMultiple)
            {
                if (RowidAttachmentRelationship == 0)
                {
                    throw new Exception("Debe especificar el RowidAttachmentRelationship");
                }
                if (_FilesDeleted.Count > 0)
                {
                    var response = await BackendRouterService.GetSDKBusinessModel("BLAttachmentDetail", AuthenticationService).Call("DeleteMultiAttachmentDetail", _FilesDeleted);
                    if (!response.Success)
                    {
                        throw new Exception(response.Errors.First());
                    }
                }
                foreach (var itemFile in _FilesToSave)
                {
                    await SaveAttachment(itemFile.File, itemFile);
                    _FilesToSave = new();
                }
            }
            else
            {
                await SaveAttachment(InputFile.File);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Prueba " + ex.Message);
        }
    }

    private async Task SaveAttachment(IBrowserFile itemFile, SDKInputFieldDTO item = null)
    {
        var formFile = await ConvertToIFormFile(itemFile);
        var fileUploadDTO = new SDKFileUploadDTO();

        if (SaveBytes)
        {
            fileUploadDTO = await BusinessObj.UploadSingleByte(formFile);
        }
        else
        {
            fileUploadDTO = await BusinessObj.UploadSingle(formFile);
        }

        if (fileUploadDTO != null)
        {
            fileUploadDTO.RowidAttachment = RowidAttachmentRelationship;
            dynamic saveFile;

            if (RowidAttachmentDetail > 0)
            {
                saveFile = await BusinessObj.SaveAttachmentDetail(fileUploadDTO, RowidAttachmentDetail);
            }
            else
            {
                saveFile = await BusinessObj.SaveAttachmentDetail(fileUploadDTO);
                if (!IsMultiple)
                {
                    RowidAttachmentDetail = saveFile;
                }
                else
                {
                    item.RowidAttachmentDetail = saveFile;
                }
            }
        }
    }

    private async Task<IFormFile> ConvertToIFormFile(IBrowserFile browserFile)
    {
        var ms = new MemoryStream();
        if (browserFile.Size > MaxSize)
        {
            throw new Exception("El archivo es demasiado grande");
        }

        await browserFile.OpenReadStream(maxAllowedSize: MaxSize).CopyToAsync(ms);

        var file = new FormFile(ms, 0, ms.Length, null, browserFile.Name)
        {
            Headers = new HeaderDictionary(),
            ContentType = (browserFile.ContentType == "" ? "application/octet-stream" : browserFile.ContentType)
        };

        return file;
    }


    private async Task IntanceBusinessObj()
    {
        if (ServiceProvider != null)
        {
            var businessModel = BackendRouterService.GetSDKBusinessModel(BusinessName, null);
            if (businessModel != null)
            {
                BusinessObj = ActivatorUtilities.CreateInstance(ServiceProvider,
                Utilities.SearchType($"{businessModel.Namespace}.{businessModel.Name}", true));
            }
        }
    }
}


