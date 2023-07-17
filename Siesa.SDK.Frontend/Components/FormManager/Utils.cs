using System;
using System.Collections.Generic;
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
                        List<string> filtersAnd = new List<string>();
                        foreach (dynamic property in properties){
                            string filtersAndStr = "";
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
            bool isNullable = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

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
                var dateStr = date.ToString("yyyy, MM, dd");
                return $"({name} == null ? DateTime.MinValue : {name}) {comparisonOperator} DateTime({dateStr})";
            }
            else
            {
                return $"{name} {comparisonOperator} {value}";
            }
        }

        private static string GetFiltersStr(string name, dynamic dynamicValue){
            string filtersAndStr = "(";

            if (name.EndsWith("__in"))
            {
                var list = JsonConvert.DeserializeObject<List<dynamic>>(JsonConvert.SerializeObject(dynamicValue));
                if (list.Count > 0)
                {
                    name = name.Replace("__in", "");
                    for (int i = 0; i < list.Count; i++)
                    {
                        dynamic itemIn = list[i];
                        dynamic itemInValue = GetFilterValue(itemIn);
                        string condition = GenerateFilterCondition(name, itemInValue, "==");

                        if (i > 0)
                            filtersAndStr += " or ";

                        filtersAndStr += condition;
                    }
                }
            }
            else if (name.EndsWith("__notin"))
            {
                var list = JsonConvert.DeserializeObject<List<dynamic>>(JsonConvert.SerializeObject(dynamicValue));
                if (list.Count > 0)
                {
                    name = name.Replace("__notin", "");
                    for (int i = 0; i < list.Count; i++)
                    {
                        dynamic itemIn = list[i];
                        dynamic itemInValue = GetFilterValue(itemIn);
                        string condition = GenerateFilterCondition(name, itemInValue, "!=");

                        if (i > 0)
                            filtersAndStr += " and ";

                        filtersAndStr += condition;
                    }
                }
            }
            else if (name.EndsWith("__gt"))
            {
                name = name.Replace("__gt", "");
                filtersAndStr += GenerateFilterCondition(name, dynamicValue, ">");
            }
            else if (name.EndsWith("__gte"))
            {
                name = name.Replace("__gte", "");
                filtersAndStr += GenerateFilterCondition(name, dynamicValue, ">=");
            }
            else if (name.EndsWith("__lt"))
            {
                name = name.Replace("__lt", "");
                filtersAndStr += GenerateFilterCondition(name, dynamicValue, "<");
            }
            else if (name.EndsWith("__lte"))
            {
                name = name.Replace("__lte", "");
                filtersAndStr += GenerateFilterCondition(name, dynamicValue, "<=");
            }
            else if (name.EndsWith("__contains"))
            {
                name = name.Replace("__contains", "");
                filtersAndStr += GenerateFilterCondition(name, dynamicValue, ".ToLower().Contains");
            }
            else
            {
                filtersAndStr += GenerateFilterCondition(name, dynamicValue, "==");
            }

            filtersAndStr += ")";
            return filtersAndStr;
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