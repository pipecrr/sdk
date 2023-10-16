namespace Siesa.SDK.Frontend.Components.Fields
{
    public class SDKEnumWrapper<T> {

        public SDKEnumWrapper(T type, string displayText)
        {
            Type = type;
            DisplayText = displayText;
        }
        public SDKEnumWrapper()
        {
        }
        public T Type { get; set; }
        public string DisplayText { get; set; }
    }
}
