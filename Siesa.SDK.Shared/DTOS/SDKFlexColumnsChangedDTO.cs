namespace Siesa.SDK.Shared.DTOS
{
    public class SDKFlexColumnsChangedDTO{
        public SDKFlexColumn column { get; set; }
        public bool formula_changed { get; set; }
        public string action { get; set; }
        public int position { get; set; }
    }
}