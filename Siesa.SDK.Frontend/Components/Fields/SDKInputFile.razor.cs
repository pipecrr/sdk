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
using Siesa.SDK.Frontend.Components.Layout;
using Siesa.Global.Enums;
using Siesa.SDK.Frontend.Services;
using Microsoft.Extensions.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Collections;



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

    [Inject]
    private IFeaturePermissionService FeaturePermissionService { get; set; }

    [Inject]
    private SDKNotificationService Notification { get; set; }

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
    private string _display = "none"; 

    private readonly List<string> _ExtensionsImage = new() { "jpg", "jpeg", "png", "gif", "bmp", "svg", "webp", "tif ", "tiff"  };

    protected override async Task OnInitializedAsync()
    {
       await base.OnInitializedAsync().ConfigureAwait(true);
    }

    protected override async Task OnParametersSetAsync()
    {
        if (_cancellationToken != null)
        {
            _cancellationToken.Cancel();
        }
        _cancellationToken = new CancellationTokenSource();
        await Task.Delay(500, _cancellationToken.Token).ConfigureAwait(true);
        _IsLoading = true;
        if (IsMultiple && _FilesSelected.Count == 0)
        {
            var response = await BackendRouterService.GetSDKBusinessModel("BLAttachmentDetail", AuthenticationService).Call("GetAttachmentsDetail", RowidAttachmentRelationship, SaveBytes).ConfigureAwait(true);
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
            var response = await BackendRouterService.GetSDKBusinessModel("BLAttachmentDetail", AuthenticationService).Call("GetAttachmentDetail", RowidAttachmentDetail).ConfigureAwait(true);
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
            _display = "block";
        }
        _IsLoading = false;
        StateHasChanged();

        await base.OnParametersSetAsync().ConfigureAwait(true);
    }

    private string GetIconExtension(string extension)
    {
        if (!string.IsNullOrEmpty(extension))
        {
            extension = extension.Split("/")[1];

            switch (extension)
            {
                case "pdf":
                    return "fa-file-pdf";
                case "xls":
                case "xlsx":
                case "csv":
                    return "fa-file-excel";
                case "doc":
                case "docx":
                    return "fa-file-word";
                case "ppt":
                case "pptx":
                    return "fa-file-powerpoint";
                case "zip":
                case "rar":
                    return "fa-file-zipper";
                case "mp3":    
                case "wav":
                case "ogg":
                    return "fa-file-audio";
                case "mp4":
                case "avi":
                case "mov":
                case "wmv":
                    return "fa-file-video";
                default:
                    return "fa-file-lines";
            }
        }
        return "fa-file-lines";
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
        await JSRuntime.InvokeVoidAsync("closePreviewImage", previewImageElem).ConfigureAwait(true);
        InputFile = null;
        _display = "none";
        StateHasChanged();
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
            _display = "block";
            if (IsMultiple)
            {
                var files = InputFile.GetMultipleFiles();
                foreach (var itemFile in files)
                {
                    if (itemFile.Size > MaxSize)
                    {
                        _ = Notification.ShowError("Custom.SDKInputFile.FileMaxSize", new object[] { itemFile.Name });
                        InputFile = null;
                        break;
                    }
                    var file = await ConvertToIFormFile(itemFile).ConfigureAwait(true);
                    
                    var urlImage = await GetFileUrl(file).ConfigureAwait(true);

                    SDKInputFieldDTO inputFieldDTO = new()
                    {
                        Url = urlImage,
                        File = file,
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
                if (InputFile.File.Size > MaxSize)
                {
                    _ = Notification.ShowError("Custom.SDKInputFile.FileMaxSize", new object[] { InputFile.File.Name });
                    InputFile = null;
                    _display = "none";
                    return ;
                }
                SingleFileSize = InputFile.File.Size / 1024;

                var singleFile = await ConvertToIFormFile(_InputFile.File).ConfigureAwait(true);
                var urlImage = await GetFileUrl(singleFile).ConfigureAwait(true);

                _UrlImage = urlImage;
            }

            OnInputFile?.Invoke(_InputFile);
        }
    }

    private static async Task<string> GetFileUrl(FormFile file)
    {
        using (var ms = new MemoryStream())
        {
            var _stream = file.OpenReadStream();
            ms.Seek(0, SeekOrigin.Begin);
            await _stream.CopyToAsync(ms).ConfigureAwait(true);         
            ms.Seek(0, SeekOrigin.Begin);
            var base64 = Convert.ToBase64String(ms.ToArray());
            var url = $"data:{file.ContentType};base64,{base64}";
            return url;
        }
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
            InputFile = null;
        }
        StateHasChanged();
    }

    public async Task SDKUploadFile(bool ignorePermissions = false)
    {
        try
        {
            await IntanceBusinessObj().ConfigureAwait(true);
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
                    var response = await BackendRouterService.GetSDKBusinessModel("BLAttachmentDetail", AuthenticationService).Call("DeleteMultiAttachmentDetail", _FilesDeleted).ConfigureAwait(true);
                    if (!response.Success)
                    {
                        throw new Exception(response.Errors.First());
                    }
                }
                foreach (var itemFile in _FilesToSave)
                {
                    await SaveAttachment(itemFile.File,ignorePermissions, itemFile).ConfigureAwait(true);
                    _FilesToSave = new();
                }
            }
            else
            {
                await SaveAttachment(InputFile.File, ignorePermissions).ConfigureAwait(true);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private async Task SaveAttachment(IBrowserFile itemFile,bool ignorePermissions, SDKInputFieldDTO item = null)
    {        
        var formFile = await ConvertToIFormFile(itemFile).ConfigureAwait(true);
        var fileUploadDTO = new SDKFileUploadDTO();

        if (SaveBytes)
        {
            fileUploadDTO = await BusinessObj.UploadSingleByte(formFile, ignorePermissions);
        }
        else
        {
            fileUploadDTO = await BusinessObj.UploadSingle(formFile, ignorePermissions);
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

    private async Task<FormFile> ConvertToIFormFile(IBrowserFile browserFile)
    {
        var ms = new MemoryStream();
        if (browserFile.Size > MaxSize)
        {
            _ = Notification.ShowError("Custom.SDKInputFile.FileMaxSize", new object[] { browserFile.Name });
            return null;
        }
        
        await browserFile.OpenReadStream(maxAllowedSize: MaxSize).CopyToAsync(ms).ConfigureAwait(true);
        
        ms.Seek(0, SeekOrigin.Begin);

        var file = new FormFile(ms, 0, ms.Length, null, browserFile.Name)
        {
            //Headers = new HeaderDictionary(),
            ContentType = browserFile.ContentType ?? "application/octet-stream"
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

public class FormFile : IFormFile
{
    public FormFile(Stream baseStream, long baseStreamOffset, long length, string name, string fileName)
    {
        _stream = baseStream ?? throw new ArgumentNullException(nameof(baseStream));
        Length = length;
        Name = name;
        FileName = fileName;
    }

    private readonly Stream _stream;
    public string ContentType {get; set;}

    public string ContentDisposition {get; set;}

    public IHeaderDictionary Headers {get; set;}

    public long Length {get; set;}

    public string Name {get; set;}

    public string FileName {get; set;}

    public Stream OpenReadStream()
    {
        return _stream;
    }

    public void CopyTo(Stream target)
    {
        _stream.CopyTo(target);   
    }
    public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
    {
        return _stream.CopyToAsync(target, cancellationToken);
    }
}
