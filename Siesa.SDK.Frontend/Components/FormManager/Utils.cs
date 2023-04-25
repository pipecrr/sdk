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

        private static string GetFiltersStr(string name, dynamic dynamicValue)
        {
            string filtersAndStr = "(";
            Type type = dynamicValue.GetType();
            bool isNullable = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) ? true : false;
            if(name.EndsWith("__in")){
                var list = JsonConvert.DeserializeObject<List<dynamic>>(JsonConvert.SerializeObject(dynamicValue));
                if(list.Count > 0){
                    name = name.Replace("__in", "");
                    foreach (var itemIn in list){
                        dynamic itemInValue = GetFilterValue(itemIn);
                        Type typeIn = itemIn.GetType();
                        if(filtersAndStr != "("){
                            filtersAndStr += " or ";
                        }
                        if(typeIn == typeof(int?) || typeIn == typeof(int) || typeIn == typeof(decimal?) || typeIn == typeof(decimal) || typeIn == typeof(byte?) || typeIn == typeof(byte)){
                            filtersAndStr += $"({name} == null ? 0 : {name}) == {itemInValue}";
                        }else if(typeIn == typeof(bool) || typeIn == typeof(bool?)){
                            filtersAndStr += $"({name} == null ? false : {name}) == {itemInValue}";
                        }else if(typeIn == typeof(string)){
                            filtersAndStr += $"({name} == null ? \"\" : {name}) == {itemInValue}";
                        }
                        else{
                            filtersAndStr += $"{name} == {itemInValue}";
                        }
                    }
                }
            }else if(name.EndsWith("__notin")){
                var list = JsonConvert.DeserializeObject<List<dynamic>>(JsonConvert.SerializeObject(dynamicValue));
                if(list.Count > 0){
                    name = name.Replace("__notin", "");
                    foreach (var itemIn in list){
                        dynamic itemInValue = GetFilterValue(itemIn);
                        Type typeIn = itemIn.GetType();
                        if(filtersAndStr != "("){
                            filtersAndStr += " and ";
                        }
                        if(typeIn == typeof(int?) || typeIn == typeof(int) || typeIn == typeof(decimal?) || typeIn == typeof(decimal) || typeIn == typeof(byte?) || typeIn == typeof(byte)){
                            filtersAndStr += $"({name} == null ? 0 : {name}) != {itemInValue}";
                        }else if(typeIn == typeof(bool) || typeIn == typeof(bool?)){
                            filtersAndStr += $"({name} == null ? false : {name}) != {itemInValue}";
                        }else if(typeIn == typeof(string)){
                            filtersAndStr += $"({name} == null ? \"\" : {name}) != {itemInValue}";
                        }else{
                            filtersAndStr += $"{name} != {itemInValue}";
                        }
                    }
                }
            }
            else if(name.EndsWith("__gt")){
                name = name.Replace("__gt", "");
                if(type == typeof(int?) || type == typeof(int) || type == typeof(decimal?) || type == typeof(decimal) || type == typeof(byte?) || type == typeof(byte)){
                    filtersAndStr += $"({name} == null ? 0 : {name}) > {dynamicValue}";
                }else{
                    filtersAndStr += $"{name} > {dynamicValue}";
                }
            }else if(name.EndsWith("__gte")){
                name = name.Replace("__gte", "");
                if(type == typeof(int?) || type == typeof(int) || type == typeof(decimal?) || type == typeof(decimal) || type == typeof(byte?) || type == typeof(byte)){
                    filtersAndStr += $"({name} == null ? 0 : {name}) >= {dynamicValue}";
                }else{
                    filtersAndStr += $"{name} >= {dynamicValue}";
                }                
            }else if(name.EndsWith("__lt")){
                name = name.Replace("__lt", "");
                if(type == typeof(int?) || type == typeof(int) || type == typeof(decimal?) || type == typeof(decimal) || type == typeof(byte?) || type == typeof(byte)){
                    filtersAndStr += $"({name} == null ? 0 : {name}) < {dynamicValue}";
                }else{
                    filtersAndStr += $"{name} < {dynamicValue}";
                }
            }else if(name.EndsWith("__lte")){
                name = name.Replace("__lte", "");
                if(type == typeof(int?) || type == typeof(int) || type == typeof(decimal?) || type == typeof(decimal) || type == typeof(byte?) || type == typeof(byte)){
                    filtersAndStr += $"({name} == null ? 0 : {name}) <= {dynamicValue}";
                }else{
                    filtersAndStr += $"{name} <= {dynamicValue}";
                }
            }else if(name.EndsWith("__contains")){
                name = name.Replace("__contains", "");
                filtersAndStr += $"({name} == null ? \"\" : {name}).ToLower().Contains(\"{dynamicValue}\".ToLower())";
            }else{
                if(type == typeof(int?) || type == typeof(int) || type == typeof(decimal?) || type == typeof(decimal) || type == typeof(byte?) || type == typeof(byte)){
                    filtersAndStr += $"({name} == null ? 0 : {name}) == {dynamicValue}";
                }else if(type == typeof(bool) || type == typeof(bool?)){
                    filtersAndStr += $"({name} == null ? false : {name}) == {dynamicValue}";
                }else if(type == typeof(string)){
                    filtersAndStr += $"({name} == null ? \"\" : {name}).ToLower() == \"{dynamicValue}\".ToLower()";
                }else{
                    filtersAndStr += $"{name} == {dynamicValue}";
                }
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