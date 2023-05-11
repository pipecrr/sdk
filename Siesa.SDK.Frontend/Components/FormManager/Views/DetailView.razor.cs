using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Siesa.SDK.Frontend.Components.FormManager.Model;
using Microsoft.JSInterop;
using Siesa.SDK.Business;
using Siesa.SDK.Frontend.Components.FormManager.Model.Fields;
using Siesa.SDK.Frontend.Utils;
using Radzen;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Frontend.Services;
using Siesa.SDK.Entities;
using Siesa.Global.Enums;
using Siesa.SDK.Frontend.Components.FormManager.Fields;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;

namespace Siesa.SDK.Frontend.Components.FormManager.Views
{
    public partial class DetailView : ComponentBase
    {
        [Inject] public Radzen.DialogService dialogService { get; set; }
        [Parameter]
        public string BusinessName { get; set; }
        [Parameter]
        public dynamic BusinessObj { get; set; }

        [Parameter]
        public bool SetTopBar { get; set; } = true;

        [Parameter]
        public bool IsSubpanel { get; set; }

        [Parameter]
        public bool ShowTitle { get; set; } = true;
        [Parameter]
        public bool ShowButtons { get; set; } = true;
        [Parameter]
        public bool ShowCancelButton { get; set; } = true;

        [Parameter]
        public bool ShowDeleteButton { get; set; } = true;

        [Parameter]
        public string ViewdefName { get; set; }

        public Boolean Loading = true;

        [Inject] public IJSRuntime JSRuntime { get; set; }
        [Inject] public NavigationManager NavManager { get; set; }

        [Inject] public NavigationService NavigationService { get; set; }
        [Inject]
        public IBackendRouterService BackendRouterService { get; set; }

        [Inject] public IFeaturePermissionService FeaturePermissionService { get; set; }
        [Inject] public IAuthenticationService AuthenticationService { get; set; }

        [Inject] public SDKNotificationService NotificationService { get; set; }        
        protected FormViewModel FormViewModel { get; set; } = new FormViewModel();
        protected List<Panel> Panels { get { return FormViewModel.Panels; } }
        public List<Panel> AuxPanels { get; set; } = new List<Panel>();
        public Boolean ModelLoaded = false;
        public String ErrorMsg = "";
        public List<string> ErrorList = new List<string>();
        protected bool CanCreate;
        protected bool CanEdit;
        protected bool CanDelete;
        protected bool CanDetail;
        protected bool CanList;
        public Boolean ContainAttachments = false;
        Relationship RelationshipAttachment = new Relationship();
        E00270_Attachment ParentAttachment = new E00270_Attachment();

        private bool HasExtraButtons { get; set; }
        private List<Button> ExtraButtons { get; set; }
        private Button CreateButton { get; set; }
        private Button DuplicateButton { get; set; }
        private Button EditButton { get; set; }
        private Button ListButton { get; set; }
        private Button DeleteButton { get; set; }
        private string _viewdefName;
        private void setViewContext(List<Panel> panels)
        {
            for (int i = 0; i < panels.Count; i++)
            {
                if (String.IsNullOrEmpty(panels[i].ResourceTag))
                {
                    if (String.IsNullOrEmpty(panels[i].ResourceTag))
                    {
                        panels[i].ResourceTag = $"{BusinessName}.Viewdef.detail.Panel.{panels[i].Name}";
                    }
                }

                for (int j = 0; j < panels[i].Fields.Count; j++)
                {
                    panels[i].Fields[j].ViewContext = "DetailView";
                    panels[i].Fields[j].GetFieldObj(BusinessObj);
                }
                if (panels[i].SubViewdef != null && panels[i].SubViewdef.Panels.Count > 0)
                {
                    setViewContext(panels[i].SubViewdef.Panels);
                }
            }

        }

        protected async Task InitView(string bName = null)
        {
            Loading = true;
            if (bName == null)
            {
                bName = BusinessName;
            }
            await CheckPermissions();
            await CreateRelationshipAttachment();
            if(String.IsNullOrEmpty(ViewdefName)){
                _viewdefName = "detail";
            }else{
                _viewdefName = ViewdefName;
            }            
            var metadata = BackendRouterService.GetViewdef(bName, _viewdefName);
            if (String.IsNullOrEmpty(metadata) && _viewdefName.Equals("related_detail"))
            {
                metadata = BackendRouterService.GetViewdef(bName, "detail");
            }
            if(String.IsNullOrEmpty(metadata))
            {
                metadata = BackendRouterService.GetViewdef(bName, "default");
            }
            if (String.IsNullOrEmpty(metadata))
            {
                //ErrorMsg = "No hay definición para la vista de detalle";
                ErrorMsg = "Custom.Generic.ViewdefNotFound";
                ErrorList.Add(ErrorMsg);
            }
            else
            {
                try
                {
                    FormViewModel = JsonConvert.DeserializeObject<FormViewModel>(metadata);
                }
                catch (System.Exception)
                {
                    //Soporte a viewdefs anteriores
                    var panels = JsonConvert.DeserializeObject<List<Panel>>(metadata);
                    FormViewModel.Panels = panels;
                }
                //AddPanel(FormViewModel.Panels, BusinessObj);
                setViewContext(Panels);
                if (FormViewModel.Relationships != null && FormViewModel.Relationships.Count > 0)
                {
                    foreach (var relationship in FormViewModel.Relationships)
                    {
                        if (String.IsNullOrEmpty(relationship.ResourceTag))
                        {
                            relationship.ResourceTag = $"{BusinessName}.Relationship.{relationship.Name}";
                        }
                        var canListRel = await FeaturePermissionService.CheckUserActionPermission(relationship.RelatedBusiness, enumSDKActions.Consult, AuthenticationService);
                        relationship.Enabled = canListRel;
                    }
                }
                ModelLoaded = true;
            }
            await EvaluateButtonAttributes();
           
            Loading = false;
            StateHasChanged();
        }

        // private async Task AddPanel(dynamic data, SDKBusinessModel bl)
        // {
        //     if(AuxPanels == null){
        //         AuxPanels = new List<Panel>();
        //     }            
        //     foreach(var item in data){
        //         var panel = new Panel();
        //         panel.Name = item.Id;
        //         panel.ResourceTag = item.Tag;
        //         var requestColumns = await bl.Call("GetColumnsDynamicEntity", item.Rowid);
        //         if(requestColumns.Success && requestColumns.Data != null){
        //             List<FieldOptions> fields = new List<FieldOptions>();
        //             foreach(var column in requestColumns.Data){
        //                 var field = new FieldOptions();
        //                 field.Name = column.Id;
        //                 field.FieldType = GetTypesField(column.DataType);
        //                 field.ResourceTag = column.Tag;
        //                 field.ViewContext = "DetailView";
        //                 fields.Add(field);
        //             }
        //             panel.Fields = fields;
        //         }
        //         Type baseObjType = CreateDynamicObject(BusinessObj.BaseObj.GetType().Name, panel.Fields);
        //         var DynamicBaseObj = Activator.CreateInstance(baseObjType);

        //         //panel.DynamicBusinessObj = DynamicBaseObj;

        //         AuxPanels.Add(panel);
        //     }
        // }

        private Type CreateDynamicObject(string id, List<FieldOptions> fields)
        {
            // Crea una nueva assembly dinámica.
            AssemblyName assemblyName = new AssemblyName("DynamicEntityAssembly");
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);

            // Crea un nuevo módulo dinámico en la assembly.
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicEntityModule");

            // Crea un nuevo tipo con propiedades nulas.
            TypeBuilder typeBuilder = moduleBuilder.DefineType(id, TypeAttributes.Public | TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed, typeof(object));
            foreach(var field in fields){

                Type type = GetTypeTypesField(field.FieldType);
                typeBuilder.DefineField(field.Name, type, FieldAttributes.Public);
                
                // Crea un campo privado para cada propiedad del tipo original.
                FieldBuilder fieldBuilder = typeBuilder.DefineField($"_{field.Name}", type, FieldAttributes.Private);

                // Crea una propiedad pública 
                PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(field.Name, PropertyAttributes.None, type, new Type[] { type });

                // Define el método get.
                MethodBuilder getMethodBuilder = typeBuilder.DefineMethod("get_" + field.Name, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, type, Type.EmptyTypes);

                // Crea el cuerpo del método.
                ILGenerator getIL = getMethodBuilder.GetILGenerator();
                getIL.Emit(OpCodes.Ldarg_0);
                getIL.Emit(OpCodes.Ldfld, fieldBuilder);
                getIL.Emit(OpCodes.Ret);
                propertyBuilder.SetGetMethod(getMethodBuilder);

                // Define el método set.
                MethodBuilder setMethodBuilder = typeBuilder.DefineMethod("set_" + fieldBuilder, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null, new Type[] { type });

                // Crea el cuerpo del método.
                ILGenerator setIL = setMethodBuilder.GetILGenerator();
                setIL.Emit(OpCodes.Ldarg_0);
                setIL.Emit(OpCodes.Ldarg_1);
                setIL.Emit(OpCodes.Stfld, fieldBuilder);
                setIL.Emit(OpCodes.Ret);
                propertyBuilder.SetSetMethod(setMethodBuilder);
            }

            // Crea el constructor.
            ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
            ILGenerator constructorIL = constructorBuilder.GetILGenerator();
            constructorIL.Emit(OpCodes.Ldarg_0);
            constructorIL.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));
            constructorIL.Emit(OpCodes.Ret);

            Type generetedType = typeBuilder.CreateType();

            return generetedType;
        }

        private Type GetTypeTypesField(FieldTypes fieldType)
        {
            switch(fieldType){
                case FieldTypes.CharField:
                    return typeof(string);
                case FieldTypes.DecimalField:
                    return typeof(decimal);
                case FieldTypes.DateTimeField:
                    return typeof(DateTime);
                default:
                    return typeof(string);
            }
        }

        private FieldTypes GetTypesField(dynamic dataType)
        {
            switch(dataType){
                case 0:
                    return FieldTypes.CharField;
                case 1:
                    return FieldTypes.DecimalField;
                case 2:
                    return FieldTypes.DateTimeField;
                default:
                    return FieldTypes.CharField;
            }
        }

        private async Task EvaluateButtonAttributes()
        {
            if(FormViewModel.Buttons != null){
                ExtraButtons = new List<Button>();
                foreach (var button in FormViewModel.Buttons){
                    if(button.CustomAttributes != null && button.CustomAttributes.ContainsKey("sdk-disabled")){
                        var disabled = await evaluateCodeButtons(button, "sdk-disabled");
                        button.Disabled = disabled;
                    }
                    if(button.CustomAttributes != null && button.CustomAttributes.ContainsKey("sdk-hide")){
                        var hidden = await evaluateCodeButtons(button, "sdk-hide");
                        button.Hidden = hidden;
                    }
                    if(button.CustomAttributes != null && button.CustomAttributes.ContainsKey("sdk-show")){
                        var show = await evaluateCodeButtons(button, "sdk-show");
                        button.Hidden = !show;
                    }
                    if(button.Id != null){
                        if(Enum.TryParse<enumTypeButton>(button.Id, out enumTypeButton typeButton)){
                            switch (typeButton)
                            {
                                case enumTypeButton.Create:
                                    CreateButton = button;
                                    break;
                                case enumTypeButton.Duplicate:
                                    DuplicateButton = button;
                                    break;
                                case enumTypeButton.Edit:
                                    EditButton = button;
                                    break;
                                case enumTypeButton.List:
                                    ListButton = button;
                                    break;
                                case enumTypeButton.Delete:
                                    DeleteButton = button;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }else{
                        ExtraButtons.Add(button);
                    }
                    HasExtraButtons = ExtraButtons.Count > 0;
                }
                //_ = InvokeAsync(() => StateHasChanged());
            }
        }

        public async Task<bool> evaluateCodeButtons(Button button, string condition){
            bool disabled = button.Disabled;
            var sdkDisable = button.CustomAttributes[condition];
            if(sdkDisable != null){
                var eject = (bool)await Evaluator.EvaluateCode((string)sdkDisable, BusinessObj); //revisar
                if(eject != null){
                    disabled = eject;
                }
            }
            return disabled;
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            bool changeBusinessName = parameters.DidParameterChange(nameof(BusinessName), BusinessName);

            await base.SetParametersAsync(parameters);

            if(BusinessName != null && (changeBusinessName)){
                Loading = false;
                this.ModelLoaded = false;
                ErrorMsg = "";
                ErrorList = new List<string>();
                await InitView(BusinessName);
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            //InitView();
        }

        private async Task CreateRelationshipAttachment()
        {
            var attachment = BusinessObj.BaseObj.GetType().GetProperty("RowidAttachment");
            if(attachment == null || BusinessName == "BLAttachmentDetail"){
                return;
            }
            ContainAttachments = true;
        }
        private void GoToCreate()
        {
            NavManager.NavigateTo($"{BusinessName}/create/");
        }

        private void GoToDuplicate()
        {
            NavManager.NavigateTo($"{BusinessName}/create/{BusinessObj.BaseObj.Rowid}/");
        }

        private void GoToEdit()
        {
            NavManager.NavigateTo($"{BusinessName}/edit/{BusinessObj.BaseObj.Rowid}/");
        }

        private void GoToList()
        {
            NavManager.NavigateTo($"{BusinessName}/");
        }

        private async Task DeleteBusiness()
        {
            dynamic result = null;
            try{
                result = await BusinessObj.DeleteAsync();
            }catch(Exception ex){
                ErrorMsg = ex.Message;
                ErrorList.Add(ErrorMsg);
            }

            if (result != null && result.Errors.Count > 0)
            {
                var errorMessage ="Custom.Generic.Message.DeleteError";
                NotificationService.ShowError(errorMessage);
                ErrorList.Add(errorMessage);
                // ErrorMsg = "<ul>";
                // foreach (var error in result.Errors)
                // {
                //     ErrorMsg += $"<li>";
                //     ErrorMsg += !string.IsNullOrWhiteSpace(error.Attribute) ? $"{error.Attribute} - " : string.Empty;
                //     ErrorMsg += error.Message.Replace("\n", "<br />");
                //     ErrorMsg += $"</li>";
                // }
                // ErrorMsg += "</ul>";


                return;
            }
            if (result != null)
            {
                if (IsSubpanel)
                {
                    dialogService.Close(false);
                }
                else
                {
                    string uri = NavManager.Uri;
                    NavigationService.RemoveItem(uri);
                    NavManager.NavigateTo($"{BusinessName}/");
                }
            }
        }


        private void OnClickCustomButton(Button button)
        {
            if (!string.IsNullOrEmpty(button.Href))
            {
                if (button.Target == "_blank")
                {
                    _ = JSRuntime.InvokeVoidAsync("window.open", button.Href, "_blank");
                }
                else
                {
                    NavManager.NavigateTo(button.Href);
                }


            }
            else if (!string.IsNullOrEmpty(button.Action))
            {
                Evaluator.EvaluateCode(button.Action, BusinessObj);
            }
        }

        protected virtual async Task CheckPermissions()
        {
            if (FeaturePermissionService != null && !string.IsNullOrEmpty(BusinessName))
            {
                try
                {
                    CanList = await FeaturePermissionService.CheckUserActionPermission(BusinessName, enumSDKActions.Consult, AuthenticationService);
                    CanCreate = await FeaturePermissionService.CheckUserActionPermission(BusinessName, enumSDKActions.Create, AuthenticationService);
                    CanEdit = await FeaturePermissionService.CheckUserActionPermission(BusinessName, enumSDKActions.Edit, AuthenticationService);
                    CanDelete = await FeaturePermissionService.CheckUserActionPermission(BusinessName, enumSDKActions.Delete, AuthenticationService);
                    CanDetail = await FeaturePermissionService.CheckUserActionPermission(BusinessName, enumSDKActions.Consult, AuthenticationService);
                }
                catch (System.Exception)
                {
                }

                if (!CanDetail)
                {
                    ErrorMsg = "Custom.Generic.Unauthorized";
                    NotificationService.ShowError("Custom.Generic.Unauthorized");
                    ErrorList.Add("Custom.Generic.Unauthorized");
                    if(!IsSubpanel){
                        // NavigationService.NavigateTo("/", replace: true);
                    }
                }
            }
        }
    }
}

