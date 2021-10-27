using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System;

namespace Siesa.SDK.Frontend.Components.FormManager.Model.Fields
{
    public class FieldClass<TProperty> : ComponentBase 
    {
        protected TProperty GetValue { get {

                    return (TProperty)BindModel.GetType().GetProperty(FieldName)?.GetValue(BindModel, null);

         } } //getAttr
        [Parameter] public FieldOptions FieldOpt { get; set; }

        [Parameter] public object BindModel { get; set; }

        [Parameter] public TProperty BindProperty { get; set; }

        [Parameter] public TProperty Text { get; set; }

        //[Parameter] public Func<int> RefresParent { get; set; }

        [Parameter] public List<string> EscucharA { get; set; } = new List<string>();

        [Inject] protected IJSRuntime jsRuntime { get; set; }

        [Inject] protected RefreshService RService { get; set; }

        [Parameter] public EventCallback RefreshParent { get; set; }

        private object FinalBindModel { get; set; }

        [Parameter] public string FieldName { get; set; }

        protected void RefreshMe()
        {
           InvokeAsync(StateHasChanged);
        }
        protected override void OnInitialized()
        {
            ////Split FieldOpt.Name
            //string[] fieldPath = FieldOpt.Name.Split('.');
            ////loop through the path
            //object currentObject = BindModel;
            //for (int i = 0; i < (fieldPath.Length - 1); i++)
            //{
            //    currentObject = currentObject.GetType().GetProperty(fieldPath[i]).GetValue(currentObject, null);
            //}
            //FinalBindModel = currentObject;
            //fieldName = fieldPath[fieldPath.Length - 1];

            if(EscucharA != null)
            {
                foreach (string esc in EscucharA)
                {
                    RService.AddObserver(esc, RefreshMe);
                }
            }
        }

        
        public void SetAttr(TProperty value)
        {
            BindModel.GetType().GetProperty(FieldName).SetValue(BindModel, value); //setattr
            RService.Transmit(FieldName); //TODO Considerar ID o tener un customID en esta clase para modelos mas complejos
        }

    }
}
