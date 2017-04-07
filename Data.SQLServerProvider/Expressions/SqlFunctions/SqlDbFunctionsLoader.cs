using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TOF.Data.Abstractions;

namespace TOF.Data.Providers.SqlServer.Expressions.SqlFunctions
{
    public class SqlDbFunctionsLoader
    {
        private static IEnumerable<IDbQueryFunctionNode> _loadedFunctions = null;

        public static IEnumerable<IDbQueryFunctionNode> Load()
        {
            if (_loadedFunctions == null)
            {
                List<IDbQueryFunctionNode> functions = new List<IDbQueryFunctionNode>();
                Assembly thisAssembly = typeof(SqlDbFunctionsLoader).Assembly;
                var functionTypes = thisAssembly.GetTypes().Where(
                    c => c.FindInterfaces(new TypeFilter((t, o) => t == typeof(IDbQueryFunctionNode)), null).Any());

                foreach (var type in functionTypes)
                    functions.Add(Activator.CreateInstance(type) as IDbQueryFunctionNode);

                _loadedFunctions = functions;
            }

            return _loadedFunctions;
        }
    }
}
