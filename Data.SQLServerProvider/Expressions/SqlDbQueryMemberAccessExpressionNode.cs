using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using tofx.Data.Providers.SqlServer.Expressions.SqlFunctions;
using tofx.Core.Utils.TypeExtensions;
using tofx.Data.Abstractions;

namespace tofx.Data.Providers.SqlServer.Expressions
{
    public class SqlDbQueryMemberAccessExpressionNode : SqlDbQueryExpressionNode
    {
        private static IEnumerable<IDbQueryFunctionNode> _supportedFunctionNodes = null;
        private bool _requireToRenderAlias = true;

        static SqlDbQueryMemberAccessExpressionNode()
        {
            _supportedFunctionNodes = SqlDbFunctionsLoader.Load();
        }

        public SqlDbQueryMemberAccessExpressionNode(bool requireToRenderAlias = true)
        {
            _requireToRenderAlias = requireToRenderAlias;
        }

        public override string Parse(IDictionary<string, object> parameterDictionary, Expression expressionNode)
        {
            var exp = expressionNode as MemberExpression;
            var expressionBuilder = new StringBuilder();

            if (exp.Member.MemberType == MemberTypes.Property)
            {
                if (exp.Expression == null)
                {
                    var callExpression = new SqlDbQueryCallExpressionNode()
                    {
                        ModelStrategy = ModelStrategy
                    };

                    expressionBuilder.Append(callExpression.Parse(parameterDictionary, exp));
                }
                else
                {
                    var expParser = SqlDbQueryExpressionFactory.GetNodeParser(exp.Expression, ModelStrategy);
                    bool handled = false;

                    foreach (var functionNode in _supportedFunctionNodes)
                    {
                        if (functionNode.CheckForHandle(exp.Member.Name))
                        {
                            ((SqlDbQueryExpressionNode)functionNode).ModelStrategy = ModelStrategy;
                            expressionBuilder.Append(functionNode.Parse(parameterDictionary, exp.Expression));
                            handled = true;
                            break;
                        }
                    }

                    if (!handled)
                    {
                        // get alias type.
                        Type t = exp.Expression.Type;

                        // check this is field (must render as parameter, not field)
                        // solution referenced from 
                        // http://stackoverflow.com/questions/6635678/how-to-recognize-a-lambda-memberexpression-of-type-field-reference
                        bool isQueryParameterProperty =
                            exp.Member is PropertyInfo && !(exp.Expression.NodeType == ExpressionType.Parameter);
                        var propQuery = ModelStrategy.PropertyStrategies.Where(c => c.GetPropertyInfo().Name == exp.Member.Name);

                        if (propQuery.Any())
                        {
                            if (!isQueryParameterProperty)
                            {
                                //if (this._requireToRenderAlias)
                                //{
                                //    expressionBuilder.Append(this.ModelStrategy.GetTableAlias());
                                //    expressionBuilder.Append(".");
                                //}

                                expressionBuilder.Append(propQuery.First().GetParameterName());
                            }
                            else
                            {
                                MemberExpression memberExp = exp.Expression as MemberExpression;
                                string pname = ParameterUtils.CreateNewParameter(ref parameterDictionary);
                                object value = null;
                                
                                if ((memberExp == null && exp.Expression is ConstantExpression))
                                {
                                    value = ExtractValueFromConstantExpression(
                                        (exp.Expression as ConstantExpression), exp.Member.Name, exp.Member.Name);
                                }
                                else if (memberExp.Expression is ConstantExpression)
                                {
                                    value = ExtractValueFromConstantExpression(
                                        (memberExp.Expression as ConstantExpression), memberExp.Member.Name, exp.Member.Name);
                                }
                                else
                                    value = ExecuteLambdaExpression(exp);

                                if (value.GetType() == typeof(DateTime))
                                {
                                    expressionBuilder.Append(string.Format("@{0}", pname));
                                    parameterDictionary[pname] = Convert.ToDateTime(value);
                                }
                                else if (value.GetType() == typeof(Guid))
                                {
                                    expressionBuilder.Append(string.Format("@{0}", pname));
                                    parameterDictionary[pname] = new Guid(value.ToString());
                                }
                                else if (exp.Type == typeof(byte[]) || exp.Type == typeof(byte))
                                {
                                    if (value != null)
                                    {
                                        byte[] byteArrayData = null;

                                        if (exp.Type == typeof(byte[]))
                                            byteArrayData = (byte[])value;
                                        else
                                            byteArrayData = new byte[] { (byte)value };

                                        expressionBuilder.Append(string.Format("@{0}", pname));
                                        parameterDictionary[pname] = byteArrayData;

                                        //expressionBuilder.Append("CONVERT(varbinary(max), ");
                                        //expressionBuilder.Append("0x" + byteArrayData.ToHexString(true));
                                        //expressionBuilder.Append(")");
                                    }
                                }
                                else if (value.GetType().IsValueType)
                                {
                                    expressionBuilder.Append(string.Format("@{0}", pname));
                                    parameterDictionary[pname] = value;
                                }
                                else
                                {
                                    expressionBuilder.Append(string.Format("@{0}", pname));
                                    parameterDictionary[pname] = value.ToString();
                                }
                            }
                        }
                        else
                        {
                            if (!isQueryParameterProperty)
                            {
                                //if (this._requireToRenderAlias)
                                //{
                                //    expressionBuilder.Append(this.ModelStrategy.GetTableAlias());
                                //    expressionBuilder.Append(".");
                                //}

                                expressionBuilder.Append(exp.Member.Name);
                            }
                            else
                            {
                                MemberExpression memberExp = exp.Expression as MemberExpression;
                                string pname = ParameterUtils.CreateNewParameter(ref parameterDictionary);
                                object value = null;

                                if (memberExp.Expression is ConstantExpression)
                                {
                                    var constantExp = memberExp.Expression as ConstantExpression;
                                    value = ExtractValueFromConstantExpression(
                                        constantExp, memberExp.Member.Name, exp.Member.Name);
                                }
                                else
                                    value = ExecuteLambdaExpression(exp);

                                if (value.GetType() == typeof(DateTime))
                                {
                                    expressionBuilder.Append(string.Format("@{0}", pname));
                                    parameterDictionary[pname] = Convert.ToDateTime(value);
                                }
                                else if (value.GetType() == typeof(Guid))
                                {
                                    expressionBuilder.Append(string.Format("@{0}", pname));
                                    parameterDictionary[pname] = new Guid(value.ToString());
                                }
                                else if (exp.Type == typeof(byte[]) || exp.Type == typeof(byte))
                                {
                                    if (value != null)
                                    {
                                        byte[] byteArrayData = null;

                                        if (exp.Type == typeof(byte[]))
                                            byteArrayData = (byte[])value;
                                        else
                                            byteArrayData = new byte[] { (byte)value };

                                        expressionBuilder.Append(string.Format("@{0}", pname));
                                        parameterDictionary[pname] = byteArrayData;

                                        //expressionBuilder.Append("CONVERT(varbinary(max), ");
                                        //expressionBuilder.Append("0x" + byteArrayData.ToHexString(true));
                                        //expressionBuilder.Append(")");
                                    }
                                }
                                else if (exp.Type.IsArray)
                                {
                                    object[] array = value as object[];
                                    bool isFirst = true;

                                    expressionBuilder.Append("(");

                                    foreach (object arrayValue in array)
                                    {
                                        string arrayPName = ParameterUtils.CreateNewParameter(ref parameterDictionary);

                                        if (!isFirst)
                                        {
                                            expressionBuilder.Append(",");
                                            expressionBuilder.Append(string.Format("@{0}", arrayPName));

                                            if (arrayValue is string)
                                            {
                                                parameterDictionary[arrayPName] = arrayValue.ToString();
                                            }
                                            else
                                            {
                                                parameterDictionary[arrayPName] = arrayValue;
                                            }
                                        }
                                        else
                                        {
                                            expressionBuilder.Append(string.Format("@{0}", arrayPName));

                                            if (arrayValue is string)
                                            {
                                                parameterDictionary[arrayPName] = arrayValue.ToString();
                                            }
                                            else
                                            {
                                                parameterDictionary[arrayPName] = arrayValue;
                                            }

                                            isFirst = false;
                                        }
                                    }

                                    expressionBuilder.Append(")");
                                }
                                else if (value.GetType().IsValueType)
                                {
                                    expressionBuilder.Append(string.Format("@{0}", pname));
                                    parameterDictionary[pname] = value;
                                }
                                else
                                {
                                    expressionBuilder.Append(string.Format("@{0}", pname));
                                    parameterDictionary[pname] = value.ToString();
                                }
                            }
                        }
                    }
                }
            }
            else if (exp.Member.MemberType == MemberTypes.Field)
            {
                object v = Expression.Lambda(exp).Compile().DynamicInvoke();
                string pname = ParameterUtils.CreateNewParameter(ref parameterDictionary);

                if (exp.Type == typeof(string))
                {
                    if (v != null)
                    {
                        expressionBuilder.Append(string.Format("@{0}", pname));
                        parameterDictionary[pname] = v.ToString();

                    }
                    else
                        parameterDictionary[pname] = DBNull.Value;
                }
                else if (exp.Type == typeof(DateTime))
                {
                    if (v != null)
                    {
                        expressionBuilder.Append(string.Format("@{0}", pname));
                        parameterDictionary[pname] = Convert.ToDateTime(v);
                    }
                }
                else if (exp.Type == typeof(byte[]) || exp.Type == typeof(byte))
                {                    
                    if (v != null)
                    {
                        byte[] byteArrayData = null;

                        if (exp.Type == typeof(byte[]))
                            byteArrayData = (byte[])v;
                        else
                            byteArrayData = new byte[] { (byte)v };

                        expressionBuilder.Append(string.Format("@{0}", pname));
                        parameterDictionary[pname] = Convert.ToDateTime(v);

                        //expressionBuilder.Append("CONVERT(varbinary(max), ");
                        //expressionBuilder.Append("0x" + byteArrayData.ToHexString(true));
                        //expressionBuilder.Append(")");
                    }
                }
                else if (exp.Type.IsArray)
                {
                    object[] array = v as object[];
                    bool isFirst = true;

                    expressionBuilder.Append("(");

                    foreach (object arrayValue in array)
                    {
                        string arrayPName = ParameterUtils.CreateNewParameter(ref parameterDictionary);

                        if (!isFirst)
                        {
                            expressionBuilder.Append(",");
                            expressionBuilder.Append(string.Format("@{0}", arrayPName));

                            if (arrayValue is string)
                            {
                                parameterDictionary[arrayPName] = arrayValue.ToString();
                            }
                            else
                            {
                                parameterDictionary[arrayPName] = arrayValue;
                            }
                        }
                        else
                        {
                            expressionBuilder.Append(string.Format("@{0}", arrayPName));

                            if (arrayValue is string)
                            {
                                parameterDictionary[arrayPName] = arrayValue.ToString();
                            }
                            else
                            {
                                parameterDictionary[arrayPName] = arrayValue;
                            }

                            isFirst = false;
                        }
                    }

                    expressionBuilder.Append(")");
                }
                else
                    expressionBuilder.Append(v.ToString());
            }
            else
            {
                var propQuery = ModelStrategy.PropertyStrategies.Where(c => c.GetPropertyInfo().Name == exp.Member.Name);

                if (propQuery.Any())
                {
                    //if (this._requireToRenderAlias)
                    //{
                    //    expressionBuilder.Append(this.ModelStrategy.GetTableAlias());
                    //    expressionBuilder.Append(".");
                    //}

                    expressionBuilder.Append(propQuery.First().GetParameterName());
                }
                else
                {
                    //if (this._requireToRenderAlias)
                    //{
                    //    expressionBuilder.Append(this.ModelStrategy.GetTableAlias());
                    //    expressionBuilder.Append(".");
                    //}

                    expressionBuilder.Append(exp.Member.Name);
                }
            }

            return expressionBuilder.ToString();
        }

        private object ExecuteLambdaExpression(MemberExpression lambdaExpression)
        {
            var expressionBuilder = new StringBuilder();
            string expressionType = lambdaExpression.GetType().Name;
            object pv = null;

            // compile expression and invoke it to get value.
            // solution found:
            // http://stackoverflow.com/questions/3457558/getting-values-from-expressiontrees

                object v = Expression.Lambda(lambdaExpression.Expression).Compile().DynamicInvoke();
                // use reflection to get mapped property value.
                PropertyInfo expMemberProperty = lambdaExpression.Member as PropertyInfo;

                if (expMemberProperty != null)
                    pv = expMemberProperty.GetValue(v, null);
                else
                    pv = v;
            
            return pv;
        }

        private object ExtractValueFromConstantExpression(
            ConstantExpression expression, string memberName, string propertyName)
        {
            object value = Convert.ChangeType(expression.Value, expression.Type);

            if (value.GetType().GetField(memberName) != null)
                value = value.GetType().GetField(memberName).GetValue(value);
            else
                value = value.GetType().GetProperty(memberName).GetValue(value);

            // if expression is Enumerate, skip it.
            if (!value.GetType().IsEnum && !value.GetType().IsValueType)
            {
                value = value.GetType().GetProperty(propertyName).GetValue(value);
            }
            else if (value.GetType().IsEnum)
            {
                return (int)value;
            }

            return value;
        }
    }
}
