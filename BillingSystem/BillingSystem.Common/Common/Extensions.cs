using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace BillingSystem.Common.Common
{
    public static class ExtensionMethods
    {
        public const string IdColumn = "Id";

        public static DataTable CreateCommonDatatable()
        {
            var dt = new DataTable();
            dt.Columns.Add(new DataColumn("Id", typeof(int)));
            dt.Columns.Add(new DataColumn("Value1", typeof(string)));
            dt.Columns.Add(new DataColumn("Value2", typeof(string)));
            dt.Columns.Add(new DataColumn("Value3", typeof(string)));
            dt.Columns.Add(new DataColumn("Value4", typeof(string)));
            dt.Columns.Add(new DataColumn("Value5", typeof(string)));
            dt.Columns.Add(new DataColumn("Value6", typeof(string)));
            dt.Columns.Add(new DataColumn("Value7", typeof(string)));

            return dt;
        }

        public static DataTable ToDataTable<T>(this List<T> items)
        {
            var dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            var Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (var item in items)
            {
                var values = new object[Props.Length];
                for (var i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName, bool isAscending)
        {
            var type = typeof(T);
            var methodName = isAscending ? "OrderBy" : "OrderByDescending";
            var property = (type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance) ??
                            type.GetProperty(propertyName + IdColumn, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)) ??
                           type.GetProperty(IdColumn, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            //Property Sorting for the external Table Columns, Below is the temprory Fix
            if (property != null)
            {
                if (property.PropertyType.IsClass)
                {
                    if (property.PropertyType.FullName != "System.String")
                    {
                        property = type.GetProperty(IdColumn,
                                                    BindingFlags.IgnoreCase | BindingFlags.Public |
                                                    BindingFlags.Instance);
                    }
                }
            }
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExp = Expression.Lambda(propertyAccess, parameter);
            var resultExp = Expression.Call(typeof(Queryable), methodName, new[] { type, property.PropertyType }, source.Expression, Expression.Quote(orderByExp));
            return source.Provider.CreateQuery<T>(resultExp);
        }

        public static IQueryable<T> Where<T>(this IQueryable<T> query,
           string column, object value)
        {
            return query.Where(column, value, WhereOperation.Contains);
        }

        public static IQueryable<T> Where<T>(this IQueryable<T> query,
            string column, object value, WhereOperation operation)
        {
            if (string.IsNullOrEmpty(column)) { return query; }

            var parameter = Expression.Parameter(query.ElementType, "p");

            var memberAccess = column.Split('.').Aggregate<string, MemberExpression>(null, (current, property) => Expression.Property(current ?? (parameter as Expression), property));

            //change param value type necessary to getting bool from string
            ConstantExpression filter = null;
            try
            {
                filter = Expression.Constant
                (
                    Convert.ChangeType(value, memberAccess.Type, CultureInfo.InvariantCulture)
                 );
            }
            catch
            {
                return query;
            }

            //switch operation
            LambdaExpression lambda = null;
            try
            {
                Expression condition = null;
                switch (operation)
                {
                    case WhereOperation.Equal:
                        condition = Expression.Equal(memberAccess, filter);
                        lambda = Expression.Lambda(condition, parameter);
                        break;
                    case WhereOperation.NotEqual:
                        condition = Expression.NotEqual(memberAccess, filter);
                        lambda = Expression.Lambda(condition, parameter);
                        break;
                    case WhereOperation.Contains:
                        condition = Expression.Call(memberAccess,
                            typeof(string).GetMethod("Contains"),
                            Expression.Constant(value));
                        lambda = Expression.Lambda(condition, parameter);
                        break;
                }
            }
            catch
            {
                return query;
            }

            var result = Expression.Call(
                   typeof(Queryable), "Where",
                   new[] { query.ElementType },
                   query.Expression,
                   lambda);

            return query.Provider.CreateQuery<T>(result);
        }

        /// <summary>
        /// Searches in all string properties for the specifed search key.  
        /// It is also able to search for several words. If the searchKey is for example 'Chandru BK' then  
        /// all records which contain either 'Chandru' or 'BK' in some string property are returned.   
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IQueryable<T> FullTextSearch<T>(this IQueryable<T> query, string value)
        {
            return FullTextSearch<T>(query, value, false);
        }


        /// <summary>  
        /// Searches in all string properties for the specifed search key.  
        /// It is also able to search for several words. If the searchKey is for example 'John Travolta' then  
        /// with exactMatch set to false all records which contain either 'John' or 'Travolta' in some string property  
        /// are returned.  
        /// </summary>  
        /// <typeparam name="T"></typeparam>  
        /// <param name="queryable"></param>  
        /// <param name="searchKey"></param>  
        /// <param name="exactMatch">Specifies if only the whole word or every single word should be searched.</param>  
        /// <returns></returns>
        public static IQueryable<T> FullTextSearch<T>(this IQueryable<T> queryable, string searchKey, bool exactMatch)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "c");
            MethodInfo containsMethod = typeof(string).GetMethod("Contains", new Type[] { typeof(string) });
            //MethodInfo toStringMethod = typeof (object).GetMethod("ToString", new Type[] {});
            var publicProperties =
                typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(
                    p => p.PropertyType == typeof(string));
            Expression orExpressions = null;
            string[] searchKeyParts;
            if (exactMatch)
            {
                searchKeyParts = new[] { searchKey };
            }
            else
            {
                searchKeyParts = searchKey.Split(' ');
            }
            foreach (var property in publicProperties)
            {
                Expression nameProperty = Expression.Property(parameter, property);
                foreach (var searchKeyPart in searchKeyParts)
                {
                    Expression searchKeyExpression = Expression.Constant(searchKeyPart);
                    Expression callContainsMethod = Expression.Call(nameProperty, containsMethod, searchKeyExpression);
                    if (orExpressions == null)
                    {
                        orExpressions = callContainsMethod;
                    }
                    else
                    {
                        orExpressions = Expression.Or(orExpressions, callContainsMethod);
                    }
                }
            }
            MethodCallExpression whereCallExpression = Expression.Call(typeof(Queryable), "Where",
                                                                       new Type[] { queryable.ElementType },
                                                                       queryable.Expression,
                                                                       Expression.Lambda<Func<T, bool>>(orExpressions,
                                                                                                        new ParameterExpression
                                                                                                            []
                                                                                                            {parameter}));
            return queryable.Provider.CreateQuery<T>(whereCallExpression);
        }


        public static MultiResultSetReader MultiResultSetSqlQuery(this DbContext context, string storedProcedure, bool isCompiled = true, params SqlParameter[] parameters)
        {
            var query = storedProcedure;
            if (!isCompiled)
            {
                var paramsString = parameters.Any(a => a.ParameterName.Contains("@")) ? string.Join(",", parameters.Select(p => p.ParameterName)) : string.Join(",", parameters.Select(p => $"@{p.ParameterName}"));
                query = $"Exec {storedProcedure} {paramsString}";
            }
            return new MultiResultSetReader(context, query, parameters);
        }

        /// <summary>
        /// Replaces a substring within a string with another substring with optional case sensitivity turned off.
        /// </summary>
        /// <param name="originalString">String to do replacements on</param>
        /// <param name="oldValue">The string to find</param>
        /// <param name="newValue">The string to replace found string wiht</param>
        /// <param name="caseInsensitive">If true case insensitive search is performed</param>
        /// <returns>updated string or original string if no matches</returns>
        public static string ReplaceEx(this string originalString, string oldValue, string newValue, bool caseInsensitive)
        {
            var at1 = 0;
            while (true)
            {
                at1 = caseInsensitive ? originalString.IndexOf(oldValue, at1, originalString.Length - at1, StringComparison.OrdinalIgnoreCase) : originalString.IndexOf(oldValue, at1);
                if (at1 == -1) return originalString;
                originalString = originalString.Substring(0, at1) + newValue + originalString.Substring(at1 + oldValue.Length);
                at1 += newValue.Length;
            }
        }

        /// <summary>
        /// Get the Typed List which is required for the MVC Grid.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<Dictionary<string, Object>> GetTypedList(IEnumerable list)
        {
            var rows = new List<Dictionary<string, Object>>();
            if (list == null) return rows;
            foreach (var rec in list)
            {
                var row = new Dictionary<string, Object>();
                var oProps = rec.GetType().GetProperties();
                foreach (var pi in oProps)
                {
                    var colType = pi.PropertyType;
                    if (colType == typeof(Int32) || colType == typeof(Int32?) || colType == typeof(String))
                    {
                        if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }
                        row.Add(pi.Name, pi.GetValue(rec, null) ?? DBNull.Value);
                    }
                    else
                    {
                        continue;
                    }
                }
                rows.Add(row);
            }
            return rows;
        }

        public static List<Dictionary<string, Object>> GetTypedList(DataTable dataTable)
        {
            return (from DataRow dr in dataTable.Rows select dataTable.Columns.Cast<DataColumn>().ToDictionary(col => col.ColumnName, col => dr[col])).ToList();
        }
    }
}
