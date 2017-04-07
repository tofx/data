using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TOF.Core.Utils;
using TOF.Core.Utils.TypeExtensions;
using TOF.Data.Abstractions;

namespace TOF.Data.Providers.SqlServer.ParameterParsingStrategies
{
    public class CollectionParameterParsingStrategy : 
        IDbQueryParameterParsingStrategy, IDbQueryCollectionParameterParsingStrategy
    {
        private readonly IEnumerator _collection;
        private IEnumerable<IDbDataParameter> _parameters;

        public CollectionParameterParsingStrategy(IEnumerable collection)
        {
            ParameterChecker.NotNull(collection);
            _collection = collection.GetEnumerator();
        }

        public bool Next()
        {
            return _collection.MoveNext();
        }

        public IEnumerable<IDbDataParameter> Parse()
        {
            var currentValue = _collection.Current;
            var valueType = currentValue.GetType();

            if (_parameters == null)
            {
                valueType = currentValue.GetType();
                _parameters = ParseToDbParameter(valueType);
            }

            BindValueToParameter(ref _parameters, valueType, currentValue);
            return _parameters;
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
