using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Siesa.SDK.Frontend.Components.FormManager.ViewModels;
using Siesa.SDK.Frontend.Components.FormManager.Views;
using Siesa.SDK.Frontend.Utils;

namespace Siesa.SDK.Frontend.Components.FormManager
{
    public static class FormUtils
    {
        public static RenderFragment RenderFreeForm(string viewdef, dynamic BusinessObj, string Title, DynamicViewType ViewContext = DynamicViewType.Create, bool SetTopBar = true, string key = "")
        {
            if(string.IsNullOrEmpty(key)) 
            {
                key = Guid.NewGuid().ToString();
            }
            return (builder) =>
            {
                builder.OpenComponent<FreeForm>(0);
                builder.AddAttribute(1, "Viewdef", viewdef);
                builder.AddAttribute(2, "BusinessObj", BusinessObj);
                builder.AddAttribute(3, "BusinessName", BusinessObj.BusinessName);
                builder.AddAttribute(4, "Title", Title);
                builder.AddAttribute(5, "ViewContext", ViewContext);
                builder.AddAttribute(6, "SetTopBar", SetTopBar);
                builder.SetKey(key);
                builder.CloseComponent();
            };
        }
        public static async Task<string> GenerateFilters(List<List<object>> filters, dynamic BusinessObj)
        {
            var filter = "";
            List<string> filtersOr = new List<string>();
            foreach (var itemAnd in filters){
                List<object> filtersInside = (List<object>)itemAnd;
                if(filtersInside.Count > 0){
                    string filtersOrStr = "";
                    List<string> filtersOrInside = new List<string>();
                    foreach (var item in filtersInside){
                        string filtersOrInsideStr = "";

                        dynamic properties;
                        if(BusinessObj == null){
                            properties = item.GetType().GetProperties();
                        }else{
                            properties = JsonConvert.DeserializeObject<dynamic>(item.ToString()).Properties();
                        }
                        List<string> filtersAnd = new ();
                        foreach (dynamic property in properties){
                            string filtersAndStr;
                            string name = property.Name;
                            var codeValue = "";
                            if(BusinessObj != null){
                                codeValue = property.Value.ToString();
                            }
                            dynamic dynamicValue;
                            if (String.IsNullOrEmpty(codeValue) && BusinessObj != null){
                                continue;
                            }
                            if(BusinessObj != null){
                                dynamicValue = await Evaluator.EvaluateCode(codeValue, BusinessObj);
                            }else{
                                dynamicValue = property.GetValue(item);
                            }
                            dynamicValue = GetFilterValue(dynamicValue);
                            filtersAndStr = GetFiltersStr(name, dynamicValue);
                            filtersAnd.Add(filtersAndStr);
                        }
                        if(filtersAnd.Count > 1){
                            filtersOrInsideStr += "(";
                            filtersOrInsideStr += string.Join(" and ", filtersAnd);
                            filtersOrInsideStr += ")";
                        }else{
                            filtersOrInsideStr += string.Join(" and ", filtersAnd);
                        }
                        filtersOrInside.Add(filtersOrInsideStr);
                    }
                    if(filtersOrInside.Count > 1){
                        filtersOrStr += "(";
                        filtersOrStr += string.Join(" or ", filtersOrInside);
                        filtersOrStr += ")";
                    }else{
                        filtersOrStr += string.Join(" or ", filtersOrInside);
                    }
                    filtersOr.Add(filtersOrStr);
                }
            }
            if(filtersOr.Count > 1){
                filter += "(";
                filter += string.Join(" and ", filtersOr);
                filter += ")";
            }else{
                filter += string.Join(" and ", filtersOr);
            }
            return filter;
        }

        private static string GenerateFilterCondition(string name, dynamic value, string comparisonOperator){
            Type type = value.GetType();

            if (type == typeof(int?) || type == typeof(int) || type == typeof(decimal?) || type == typeof(decimal) || type == typeof(byte?) || type == typeof(byte))
            {
                return $"({name} == null ? 0 : {name}) {comparisonOperator} {value}";
            }
            else if (type == typeof(bool) || type == typeof(bool?))
            {
                return $"({name} == null ? false : {name}) {comparisonOperator} {value}";
            }
            else if (type == typeof(string))
            {
                return $"({name} == null ? \"\" : {name}).ToLower() {comparisonOperator} \"{value}\".ToLower()";
            }
            else if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                var date = (DateTime)value;
                var dateStr = date.ToString("yyyy, MM, dd", CultureInfo.InvariantCulture);
                return $"({name} == null ? DateTime.MinValue : {name}) {comparisonOperator} DateTime({dateStr})";
            }
            else
            {
                return $"{name} {comparisonOperator} {value}";
            }
        }

        private static string GetFiltersStr(string name, dynamic dynamicValue){
            StringBuilder filtersAndStr = new ("(");

            switch (name)
            {
                case var _ when name.EndsWith("__in", StringComparison.Ordinal):
                    var list = JsonConvert.DeserializeObject<List<dynamic>>(JsonConvert.SerializeObject(dynamicValue));
                    if (list.Count > 0)
                    {
                        name = name.Replace("__in", "", StringComparison.Ordinal);
                        AppendFilterConditions(filtersAndStr, list, name, "==", " or ");
                    }
                    break;
                case var _ when name.EndsWith("__notin", StringComparison.Ordinal):
                    var listNotIn = JsonConvert.DeserializeObject<List<dynamic>>(JsonConvert.SerializeObject(dynamicValue));
                    if (listNotIn.Count > 0)
                    {
                        name = name.Replace("__notin", "", StringComparison.Ordinal);
                        AppendFilterConditions(filtersAndStr, listNotIn, name, "!=", " and ");
                    }
                    break;
                case var _ when name.EndsWith("__gt", StringComparison.Ordinal):
                    AppendFilterCondition(filtersAndStr, name.Replace("__gt", "", StringComparison.Ordinal), dynamicValue, ">");
                    break;
                case var _ when name.EndsWith("__gte", StringComparison.Ordinal):
                    AppendFilterCondition(filtersAndStr, name.Replace("__gte", "", StringComparison.Ordinal), dynamicValue, ">=");
                    break;
                case var _ when name.EndsWith("__lt", StringComparison.Ordinal):
                    AppendFilterCondition(filtersAndStr, name.Replace("__lt", "", StringComparison.Ordinal), dynamicValue, "<");
                    break;
                case var _ when name.EndsWith("__lte", StringComparison.Ordinal):
                    AppendFilterCondition(filtersAndStr, name.Replace("__lte", "", StringComparison.Ordinal), dynamicValue, "<=");
                    break;
                case var _ when name.EndsWith("__contains", StringComparison.Ordinal):
                    AppendFilterCondition(filtersAndStr, name.Replace("__contains", "", StringComparison.Ordinal), dynamicValue, ".ToLower().Contains");
                    break;
                default:
                    AppendFilterCondition(filtersAndStr, name, dynamicValue, "==");
                    break;
            }

            filtersAndStr.Append(')');
            return filtersAndStr.ToString();
        }

        private static void AppendFilterConditions(StringBuilder filtersAndStr, List<dynamic> list, string name, string conditionOperator, string separator)
        {
            for (int i = 0; i < list.Count; i++)
            {
                dynamic itemIn = list[i];
                dynamic itemInValue = GetFilterValue(itemIn);
                string condition = GenerateFilterCondition(name, itemInValue, conditionOperator);

                if (i > 0)
                    filtersAndStr.Append(separator);

                filtersAndStr.Append(condition);
            }
        }

        private static void AppendFilterCondition(StringBuilder filtersAndStr, string name, dynamic dynamicValue, string conditionOperator)
        {
            filtersAndStr.Append(GenerateFilterCondition(name, dynamicValue, conditionOperator));
        }
        private static dynamic GetFilterValue(dynamic dynamicValue)
        {
            Type type = dynamicValue.GetType();
            dynamic filterValue = dynamicValue;
            if(type.IsEnum){
                filterValue = (int)dynamicValue;
            }
            return filterValue;
        }
    }
}