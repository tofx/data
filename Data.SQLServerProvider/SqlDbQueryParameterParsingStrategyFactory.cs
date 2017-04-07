using System;
using System.Collections;
using tofx.Data.Abstractions;
using tofx.Data.Providers.SqlServer.ParameterParsingStrategies;

namespace tofx.Data.Providers.SqlServer
{
    public class SqlDbQueryParameterParsingStrategyFactory : IDbQueryParameterParsingStrategyFactory
    {
        public IDbQueryParameterParsingStrategy GetParsingStrategy(object value = null)
        {
            if (value == null)
                return new NullParameterParsingStrategy();

            IEnumerable collection;

            if (TryConvertCollection(value, out collection))
                return new CollectionParameterParsingStrategy(collection);

            return new SingleParameterParsingStrategy(value);
        }

        private static bool TryConvertCollection(object value, out IEnumerable collection)
        {
            collection = null;

            try
            {
                collection = (IEnumerable) value;
                return true;
            }
            catch (InvalidCastException)
            {
                return false;
            }
        }
    }
}
