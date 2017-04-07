using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using TOF.Core.Utils;
using TOF.Core.Utils.TypeExtensions;
using TOF.Data.Abstractions;

namespace TOF.Data.Providers.SqlServer.ParameterParsingStrategies
{
    public class SingleParameterParsingStrategy : IDbQueryParameterParsingStrategy
    {
        private readonly object _value;

        public SingleParameterParsingStrategy(object parameter)
        {
            _value = parameter;
        }

        public IEnumerable<IDbDataParameter> Parse()
        {
            if (_value == null)
                return (new NullParameterParsingStrategy()).Parse();

            IDbDataParameter dbParam;

            if (TryParseDbParameter(_value, out dbParam))
                return new List<IDbDataParameter>() { dbParam };

            IEnumerable<IDbDataParameter> dbParams;

            if (TryParseDbParameterCollection(_value, out dbParams))
                return dbParams;

            var parameterType = _value.GetType();
            dbParams = ParseToDbParameter(parameterType);
            BindValueToParameter(ref dbParams, parameterType, _value);

            return dbParams;
        }

        private static bool TryParseDbParameter(
            object value, out IDbDataParameter param)
        {
            try
            {
                param = (IDbDataParameter) value;
                return true;
            }
            catch (InvalidCastException)
            {
                param = null;
                return false;
            }
        }

        private static bool TryParseDbParameterCollection(
            object value, out IEnumerable<IDbDataParameter> parameters)
        {
            parameters = null;
            var collection = value as IEnumerable;

            if (collection == null)
                return false;

            try
            {
                parameters = (IEnumerable<IDbDataParameter>)value;
                return true;
            }
            catch (InvalidCastException)
            {
                return false;
            }
        }

        private static IEnumerable<IDbDataParameter> ParseToDbParameter(Type typeToParse)
        {
            var properties = typeToParse.GetProperties().Where(t => t.DeclaringType == typeToParse);
            var parameters = new List<IDbDataParameter>();

            foreach (var property in properties)
            {
                DbParameterParser parser = new DbParameterParser(property);
                parameters.Add(parser.GetDbParameter());
            }

            return parameters;
        }

        private static void BindValueToParameter(
            ref IEnumerable<IDbDataParameter> parameters, Type typeToParse, object item)
        {
            var properties = typeToParse.GetProperties().Where(t => t.DeclaringType == typeToParse);

            foreach (var property in properties)
            {
                var query = parameters.Where(p =>
                    string.Compare(p.ParameterName.Substring(1), 
                    property.Name, 
                    StringComparison.InvariantCultureIgnoreCase) == 0).ToList();

                if (query.Any())
                {
                    IDbDataParameter parameter = query.First();

                    // adjust parameter size if variable type.
                    switch (parameter.DbType)
                    {
                        case DbType.Binary:

                            if (item.GetType().IsNullable() && property.GetValue(item) == null)
                            {
                                parameter.Value = DBNull.Value;
                            }
                            else
                            {
                                var data = (byte[])property.GetValue(item);
                                parameter.Size = data.Length;
                                parameter.Value = data;
                            }

                            break;

                        case DbType.String:

                            if (item.GetType().IsNullable() && property.GetValue(item) == null)
                            {
                                parameter.Value = DBNull.Value;
                            }
                            else
                            {
                                var dataStr = property.GetValue(item).ToString();
                                parameter.Size = dataStr.Length;
                                parameter.Value = dataStr;
                            }

                            break;

                        default:

                            if (item.GetType().IsNullable() && property.GetValue(item) == null)
                                parameter.Value = DBNull.Value;
                            else
                                parameter.Value = property.GetValue(item);

                            break;
                    }
                }
            }
        }
    }
}
