using System;
using DevExpress.Blazor;

namespace Siesa.SDK.Frontend.Components.Fields
{
    public static class SDKNumericMask
    {
        public static string Currency { get { return NumericMask.Currency; } }
        public static string WholeNumber { get { return NumericMask.WholeNumber; } }
        public static string RealNumber { get { return NumericMask.RealNumber; } }
        public static string RealNumberWithThousandSeparator { get { return NumericMask.RealNumberWithThousandSeparator; } }
        public static string Percentage { get { return NumericMask.Percentage; } }
        public static string PercentageMultipliedBy100 { get { return NumericMask.PercentageMultipliedBy100; } }
    }
    
}    
