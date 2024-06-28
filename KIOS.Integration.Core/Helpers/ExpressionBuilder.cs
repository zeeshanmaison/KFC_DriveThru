using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace DriveThru.Integration.Core.Helpers
{
    //Run time p lamda exp buillder give property name ,operation, value
    public static class ExpressionBuilder
    {
        private static MethodInfo _containsMethod = typeof(string).GetMethod("Contains");
        private static MethodInfo _inMethod = typeof(List<string>).GetMethod("Contains");
        private static MethodInfo _startsWithMethod = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
        private static MethodInfo _endsWithMethod = typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) });

        private static Expression GetConvertedSource(ParameterExpression sourceParameter, PropertyInfo sourceProperty, TypeCode typeCode)
        {
            MemberExpression sourceExpressionProperty = Expression.Property(sourceParameter, sourceProperty);
            MethodCallExpression changeTypeCall = Expression.Call(typeof(Convert)
                                                    .GetMethod("ChangeType",
                                                            new[] { typeof(object),
                                                            typeof(TypeCode) }),
                                                            sourceExpressionProperty,
                                                            Expression.Constant(typeCode));
            Expression convert = Expression.Convert(changeTypeCall, Type.GetType("System." + typeCode));
            ConditionalExpression convertExpression = Expression.Condition(Expression.Equal(sourceExpressionProperty,
                                                        Expression.Constant(null, sourceProperty.PropertyType)),
                                                        Expression.Default(Type.GetType("System." + typeCode)),
                                                        convert);

            return convertExpression;
        }

        private static Expression<Func<T, bool>> GetExpression<T>(string propertiesTree, string operation, object value)
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(T), "x");
            MemberExpression memberExpression = Expression.Property(parameterExpression, propertiesTree.Split('.')[0]);
            PropertyInfo propertyInfo = typeof(T).GetProperty(propertiesTree.Split('.')[0],
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            ConstantExpression constantExpression = null;

            foreach (string property in propertiesTree.Split('.').Skip(1))
            {
                memberExpression = Expression.Property(memberExpression, property);
                propertyInfo = propertyInfo.PropertyType.GetProperty(property,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            }

            if (operation == "in")
            {
                try
                {
                    if (propertyInfo.PropertyType == typeof(int))
                    {
                        List<int> integers = value.ToString().Split(',').Select(int.Parse).ToList();
                        constantExpression = Expression.Constant(integers);
                        _inMethod = typeof(List<int>).GetMethod("Contains");
                    }
                    else if (propertyInfo.PropertyType == typeof(Guid))
                    {
                        List<Guid> guids = value.ToString().Split(',').Select(Guid.Parse).ToList();
                        constantExpression = Expression.Constant(guids);
                        _inMethod = typeof(List<Guid>).GetMethod("Contains");
                    }
                    else if (propertyInfo.PropertyType == typeof(bool))
                    {
                        List<bool> bools = value.ToString().Split(',').Select(bool.Parse).ToList();
                        constantExpression = Expression.Constant(bools);
                        _inMethod = typeof(List<bool>).GetMethod("Contains");
                    }
                    else if (propertyInfo.PropertyType == typeof(DateTime))
                    {
                        throw new NotImplementedException();
                    }
                    else
                    {
                        List<string> strings = value.ToString().Split(',').ToList();
                        constantExpression = Expression.Constant(strings);
                        _inMethod = typeof(List<string>).GetMethod("Contains");
                    }
                }
                catch (Exception)
                {
                    if (typeof(T).GetProperty(propertiesTree).GetType() != typeof(int))
                        throw new Exception(string.Format("Invalid Format Provided For Property '{0}'.",
                            propertyInfo.Name));
                }
            }
            else if (Guid.TryParse(value.ToString(), out Guid guidValue))
            {
                constantExpression = Expression.Constant(guidValue);
            }
            else if (int.TryParse(value.ToString(), out int integerValue))
            {
                constantExpression = Expression.Constant(integerValue);
            }
            else if (bool.TryParse(value.ToString(), out bool booleanValue))
            {
                constantExpression = Expression.Constant(booleanValue);
            }
            else if (DateTime.TryParse(value.ToString(), out DateTime dateTimeValue))
            {
                constantExpression = Expression.Constant(dateTimeValue);
            }
            else
            {
                constantExpression = Expression.Constant(value.ToString());
            }


            Expression expression = null;

            switch (operation)
            {
                case "in":
                    expression = Expression.Call(constantExpression, _inMethod, memberExpression);
                    break;

                case "=":
                    expression = Expression.Equal(memberExpression, constantExpression);
                    break;

                case ">":
                    expression = Expression.GreaterThan(memberExpression, constantExpression);
                    break;

                case ">=":
                    expression = Expression.GreaterThanOrEqual(memberExpression, constantExpression);
                    break;

                case "<":
                    expression = Expression.LessThan(memberExpression, constantExpression);
                    break;

                case "<=":
                    expression = Expression.LessThanOrEqual(memberExpression, constantExpression);
                    break;

                case "contains":
                    expression = Expression.Call(memberExpression, _containsMethod, constantExpression);
                    break;

                case "startswith":
                    expression = Expression.Call(memberExpression, _startsWithMethod, constantExpression);
                    break;

                case "endswith":
                    expression = Expression.Call(memberExpression, _endsWithMethod, constantExpression);
                    break;
            }

            return Expression.Lambda<Func<T, bool>>(expression, parameterExpression);
        }

        public static Expression<Func<T, bool>> BuildExpression<T>(string column, string operation, object value)
        {
            string[] propertiesTree = column.ToLower().Split('.');
            Type currentPropertyType = typeof(T);

            foreach (var currentProperty in propertiesTree)
            {
                IDictionary<string, string> properties = currentPropertyType.GetProperties()
                    .ToDictionary(x => x.Name.ToLower(), y => y.Name);

                if (!properties.ContainsKey(currentProperty.ToLower()))
                    throw new Exception(string.Format("Invalid Property {0}", column));

                currentPropertyType =
                    currentPropertyType.GetProperty(properties[currentProperty]).PropertyType;
            }

            return GetExpression<T>(column, operation.ToLowerInvariant(), value);
        }
    }
}
