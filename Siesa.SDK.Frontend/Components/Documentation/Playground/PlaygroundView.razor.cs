using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Siesa.SDK.Frontend.Components.Documentation.Services;
using Siesa.SDK.Frontend.Services;
using Siesa.SDK.Components.Visualization;
using Siesa.SDK.Frontend.Components.Documentation;
using Siesa.SDK.Frontend.Components.Visualization;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;
using DevExpress.Data.Mask.Internal;
using System.Linq;

namespace Siesa.SDK.Frontend.Components.Documentation.Playground
{
    public partial class PlaygroundView: ComponentBase
    {
        [Inject]
        private IJSRuntime JSRuntime { get; set; }
        [Inject]
        private SDKGlobalLoaderService SDKGlobalLoaderService { get; set; }
        [Inject]
        private CompilerService Compiler { get; set; }
        [Inject]
        private SDKDialogService SDKDialogService { get; set; }
        [Inject]
        private SDKNotificationService SDKNotificationService { get; set; }
        private bool _isLoaded;
        private Type _compiledType;
        private string _errorMessage;
        private ErrorBoundary? errorBoundary;
        private bool _compiling;
        private bool RunEnabled => !_compiling;
        private string RunTag => _compiling ? "" : "Run";
        private string RunIcon => _compiling ? "fa-spinner fa-spin" : "fa-play";
        private Radzen.Orientation _currentLayout = Radzen.Orientation.Horizontal;
        private string ChangeLayoutIcon => _currentLayout == Radzen.Orientation.Horizontal ? "fa-columns-3" : "fa-table-columns fa-rotate-270";
        private List<Entry> Entries { get; set; } = new();
        private List<Entry> OpenedEntries { get; set; } = new();
        private SDKTabs _TabsRef;
        private bool _showFileTopbarButtons;
        private FileTree _fileTreeRef;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Entries.Add(new Entry
            {
                Name = "Dummy.razor",
                Code = @"<InlineCrud BusinessName=""BLUser"" />"
            });
            Entries.Add(new Entry
            {
                Name = "Dummy.cs",
                Code = @"Console.WriteLine(""Hello World"");"
            });

            OpenedEntries.Add(Entries[0]);
            //set the selected entry to the first one after 500ms

            Task.Run(async () =>
            {
                await Task.Delay(500).ConfigureAwait(true);
                _fileTreeRef.Select(Entries[0]);
                StateHasChanged();
            });
        }

        private void MouseOverFileTopbar(MouseEventArgs e)
        {
            _showFileTopbarButtons = true;
        }

        private void MouseOutFileTopbar(MouseEventArgs e)
        {
            _showFileTopbarButtons = false;
        }

        private string GetActiveTabCss(Entry entry)
        {
            if (entry == null) return "";
            if (OpenedEntries.Contains(entry))
            {
                var index = OpenedEntries.IndexOf(entry);
                return index == _TabsRef.ActiveTabIndex ? "active" : "";
            }
            return "";
        }
        
        private void AddItem(Entry parent, bool isDirectory)
        {
            List<Entry> _parent;
            if (parent == null)
            {
                _parent = Entries;
            }
            else
            {
                _parent = parent.Children;
            }
            _parent.Add(new Entry
            {
                Name = "New Item",
                IsDirectory = isDirectory,
                Children = new List<Entry>(),
                Parent = parent
            });
            StateHasChanged();
        }

        private void AddFolder()
        {
            AddItem(null, true);
        }
        
        private void AddFile()
        {
            AddItem(null, false);
        }
        
        public void OnSelectItem(Entry entry)
        {
            if(entry.IsDirectory) return;
            if (!OpenedEntries.Contains(entry) && !OpenedEntries.Where(x => x.GetPath() == entry.GetPath()).Any())
            {
                OpenedEntries.Add(entry);
            }
            if(_TabsRef != null)
            {
                _TabsRef.ActiveTabIndex = OpenedEntries.IndexOf(entry);
            }
            StateHasChanged();
        }

        public void CloseEntry(Entry entry)
        {
            if (OpenedEntries.Contains(entry))
            {
                var index = OpenedEntries.IndexOf(entry);
                OpenedEntries.Remove(entry);
                if(_fileTreeRef != null)
                {
                    if(_fileTreeRef.GetSelectedItem() == entry)
                    {
                        if(index == 0 && OpenedEntries.Count == 0)
                        {
                            _fileTreeRef.Select(null);
                        }
                        else if(index == 0 && OpenedEntries.Count > 0)
                        {
                            _fileTreeRef.Select(OpenedEntries[0]);
                        }
                        else if(index > 0 && OpenedEntries.Count > 0)
                        {
                            _fileTreeRef.Select(OpenedEntries[index - 1]);
                        }
                    }
                }
            }
        }

        private void ChangeLayout()
        {
            _currentLayout = _currentLayout == Radzen.Orientation.Horizontal ? Radzen.Orientation.Vertical : Radzen.Orientation.Horizontal;
        }

        private string ResultIcon
        {
            //checkf if there is an error or if the code is compiling
            get
            {
                if (_errorMessage != null)
                {
                    return "fa-times-circle";
                }
                if (_compiling)
                {
                    return "fa-spinner fa-spin";
                }
                return "fa-check-circle";
            }
        }
        
        private async Task RunCode()
        {
            if(_fileTreeRef == null) return;
            Entry selectedItem = _fileTreeRef.GetSelectedItem() as Entry;
            var code = selectedItem.Code;
            try
            {
                _compiling = true;
                StateHasChanged();
                _compiledType = await Compiler.CompileAsync(code);
                _errorMessage = null;
            }
            catch (ApplicationException e)
            {
                _errorMessage = e.Message;
                _compiledType = null;
            }
            finally
            {
                _compiling = false;
            }
            StateHasChanged();
        }
        private async Task LoadSingleCode(string code, string fileName = null)
        {
            Entries.Clear();
            Entries.Add(new Entry
            {
                Name = fileName ?? "Dummy.razor",
                Code = code
            });
            OpenedEntries.Clear();
            InvokeAsync(StateHasChanged);
            await Task.Delay(100).ConfigureAwait(true);
            OpenedEntries.Add(Entries[0]);
            _fileTreeRef.Select(Entries[0]);
            InvokeAsync(StateHasChanged);
            _ = RunCode();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if(firstRender){
                try
                {
                    await ImportJs();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
        
        private async Task<bool> IsMonacoLoaded()
        {
            bool monaco;
            try
            {
                monaco = await JSRuntime.InvokeAsync<bool>("globalVariableExists", "monaco");
            }
            catch (Exception e)
            {
                monaco = false;
            }
            return monaco != false;
        }

        private async Task ImportJs()
        {
            SDKGlobalLoaderService.Show();
            await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/Siesa.SDK.Frontend/js/utils.js");
            await JSRuntime.InvokeVoidAsync("window.loadScript", "_content/BlazorMonaco/jsInterop.js");
            await JSRuntime.InvokeVoidAsync("window.loadScript", "_content/BlazorMonaco/lib/monaco-editor/min/vs/loader.js");
            await JSRuntime.InvokeVoidAsync("window.loadScript", "_content/BlazorMonaco/lib/monaco-editor/min/vs/editor/editor.main.js");
            //wait for monaco to be ready
            while (!await IsMonacoLoaded())
            {
                await Task.Delay(500);
            }
            SDKGlobalLoaderService.Hide();
            _isLoaded = true;
            StateHasChanged();
            
        }
    }
}