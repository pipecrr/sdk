using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Siesa.SDK.Backend.Access;
using Siesa.SDK.Entities;
using Siesa.SDK.Shared.Business;
using Siesa.SDK.Shared.DTOS;
using Siesa.SDK.Shared.Utilities;

namespace Siesa.SDK.Backend.Extensions
{
    public static class SDKFlexExtension
    {
        private static Assembly _assemblyInclude = typeof(Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions).Assembly;

        private static Assembly _assemblySelect = typeof(System.Linq.Dynamic.Core.DynamicQueryableExtensions).Assembly;

        private static Assembly _assemblyDynamic = typeof(System.Linq.Dynamic.Core.DynamicEnumerableExtensions).Assembly;

        internal static dynamic SDKFlexPreviewData(SDKContext Context, SDKFlexRequestData requestData, bool setTop = true)
        {
            List<SDKFlexColumn> columns = requestData.columns;
            if (columns.Count == 0)
            {
                return new BadRequestResult<dynamic>();
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
                        var enumT = Context.Set<E00025_EnumValue>()
                        .Include(x => x.Enum)
                        .Where(
                            x =>
                            x.Enum.Id == "enum" + column.name)
                        .Select(x => new E00025_EnumValue { Id = x.Enum.Id + "." + x.Id, Value = x.Value });

                        var ejumJoin = enumT.Join(
                            Context.Set<E00022_ResourceDescription>(),
                            x => "Enum." + x.Id,
                            x => x.Resource.Id,
                            (x, y) => new { x , y })
                            .Where(x => x.y.RowidCulture == 1);
                        
  /*                      var d = contextSet as IQueryable<object>;
                        d.Join(
                            ejumJoin, 
                            x => x.GetType().GetProperty(column.name).GetValue(x), x => x.Value, (x, y) => new { x, y.Description })
                            .Select(x => new { x.x, x.Description })
                            .Select($"new (np({column.name}) as {column.key_name}, Description as {column.key_name}_Description)");
*/
                       /* var _assemblyJoin = typeof(System.Linq.Queryable).Assembly;

                        var joinMethod = typeof(IQueryable<object>).GetExtensionMethod(_assemblyJoin, "Join", new[] { typeof(IQueryable<object>), typeof(IEnumerable<object>), typeof(Expression<Func<object, object>>), typeof(Expression<Func<object, object>>), typeof(Expression<Func<object, object, object>>), typeof(Expression<Func<object, bool>>) });

                        ParameterExpression pe1 = Expression.Parameter(typeof(E00025_EnumValue), "x");
                        var funcExpression1 = typeof(Func<,>).MakeGenericType(new Type[] { typeof(E00025_EnumValue), typeof(object) });

                        var returnExp1 = Expression.Lambda(funcExpression1, Expression.PropertyOrField(pe1, "Id"), pe1);

                        ParameterExpression pe2 = Expression.Parameter(typeof(E00022_ResourceDescription), "x");
                        var funcExpression2 = typeof(Func<,>).MakeGenericType(new Type[] { typeof(E00022_ResourceDescription), typeof(object) });
                        
                        var ce = Expression.Constant("Resource");
                        var returnExp2 = Expression.Lambda(funcExpression2, Expression.PropertyOrField(pe2, ce+".Id"), pe2);


                        var joinMethodGeneric = joinMethod.MakeGenericMethod(typeof(object), typeof(IEnumerable<object>), typeof(object), typeof(object), typeof(object));*/
                        
                        /*var prb = contextSet.Join(
                            ejumJoin,
                            x => x.GetType().GetProperty(column.name).GetValue(x),
                            x => x.Value,
                            (x, y) => new { x, y.Description })
                            .Select(x => new { x.x, x.Description });*/
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

                var resource = dynamicList;
                if (resource != null)
                {
                    return new ActionResult<dynamic>() { Data = resource };
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

                if (value == null && (!columnType.IsGenericType || columnType.GetGenericTypeDefinition() != typeof(Nullable<>)))
                {
                    value = Activator.CreateInstance(columnType);
                }

                //TimeOnly y TimeSpan se estan tratando como charField por eso se cambia el tipo del columnType
                if (columnType == typeof(TimeOnly) || columnType == typeof(TimeSpan))
                {
                    var valueSplit = value.ToString().Split(":").ToList();
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

                object filterValue;

                switch (filter.selected_operator)
                {

                    case "equal":
                        filterValue = Convert.ChangeType(value, columnType);
                        columnValue = Expression.Constant(filterValue);
                        Expression equalExpression;
                        if (columnType == typeof(DateTime))
                        {
                            var x = filterValue as DateTime?;
                            Expression greatEqualE = Expression.GreaterThanOrEqual(columnNameProperty, columnValue);
                            Expression lessEqualE = Expression.LessThan(columnNameProperty, Expression.Constant(x?.AddMinutes(1)));
                            equalExpression = Expression.And(lessEqualE, greatEqualE);
                            addExpression(ref combined, equalExpression);
                        }
                        else
                        {
                            equalExpression = Expression.Equal(columnNameProperty, columnValue);
                        }
                        addExpression(ref combined, equalExpression);
                        break;
                    case "not_equal":
                        filterValue = Convert.ChangeType(value, columnType);
                        columnValue = Expression.Constant(filterValue);
                        Expression notEqualExpression;
                        if (columnType == typeof(DateTime))
                        {
                            var xx = filterValue as DateTime?;
                            Expression lessNotEqualE = Expression.LessThan(columnNameProperty, columnValue);
                            Expression greatNotEqualE = Expression.GreaterThan(columnNameProperty, Expression.Constant(xx?.AddMinutes(1)));
                            notEqualExpression = Expression.Or(lessNotEqualE, greatNotEqualE);
                        }
                        else
                        {
                            notEqualExpression = Expression.NotEqual(columnNameProperty, columnValue);
                        }
                        addExpression(ref combined, notEqualExpression);
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
                        Expression nullOrEmptyExpression;
                        if (columnType == typeof(DateTime))
                        {
                            filterValue = Convert.ChangeType(Activator.CreateInstance(columnType), columnType);
                            columnValue = Expression.Constant(filterValue);
                            nullOrEmptyExpression = Expression.Equal(columnNameProperty, columnValue);
                        }
                        else
                        {
                            nullOrEmptyExpression = Expression.Call(getMethodInfo("IsNullOrEmpty", typeof(string)), columnNameProperty);
                        }
                        addExpression(ref combined, nullOrEmptyExpression);
                        break;
                    case "not_empty":
                        Expression notEmptyExpression;
                        if (columnType == typeof(DateTime))
                        {
                            filterValue = Convert.ChangeType(Activator.CreateInstance(columnType), columnType);
                            columnValue = Expression.Constant(filterValue);
                            notEmptyExpression = Expression.Equal(columnNameProperty, columnValue);
                        }
                        else
                        {
                            notEmptyExpression = Expression.Call(getMethodInfo("IsNullOrEmpty", typeof(string)), columnNameProperty);
                        }
                        notEmptyExpression = Expression.Not(notEmptyExpression);
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
                        Expression columnTo = Expression.Constant(Convert.ChangeType(filter.to, columnType));
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
                        columnValue = Expression.Constant(Convert.ChangeType(DateTime.Now, columnType));
                        Expression inPastExpression = Expression.LessThan(columnNameProperty, columnValue);
                        addExpression(ref combined, inPastExpression);
                        break;
                    case "in_future":
                        columnValue = Expression.Constant(Convert.ChangeType(DateTime.Now, columnType));
                        Expression inFuctureExpression = Expression.GreaterThan(columnNameProperty, columnValue);
                        addExpression(ref combined, inFuctureExpression);
                        break;
                    case "current_month":
                        DateTime todayCurrentMonth = DateTime.Now;
                        var month = todayCurrentMonth.Month;
                        var year = todayCurrentMonth.Year;
                        var numDays = DateTime.DaysInMonth(year, month);
                        var startDateMonth = new DateTime(year, month, 1);
                        var endDateMonth = new DateTime(year, month, numDays);

                        columnValue = Expression.Constant(Convert.ChangeType(startDateMonth, typeof(DateTime)));
                        Expression greaterCurrentMonthE = Expression.GreaterThanOrEqual(columnNameProperty, columnValue);

                        columnValue = Expression.Constant(Convert.ChangeType(endDateMonth, typeof(DateTime)));
                        Expression lessCurrentMonthE = Expression.LessThanOrEqual(columnNameProperty, columnValue);

                        var currentMonthExpression = Expression.And(lessCurrentMonthE, greaterCurrentMonthE);
                        addExpression(ref combined, currentMonthExpression);
                        break;
                    case "current_week":
                        DateTime todayCurrentWeek = DateTime.Now;
                        var dayWeek = ((double)todayCurrentWeek.DayOfWeek);
                        var startDayWeek = todayCurrentWeek.AddDays(-dayWeek);
                        var endDateWeek = todayCurrentWeek.AddDays(6 - dayWeek);

                        columnValue = Expression.Constant(Convert.ChangeType(startDayWeek, typeof(DateTime)));
                        Expression greaterCurrentWeekE = Expression.GreaterThanOrEqual(columnNameProperty, columnValue);

                        columnValue = Expression.Constant(Convert.ChangeType(endDateWeek, typeof(DateTime)));
                        Expression lessCurrentWeekE = Expression.LessThanOrEqual(columnNameProperty, columnValue);

                        var currentWeekExpression = Expression.And(lessCurrentWeekE, greaterCurrentWeekE);
                        addExpression(ref combined, currentWeekExpression);
                        break;
                    case "last_month":
                        DateTime todayLastMonth = DateTime.Now;
                        var lastMonth = todayLastMonth.Month - 1;
                        var lastYear = todayLastMonth.Year;

                        var startDateLastMonth = new DateTime();
                        var endDateLastMonth = new DateTime();
                        if (lastMonth <= 0)
                        {
                            startDateLastMonth = new DateTime(lastYear - 1, 12, 1);
                            endDateLastMonth = new DateTime(lastYear - 1, 12, 31);
                        }
                        else
                        {
                            var numDaysLastMonth = DateTime.DaysInMonth(lastYear, lastMonth);
                            startDateLastMonth = new DateTime(lastYear, lastMonth, 1);
                            endDateLastMonth = new DateTime(lastYear, lastMonth, numDaysLastMonth);
                        }

                        columnValue = Expression.Constant(Convert.ChangeType(startDateLastMonth, typeof(DateTime)));
                        Expression greaterLastMonthE = Expression.GreaterThanOrEqual(columnNameProperty, columnValue);

                        columnValue = Expression.Constant(Convert.ChangeType(endDateLastMonth, typeof(DateTime)));
                        Expression lessLastMonthE = Expression.LessThanOrEqual(columnNameProperty, columnValue);

                        var lastMonthExpression = Expression.And(lessLastMonthE, greaterLastMonthE);
                        addExpression(ref combined, lastMonthExpression);
                        break;
                    case "today":
                        DateTime today = DateTime.Now;

                        var startDate = today.Date;
                        var endDate = startDate.AddDays(1);

                        columnValue = Expression.Constant(Convert.ChangeType(startDate, typeof(DateTime)));
                        Expression greaterTodayE = Expression.GreaterThan(columnNameProperty, columnValue);

                        columnValue = Expression.Constant(Convert.ChangeType(endDate, typeof(DateTime)));
                        Expression lessTodayE = Expression.LessThan(columnNameProperty, columnValue);

                        var todayExpression = Expression.And(lessTodayE, greaterTodayE);
                        addExpression(ref combined, todayExpression);
                        break;
                    case "last_n_days":
                        DateTime todayLastNDate = DateTime.Now;
                        int daysLast = int.Parse(value.ToString());
                        var startDateLastnDays = todayLastNDate.Date.AddDays(-daysLast);
                        var endDateLastnDays = todayLastNDate;

                        columnValue = Expression.Constant(Convert.ChangeType(startDateLastnDays, typeof(DateTime)));
                        Expression greaterLastnDayE = Expression.GreaterThan(columnNameProperty, columnValue);

                        columnValue = Expression.Constant(Convert.ChangeType(endDateLastnDays, typeof(DateTime)));
                        Expression lessLastnDayE = Expression.LessThan(columnNameProperty, columnValue);

                        var lastnDaysExpression = Expression.And(lessLastnDayE, greaterLastnDayE);
                        addExpression(ref combined, lastnDaysExpression);
                        break;
                    case "next_n_days":
                        DateTime todayNextNDate = DateTime.Now;
                        int daysNext = int.Parse(value.ToString());
                        var startDateNextnDays = todayNextNDate.Date.AddDays(daysNext);
                        var endDateNextnDays = todayNextNDate;

                        columnValue = Expression.Constant(Convert.ChangeType(startDateNextnDays, typeof(DateTime)));
                        Expression greaterNextnDayE = Expression.GreaterThan(columnNameProperty, columnValue);

                        columnValue = Expression.Constant(Convert.ChangeType(endDateNextnDays, typeof(DateTime)));
                        Expression lessNextnDayE = Expression.LessThan(columnNameProperty, columnValue);

                        var nextnDaysExpression = Expression.And(lessNextnDayE, greaterNextnDayE);
                        addExpression(ref combined, nextnDaysExpression);
                        break;
                    case "this_year":
                        var thisYear = DateTime.Now.Year;

                        var startDateThisYear = new DateTime(thisYear, 1, 1);
                        var endDateThisYear = new DateTime(thisYear, 12, 31);

                        columnValue = Expression.Constant(Convert.ChangeType(startDateThisYear, typeof(DateTime)));
                        Expression greaterThisYearE = Expression.GreaterThan(columnNameProperty, columnValue);

                        columnValue = Expression.Constant(Convert.ChangeType(endDateThisYear, typeof(DateTime)));
                        Expression lessThisYearE = Expression.LessThan(columnNameProperty, columnValue);

                        var thisYearExpression = Expression.And(lessThisYearE, greaterThisYearE);
                        addExpression(ref combined, thisYearExpression);
                        break;
                    case "last_year":
                        var yearLastYear = DateTime.Now.Year - 1;

                        var startDateLastYear = new DateTime(yearLastYear, 1, 1);
                        var endDateLastYear = new DateTime(yearLastYear, 12, 31);

                        columnValue = Expression.Constant(Convert.ChangeType(startDateLastYear, typeof(DateTime)));
                        Expression greaterLastYearE = Expression.GreaterThan(columnNameProperty, columnValue);

                        columnValue = Expression.Constant(Convert.ChangeType(endDateLastYear, typeof(DateTime)));
                        Expression lessLastYearE = Expression.LessThan(columnNameProperty, columnValue);

                        var lastYearExpression = Expression.And(lessLastYearE, greaterLastYearE);
                        addExpression(ref combined, lastYearExpression);
                        break;
                    case "next_year":
                        var yearNextYear = DateTime.Now.Year + 1;

                        var startDateNextYear = new DateTime(yearNextYear, 1, 1);
                        var endDateNextYear = new DateTime(yearNextYear, 12, 31);

                        columnValue = Expression.Constant(Convert.ChangeType(startDateNextYear, typeof(DateTime)));
                        Expression greaterNextYearE = Expression.GreaterThan(columnNameProperty, columnValue);

                        columnValue = Expression.Constant(Convert.ChangeType(endDateNextYear, typeof(DateTime)));
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
    }
}