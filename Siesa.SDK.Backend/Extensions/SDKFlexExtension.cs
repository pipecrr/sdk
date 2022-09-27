using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Siesa.SDK.Backend.Access;
using Siesa.SDK.Entities;
using Siesa.SDK.Shared.Business;
using Siesa.SDK.Shared.DTOS;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Shared.Utilities;
namespace Siesa.SDK.Backend.Extensions
{
    public static class SDKFlexExtension
    {
        private static Assembly _assemblyInclude = typeof(Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions).Assembly;


        private static Assembly _assemblySelect = typeof(System.Linq.Dynamic.Core.DynamicQueryableExtensions).Assembly;

        private static Assembly _assemblyDynamic = typeof(System.Linq.Dynamic.Core.DynamicEnumerableExtensions).Assembly;
        
        internal static dynamic SDKFlexPreviewData(SDKContext Context, SDKFlexRequestData requestData, IAuthenticationService authenticationService, bool setTop = true)
        {
            var rowidCulture = authenticationService.User.RowidCulture;
            List<SDKFlexColumn> columns = requestData.columns;
            if (columns.Count == 0)
            {
                return new ActionResult<dynamic>() { Data = new List<dynamic>() };
            }

            List<string> strColumns = new List<string>();

            List<SDKFlexFilters> filters = requestData.filters;

            var nameEntity = requestData.selected_class;
            var nameSpaceEntity = requestData.module_path;

            var orderBy = "";

            try
            {
                var entityType = Utilities.SearchType(nameSpaceEntity + "." + nameEntity, true);
                var contextSet = Context.GetType().GetMethod("Set", types: Type.EmptyTypes).MakeGenericMethod(entityType).Invoke(Context, null);
                List<string> relatedColumns = new List<string>();

                var includeMethod = typeof(IQueryable<object>).GetExtensionMethod(_assemblyInclude, "Include", new[] { typeof(IQueryable<object>), typeof(string) });
                
                Dictionary<string,Dictionary<byte,string>> enumsDict = new Dictionary<string, Dictionary<byte, string>>();

                foreach (SDKFlexColumn column in columns)
                {
                    if (column.sortType != null)
                    {
                        orderBy = column.key_name + " " + column.sortType.ToUpper();
                    }
                    var i = columns.IndexOf(column);
                    var columnPath = column.path.Split("::");
                    if (columnPath.Count() == 1)
                    {
                            strColumns.Add("np(" + column.name + ")" + " as " + column.key_name);                        
                    }
                    else
                    {
                        var relatedColumnInclude = string.Join(".", columnPath.Skip(1));
                        if (!relatedColumns.Contains(relatedColumnInclude))
                        {
                            var includeMethodGeneric = includeMethod.MakeGenericMethod(entityType);
                            contextSet = includeMethodGeneric.Invoke(contextSet, new object[] { contextSet, relatedColumnInclude });
                            relatedColumns.Add(relatedColumnInclude);
                        }                        
                        strColumns.Add($"np({relatedColumnInclude}.{column.name}) as {column.key_name}");                        

                    }
                    if (column.type.Equals("SelectField"))
                    {   
                        if(!enumsDict.ContainsKey(column.key_name)){
                            Dictionary<byte,string> enumValue = GetEnumValues(column.name, rowidCulture, Context);
                            enumsDict.Add(column.key_name, enumValue);
                        }
                    }
                }

                string strSelect = string.Join(", ", strColumns);

                createWhere(ref contextSet, filters, entityType, _assemblySelect);

                var selectMethod = typeof(IQueryable).GetExtensionMethod(_assemblySelect, "Select", new[] { typeof(IQueryable), typeof(string), typeof(object[]) });
                contextSet = selectMethod.Invoke(contextSet, new object[] { contextSet, $"new ({strSelect})", null });

                if (!orderBy.Equals(""))
                {
                    var orderMethod = typeof(IQueryable).GetExtensionMethod(_assemblySelect, "OrderBy", new[] { typeof(IQueryable), typeof(string), typeof(object[]) });
                    contextSet = orderMethod.Invoke(contextSet, new object[] { contextSet, orderBy, null });
                }
                
                if(setTop){
                    var takeMethod = typeof(IQueryable).GetExtensionMethod(_assemblySelect, "Take", new[] { typeof(IQueryable), typeof(int) });
                    contextSet = takeMethod.Invoke(contextSet, new object[] { contextSet, 50 });
                }

                _assemblyDynamic = typeof(System.Linq.Dynamic.Core.DynamicEnumerableExtensions).Assembly;
                var dynamicListMethod = typeof(IEnumerable).GetExtensionMethod(_assemblyDynamic, "ToDynamicList", new[] { typeof(IEnumerable) });
                var dynamicList = dynamicListMethod.Invoke(contextSet, new object[] { contextSet });

                var jsonResource = JsonConvert.SerializeObject(dynamicList);
                var resourceDict = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonResource);
            
                if(enumsDict.Count>0){
                    foreach (var item in resourceDict){
                        foreach (var enumkey in enumsDict.Keys)
                        {
                            var enumValue = enumsDict[enumkey];
                            var itemValue = item[enumkey];
                            var description = enumValue[byte.Parse(itemValue.ToString())];
                            item[$"{enumkey}_oreports_key"] = item[enumkey];
                            item[enumkey] = description;
                        }                       
                    }
                    return new ActionResult<dynamic>() { Data = resourceDict};
                }

                if (resourceDict != null)
                {
                    return new ActionResult<dynamic>() { Data = resourceDict };
                }
            }
            catch (Exception e)
            {
                return new ActionResult<dynamic>() { Success = false, Errors = new List<string>() { "Error al crear la consulta" } };
            }


            return null;
        }

        private static void createWhere(ref object select, List<SDKFlexFilters> filters, Type entityType, Assembly assembly)
        {
            Expression combined = null;
            ParameterExpression pe = Expression.Parameter(entityType, entityType.Name);

            foreach (var filter in filters)
            {
                //Expression for accessing Fields name property

                Expression columnNameProperty;
                Type columnType;
                if (filter.path.Contains("::"))
                {
                    var filterColumnPath = filter.path.Split("::").Append(filter.name);
                    columnNameProperty = GetPropertyExpression(pe, string.Join(".", filterColumnPath.Skip(1)));
                    columnType = entityType;
                    foreach (var filterColumn in filterColumnPath.Skip(1))
                    {
                        columnType = columnType.GetProperty(filterColumn).PropertyType;
                    }
                }
                else
                {
                    columnNameProperty = GetPropertyExpression(pe, filter.name);
                    columnType = entityType.GetProperty(filter.name).PropertyType;
                }

                Expression columnValue;
                //the name constant to match 
                var value = filter.equal_from;
                var isNullable = false;

                if(columnType.IsGenericType && columnType.GetGenericTypeDefinition() == typeof(Nullable<>)){
                    isNullable = true;
                }

                if (value == null && !isNullable)
                {
                    value = Activator.CreateInstance(columnType);
                }

                //TimeOnly y TimeSpan se estan tratando como charField por eso se cambia el tipo del columnType
                if (value != null && (columnType == typeof(TimeOnly) || columnType == typeof(TimeSpan)))
                {
                    var valueDate = DateTime.Parse(value.ToString());
                    var valueSplit = valueDate.ToString("HH:mm:ss").Split(":").ToList();
                    int hours = 0;
                    int minutes = 0;
                    int seconds = 0;
                    if (valueSplit.Count >= 1)
                    {
                        hours = int.Parse(valueSplit[0]);
                    }
                    if (valueSplit.Count >= 2)
                    {
                        minutes = int.Parse(valueSplit[1]);
                    }
                    if (valueSplit.Count == 3)
                    {
                        seconds = int.Parse(valueSplit[2]);
                    }

                    value = Activator.CreateInstance(columnType, hours, minutes, seconds);
                }
                
                
                if(isNullable){
                    columnType = Nullable.GetUnderlyingType(columnType) ?? columnType;
                }

                if (value != null && columnType == typeof(DateOnly) && value.GetType() == typeof(DateTime)){
                    var valueSplit = value.ToString().Substring(0,10).Split("/").ToList();
                    int year = int.Parse(valueSplit[2]);
                    int month = int.Parse(valueSplit[1]);
                    int day = int.Parse(valueSplit[0]);
                    
                    value = Activator.CreateInstance(columnType, year, month, day);
                }

                object filterValue;
                var columnNamePropertyNull = columnNameProperty;
                if(isNullable){
                    columnNameProperty = Expression.Convert(columnNameProperty, columnType);
                }
                switch (filter.selected_operator)
                {
                    case "equal":
                        Expression equalExpression;                        
                        if (columnType == typeof(DateTime)){
                            filterValue = Convert.ChangeType(value, columnType);
                            columnValue = Expression.Constant(filterValue);
                            var valueDateEqual = filterValue as DateTime?;
                            Expression greatEqualE = Expression.GreaterThanOrEqual(columnNameProperty, columnValue);
                            Expression lessEqualE = Expression.LessThan(columnNameProperty, Expression.Constant(valueDateEqual?.AddMinutes(1)));
                            equalExpression = Expression.And(lessEqualE, greatEqualE);

                            addExpression(ref combined, equalExpression);
                        }else if(columnType == typeof(DateOnly)){
                            filterValue = Convert.ChangeType(value, columnType);
                            columnValue = Expression.Constant(filterValue);
                            var valueDateEqual = filterValue as DateOnly?;
                            Expression greatEqualE = Expression.GreaterThanOrEqual(columnNameProperty, columnValue);
                            Expression lessEqualE = Expression.LessThanOrEqual(columnNameProperty, Expression.Constant(valueDateEqual));
                            equalExpression = Expression.And(lessEqualE, greatEqualE);

                            addExpression(ref combined, equalExpression);
                        }
                        else{
                            filterValue = Convert.ChangeType(value, columnType);
                            columnValue = Expression.Constant(filterValue);
                            equalExpression = Expression.Equal(columnNameProperty, columnValue);
                        }
                        addExpression(ref combined, equalExpression);
                        break;
                    case "not_equal":
                        if(isNullable){
                            columnNameProperty = Expression.Convert(columnNameProperty, columnType);
                        }
                        filterValue = Convert.ChangeType(value, columnType);
                        columnValue = Expression.Constant(filterValue);
                        Expression notEqualExpression;
                        if (columnType == typeof(DateTime))
                        {
                            var valueDate = filterValue as DateTime?;
                            Expression lessNotEqualE = Expression.LessThan(columnNameProperty, columnValue);
                            Expression greatNotEqualE = Expression.GreaterThan(columnNameProperty, Expression.Constant(valueDate?.AddMinutes(1)));
                            notEqualExpression = Expression.Or(lessNotEqualE, greatNotEqualE);
                        }else if(columnType == typeof(DateOnly)){
                            var valueDate = filterValue as DateOnly?;
                            Expression lessNotEqualE = Expression.LessThan(columnNameProperty, columnValue);
                            Expression greatNotEqualE = Expression.GreaterThan(columnNameProperty, Expression.Constant(valueDate));
                            notEqualExpression = Expression.Or(lessNotEqualE, greatNotEqualE);
                        }else{
                            notEqualExpression = Expression.NotEqual(columnNameProperty, columnValue);
                        }
                        addExpression(ref combined, notEqualExpression);
                        break;
                    case "in":
                        Expression inExpression;
                        Expression inExpression2;
                        if(columnType.BaseType == typeof(Enum)){
                            var listValueEnum = JsonConvert.DeserializeObject<List<byte>>(value.ToString());
                            inExpression = Expression.Equal(columnNameProperty, Expression.Constant(Convert.ChangeType(Enum.Parse(columnType,listValueEnum[0].ToString()), columnType)));
                            for (int i = 1; i < listValueEnum.Count; i++){
                                var enumValue = Convert.ChangeType(Enum.Parse(columnType,listValueEnum[i].ToString()), columnType);
                                inExpression2 = Expression.Equal(columnNameProperty, Expression.Constant(enumValue));
                                inExpression = Expression.Or(inExpression, inExpression2);
                            }
                        }else {
                            var listValue = JsonConvert.DeserializeObject<List<bool>>(value.ToString());
                            inExpression = Expression.Equal(columnNameProperty, Expression.Constant(listValue[0]));
                            for (int i = 1; i < listValue.Count; i++){
                                inExpression2 = Expression.Equal(columnNameProperty, Expression.Constant(listValue[i]));
                                inExpression = Expression.Or(inExpression, inExpression2);
                            }                            
                        }
                        addExpression(ref combined, inExpression);
                        break;
                    case "exclude":
                        Expression excludeExpression;
                        Expression excludeExpression2;
                        if(columnType.BaseType == typeof(Enum)){
                            var listValueExclude = JsonConvert.DeserializeObject<List<byte>>(value.ToString());
                            excludeExpression = Expression.NotEqual(columnNameProperty, Expression.Constant(Convert.ChangeType(Enum.Parse(columnType,listValueExclude[0].ToString()), columnType)));
                            for (int i = 1; i < listValueExclude.Count; i++){
                                var enumValue = Convert.ChangeType(Enum.Parse(columnType,listValueExclude[i].ToString()), columnType);
                                excludeExpression2 = Expression.NotEqual(columnNameProperty, Expression.Constant(enumValue));
                                excludeExpression = Expression.And(excludeExpression, excludeExpression2);
                            }
                        }else{
                            var listValueExclude = JsonConvert.DeserializeObject<List<bool>>(value.ToString());
                            excludeExpression = Expression.NotEqual(columnNameProperty, Expression.Constant(listValueExclude[0]));
                            for (int i = 1; i < listValueExclude.Count; i++){
                                excludeExpression2 = Expression.NotEqual(columnNameProperty, Expression.Constant(listValueExclude[i]));
                                excludeExpression = Expression.And(excludeExpression, excludeExpression2);
                            }
                        }
                        addExpression(ref combined, excludeExpression);
                        break;
                    case "starts_with":
                        filterValue = Convert.ChangeType(value, columnType);
                        columnValue = Expression.Constant(filterValue);
                        Expression startsWithExpression = Expression.Call(columnNameProperty, getMethodInfo("StartsWith", typeof(string)), columnValue);
                        addExpression(ref combined, startsWithExpression);
                        break;
                    case "end_with":
                        filterValue = Convert.ChangeType(value, columnType);
                        columnValue = Expression.Constant(filterValue);
                        Expression endWithExpression = Expression.Call(columnNameProperty, getMethodInfo("EndsWith", typeof(string)), columnValue);
                        addExpression(ref combined, endWithExpression);
                        break;
                    case "contains":
                        filterValue = Convert.ChangeType(value, columnType);
                        columnValue = Expression.Constant(filterValue);
                        Expression containsExpression = Expression.Call(columnNameProperty, getMethodInfo("Contains", typeof(string)), columnValue);
                        addExpression(ref combined, containsExpression);
                        break;
                    case "null_or_empty":
                    case "empty":
                        Expression nullOrEmptyExpression = null;
                        if (columnType == typeof(DateTime) || columnType == typeof(DateOnly)){
                            filterValue = Convert.ChangeType(Activator.CreateInstance(columnType), columnType);
                            columnValue = Expression.Constant(filterValue);
                            nullOrEmptyExpression = Expression.Equal(columnNameProperty, columnValue);
                        }else if(columnType == typeof(string)){
                            nullOrEmptyExpression = Expression.Call(getMethodInfo("IsNullOrEmpty", typeof(string)), columnNameProperty);
                        }

                        if(isNullable && columnType != typeof(string))
                        {
                            var nullExpression = Expression.Equal(columnNamePropertyNull, Expression.Constant(null));
                            if(nullOrEmptyExpression != null)
                            {
                                nullOrEmptyExpression = Expression.Or(nullOrEmptyExpression, nullExpression);
                            }else{
                                nullOrEmptyExpression = nullExpression;
                            }
                        }
                        addExpression(ref combined, nullOrEmptyExpression);
                        break;
                    case "not_empty":
                        Expression notEmptyExpression = null;
                        if (columnType == typeof(DateTime) || columnType == typeof(DateOnly)){
                            filterValue = Convert.ChangeType(Activator.CreateInstance(columnType), columnType);
                            columnValue = Expression.Constant(filterValue);
                            notEmptyExpression = Expression.NotEqual(columnNameProperty, columnValue);
                        }else if(columnType == typeof(string)){
                            notEmptyExpression = Expression.Call(getMethodInfo("IsNullOrEmpty", typeof(string)), columnNameProperty);
                        }
                        if(isNullable && columnType != typeof(string))
                        {
                            var notEmptyExpression2 = Expression.NotEqual(columnNamePropertyNull, Expression.Constant(null));
                            if(notEmptyExpression != null)
                            {
                                notEmptyExpression = Expression.And(notEmptyExpression, notEmptyExpression2);
                            }else{
                                notEmptyExpression = notEmptyExpression2;
                            }
                        }
                        addExpression(ref combined, notEmptyExpression);                        
                        break;
                    case "gt":
                        filterValue = Convert.ChangeType(value, columnType);
                        columnValue = Expression.Constant(filterValue);
                        Expression greatThanExpression = Expression.GreaterThan(columnNameProperty, columnValue);
                        addExpression(ref combined, greatThanExpression);
                        break;
                    case "lt":
                        filterValue = Convert.ChangeType(value, columnType);
                        columnValue = Expression.Constant(filterValue);
                        Expression lessThanExpression = Expression.LessThan(columnNameProperty, columnValue);
                        addExpression(ref combined, lessThanExpression);
                        break;
                    case "gte":
                        filterValue = Convert.ChangeType(value, columnType);
                        columnValue = Expression.Constant(filterValue);
                        Expression greatThanOrEqualsExpression = Expression.GreaterThanOrEqual(columnNameProperty, columnValue);
                        addExpression(ref combined, greatThanOrEqualsExpression);
                        break;
                    case "between":
                        filterValue = Convert.ChangeType(value, columnType);
                        columnValue = Expression.Constant(filterValue);
                        var filterTo = filter.to;
                        Expression columnTo = null;
                        if(columnType == typeof(DateOnly)){
                            var valueSplit = filterTo.ToString().Substring(0,10).Split("-").ToList();
                            int yearB = int.Parse(valueSplit[0]);
                            int monthB = int.Parse(valueSplit[1]);
                            int dayB = int.Parse(valueSplit[2]);
                            
                            var filterToDateOnly = Activator.CreateInstance(columnType, yearB, monthB, dayB);
                            columnTo = Expression.Constant(Convert.ChangeType(filterToDateOnly, columnType));
                        }else{
                            columnTo = Expression.Constant(Convert.ChangeType(filter.to, columnType));
                        }
                        Expression greaterExpression = Expression.GreaterThan(columnNameProperty, columnValue);
                        Expression lessExpression = Expression.LessThan(columnNameProperty, columnTo);
                        Expression betweenExpression = Expression.And(lessExpression, greaterExpression);
                        addExpression(ref combined, betweenExpression);
                        break;
                    case "before":
                        filterValue = Convert.ChangeType(value, columnType);
                        columnValue = Expression.Constant(filterValue);
                        Expression beforeExpression = Expression.LessThan(columnNameProperty, columnValue);
                        addExpression(ref combined, beforeExpression);
                        break;
                    case "after":
                        filterValue = Convert.ChangeType(value, columnType);
                        columnValue = Expression.Constant(filterValue);
                        Expression afterExpression = Expression.GreaterThan(columnNameProperty, columnValue);
                        addExpression(ref combined, afterExpression);
                        break;
                    case "in_past":
                        if(columnType == typeof(DateOnly)){
                            columnValue = Expression.Constant(Convert.ChangeType(DateOnly.FromDateTime(DateTime.Now), columnType));
                        }else{
                            columnValue = Expression.Constant(Convert.ChangeType(DateTime.Now, columnType));
                        }
                        Expression inPastExpression = Expression.LessThan(columnNameProperty, columnValue);
                        addExpression(ref combined, inPastExpression);
                        break;
                    case "in_future":
                    if(columnType == typeof(DateOnly)){
                            columnValue = Expression.Constant(Convert.ChangeType(DateOnly.FromDateTime(DateTime.Now), columnType));
                        }else{
                            columnValue = Expression.Constant(Convert.ChangeType(DateTime.Now, columnType));
                        }                        
                        Expression inFuctureExpression = Expression.GreaterThan(columnNameProperty, columnValue);
                        addExpression(ref combined, inFuctureExpression);
                        break;
                    case "current_month":

                        DateTime todayCurrentMonth = DateTime.Now;
                        var month = todayCurrentMonth.Month;
                        var year = todayCurrentMonth.Year;
                        var numDays = DateTime.DaysInMonth(year, month);

                        var startDateMonth = Activator.CreateInstance(columnType, year, month, 1);
                        var endDateMonth = Activator.CreateInstance(columnType, year, month, numDays);

                        columnValue = Expression.Constant(Convert.ChangeType(startDateMonth, columnType));
                        Expression greaterCurrentMonthE = Expression.GreaterThanOrEqual(columnNameProperty, columnValue);

                        columnValue = Expression.Constant(Convert.ChangeType(endDateMonth, columnType));
                        Expression lessCurrentMonthE = Expression.LessThanOrEqual(columnNameProperty, columnValue);

                        var currentMonthExpression = Expression.And(lessCurrentMonthE, greaterCurrentMonthE);
                        addExpression(ref combined, currentMonthExpression);
                        break;
                    case "current_week":
                        DateTime todayCurrentWeek = DateTime.Now;
                        var dayWeek = ((double)todayCurrentWeek.DayOfWeek);
                        var startDateWeek = todayCurrentWeek.AddDays(-dayWeek);
                        var endDateWeek = todayCurrentWeek.AddDays(6 - dayWeek);
                        
                        var startDateW = Activator.CreateInstance(columnType, startDateWeek.Year, startDateWeek.Month, startDateWeek.Day);
                        var endDateW = Activator.CreateInstance(columnType, endDateWeek.Year, endDateWeek.Month, endDateWeek.Day);

                        columnValue = Expression.Constant(Convert.ChangeType(startDateW, columnType));
                        Expression greaterCurrentWeekE = Expression.GreaterThanOrEqual(columnNameProperty, columnValue);

                        columnValue = Expression.Constant(Convert.ChangeType(endDateW, columnType));
                        Expression lessCurrentWeekE = Expression.LessThanOrEqual(columnNameProperty, columnValue);

                        var currentWeekExpression = Expression.And(lessCurrentWeekE, greaterCurrentWeekE);
                        addExpression(ref combined, currentWeekExpression);
                        break;
                    case "last_month":
                        DateTime todayLastMonth = DateTime.Now;
                        var lastMonth = todayLastMonth.Month - 1;
                        var lastYear = todayLastMonth.Year;

                        if (lastMonth <= 0)
                        {
                            lastMonth = 12;
                            lastYear = lastYear - 1;
                        }
                        var numDaysLastMonth = DateTime.DaysInMonth(lastYear, lastMonth);
                        var startDateLastMonth = Activator.CreateInstance(columnType, lastYear, lastMonth, 1);
                        var endDateLastMonth = Activator.CreateInstance(columnType, lastYear, lastMonth, numDaysLastMonth);

                        columnValue = Expression.Constant(Convert.ChangeType(startDateLastMonth, columnType));
                        Expression greaterLastMonthE = Expression.GreaterThanOrEqual(columnNameProperty, columnValue);

                        columnValue = Expression.Constant(Convert.ChangeType(endDateLastMonth, columnType));
                        Expression lessLastMonthE = Expression.LessThanOrEqual(columnNameProperty, columnValue);

                        var lastMonthExpression = Expression.And(lessLastMonthE, greaterLastMonthE);
                        addExpression(ref combined, lastMonthExpression);
                        break;
                    case "today":
                        DateTime today = DateTime.Now;

                        var startDate = Activator.CreateInstance(columnType, today.Year, today.Month, today.Day);
                        today = today.AddDays(1);
                        var endDate = Activator.CreateInstance(columnType, today.Year, today.Month, today.Day);

                        columnValue = Expression.Constant(Convert.ChangeType(startDate, columnType));
                        Expression greaterTodayE = Expression.GreaterThan(columnNameProperty, columnValue);

                        columnValue = Expression.Constant(Convert.ChangeType(endDate, columnType));
                        Expression lessTodayE = Expression.LessThan(columnNameProperty, columnValue);

                        var todayExpression = Expression.And(lessTodayE, greaterTodayE);
                        addExpression(ref combined, todayExpression);
                        break;
                    case "last_n_days":
                        DateTime todayLastNDate = DateTime.Now;
                        int daysLast = int.Parse(value.ToString());
                        var endDateLastnDays = Activator.CreateInstance(columnType, todayLastNDate.Year, todayLastNDate.Month, todayLastNDate.Day);
                        todayLastNDate = todayLastNDate.Date.AddDays(-daysLast);
                        var startDateLastnDays = Activator.CreateInstance(columnType, todayLastNDate.Year, todayLastNDate.Month, todayLastNDate.Day);

                        columnValue = Expression.Constant(Convert.ChangeType(startDateLastnDays, columnType));
                        Expression greaterLastnDayE = Expression.GreaterThan(columnNameProperty, columnValue);

                        columnValue = Expression.Constant(Convert.ChangeType(endDateLastnDays, columnType));
                        Expression lessLastnDayE = Expression.LessThan(columnNameProperty, columnValue);

                        var lastnDaysExpression = Expression.And(lessLastnDayE, greaterLastnDayE);
                        addExpression(ref combined, lastnDaysExpression);
                        break;
                    case "next_n_days":
                        DateTime todayNextNDate = DateTime.Now;
                        int daysNext = int.Parse(value.ToString());
                        var endDateNextnDays = Activator.CreateInstance(columnType, todayNextNDate.Year, todayNextNDate.Month, todayNextNDate.Day);
                        todayNextNDate = todayNextNDate.Date.AddDays(daysNext);
                        var startDateNextnDays = Activator.CreateInstance(columnType, todayNextNDate.Year, todayNextNDate.Month, todayNextNDate.Day);

                        columnValue = Expression.Constant(Convert.ChangeType(startDateNextnDays, columnType));
                        Expression greaterNextnDayE = Expression.GreaterThan(columnNameProperty, columnValue);

                        columnValue = Expression.Constant(Convert.ChangeType(endDateNextnDays, columnType));
                        Expression lessNextnDayE = Expression.LessThan(columnNameProperty, columnValue);

                        var nextnDaysExpression = Expression.And(lessNextnDayE, greaterNextnDayE);
                        addExpression(ref combined, nextnDaysExpression);
                        break;
                    case "this_year":
                        var thisYear = DateTime.Now.Year;

                        var startDateThisYear = Activator.CreateInstance(columnType,thisYear,1,1); 
                        var endDateThisYear = Activator.CreateInstance(columnType, thisYear, 12, 31); 

                        columnValue = Expression.Constant(Convert.ChangeType(startDateThisYear, columnType));
                        Expression greaterThisYearE = Expression.GreaterThan(columnNameProperty, columnValue);

                        columnValue = Expression.Constant(Convert.ChangeType(endDateThisYear, columnType));
                        Expression lessThisYearE = Expression.LessThan(columnNameProperty, columnValue);

                        var thisYearExpression = Expression.And(lessThisYearE, greaterThisYearE);
                        addExpression(ref combined, thisYearExpression);
                        break;
                    case "last_year":
                        var yearLastYear = DateTime.Now.Year - 1;

                        var startDateLastYear = Activator.CreateInstance(columnType, yearLastYear, 1, 1);
                        var endDateLastYear = Activator.CreateInstance(columnType, yearLastYear, 12, 31);

                        columnValue = Expression.Constant(Convert.ChangeType(startDateLastYear, columnType));
                        Expression greaterLastYearE = Expression.GreaterThan(columnNameProperty, columnValue);

                        columnValue = Expression.Constant(Convert.ChangeType(endDateLastYear, columnType));
                        Expression lessLastYearE = Expression.LessThan(columnNameProperty, columnValue);

                        var lastYearExpression = Expression.And(lessLastYearE, greaterLastYearE);
                        addExpression(ref combined, lastYearExpression);
                        break;
                    case "next_year":
                        var yearNextYear = DateTime.Now.Year + 1;

                        var startDateNextYear = Activator.CreateInstance(columnType, yearNextYear, 1, 1);
                        var endDateNextYear = Activator.CreateInstance(columnType, yearNextYear, 12, 31);

                        columnValue = Expression.Constant(Convert.ChangeType(startDateNextYear, columnType));
                        Expression greaterNextYearE = Expression.GreaterThan(columnNameProperty, columnValue);

                        columnValue = Expression.Constant(Convert.ChangeType(endDateNextYear, columnType));
                        Expression lessNextYearE = Expression.LessThan(columnNameProperty, columnValue);

                        var nextYearExpression = Expression.And(lessNextYearE, greaterNextYearE);
                        addExpression(ref combined, nextYearExpression);
                        break;
                    default:
                        break;
                }
            }
            if (combined != null)
            {
                var funcExpression = typeof(Func<,>).MakeGenericType(new Type[] { entityType, typeof(bool) });
                var returnExp = Expression.Lambda(funcExpression, combined, new ParameterExpression[] { pe });
                var whereMethod = typeof(IQueryable<object>).GetExtensionMethod(assembly, "Where", new[] { typeof(IQueryable<object>), typeof(LambdaExpression) });
                var whereMethodGeneric = whereMethod.MakeGenericMethod(entityType);
                select = whereMethodGeneric.Invoke(select, new object[] { select, returnExp });
            }
        }

        private static Expression GetPropertyExpression(Expression pe, string chain)
        {
            var properties = chain.Split('.');
            foreach (var property in properties)
                pe = Expression.Property(pe, property);

            return pe;
        }

        private static void addExpression(ref Expression combined, Expression expression)
        {
            if (combined == null)
            {
                combined = expression;
            }
            else
            {
                combined = Expression.And(combined, expression);
            }
        }

        private static MethodInfo getMethodInfo(string nameMethod, Type type)
        {
            var r = type.GetMethod(nameMethod, new Type[] { type });
            return r;
        }

        public static Dictionary<byte, string> GetEnumValues(string enumName, Int64 cultureRowid, SDKContext context){
            var enumValues = context.Set<E00025_EnumValue>().Where(x => x.Enum.Id == "enum"+enumName).Join(
                context.Set<E00022_ResourceDescription>(),
                x =>  "Enum." + x.Enum.Id + "." + x.Id,
                x => x.Resource.Id,
                (x, y) => new { x.Value, y.Description, y.RowidCulture }).Where(x => x.RowidCulture == cultureRowid)
                .ToDictionary(x => x.Value, x => x.Description);

            if (enumValues.Keys.Count > 0)
            {
                return enumValues;
            }
            else
            {
                var cultureLanguageEnum = context.Set<E00021_Culture>()
                .Where(x => x.Rowid == cultureRowid)
                .Select(x => x.LanguageCode).FirstOrDefault();

                if (cultureLanguageEnum == null)
                {
                    return null;
                }

                var mainCultureRowid = context.Set<E00021_Culture>()
                .Where(x => x.LanguageCode == cultureLanguageEnum && x.CountryCode == null)
                .Select(x => x.Rowid).FirstOrDefault();

                if (mainCultureRowid == 0)
                {
                    return null;
                }

                enumValues = context.Set<E00025_EnumValue>()
                .Where(x => x.Enum.Id == enumName)
                .Join(context.Set<E00022_ResourceDescription>(), x => "Enum." + x.Enum.Id + "." + x.Id, x => x.Resource.Id,
                (x, y) => new { x.Value, y.Description, y.RowidCulture })
                .Where(x => x.RowidCulture == mainCultureRowid)
                .ToDictionary(x => x.Value, x => x.Description);

                if (enumValues != null)
                {
                    return enumValues;
                }

                return null;
            }
            
        }
    }
}