using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;
using tofx.Core.DependencyInjection;
using tofx.Core.Infrastructure;
using tofx.Data.Abstractions;

namespace tofx.Data
{
    public class DbStoredProcedureStrategy<TModel> : IDbStoredProcedureStrategy<TModel> where TModel: class, new()
    {
        private readonly string _procedureName;
        private readonly IDictionary<string, IDbModelPropertyStrategy> _modelPropertyStrategies;
        private readonly Container _container;

        private DbStoredProcedureStrategy()
        {
            _container = App.ServiceProviders;
        }

        public DbStoredProcedureStrategy(
            string procedureName, IDictionary<string, IDbModelPropertyStrategy> modelPropertyParamsBindings = null) : this()
        {
            _procedureName = procedureName;

            _modelPropertyStrategies = (modelPropertyParamsBindings == null)
                ? new Dictionary<string, IDbModelPropertyStrategy>()
                : new Dictionary<string, IDbModelPropertyStrategy>(modelPropertyParamsBindings);
        }

        public string GetDbProcName()
        {
            return _procedureName;
        }

        public IEnumerable<IDbDataParameter> GetDbParameters()
        {
            foreach (var propBinding in _modelPropertyStrategies)
            {
                var param = new SqlParameter()
                {
                    ParameterName = propBinding.Key
                };

                if (propBinding.Value.GetMapDbType() != null)
                    param.DbType = propBinding.Value.GetMapDbType().Value;
                if (propBinding.Value.GetLength() != null)
                    param.Size = propBinding.Value.GetLength().Value;

                yield return param;
            }
        }

        public IDictionary<string, IDbModelPropertyStrategy> GetModelPropertyBindings()
        {
            return _modelPropertyStrategies;
        }

        private void AddOrReplaceParameterBindingInfo(string parameterName, IDbModelPropertyStrategy parameterBindingInfo)
        {
            if (_modelPropertyStrategies.ContainsKey(parameterName))
                _modelPropertyStrategies[parameterName] = parameterBindingInfo;
            else
                _modelPropertyStrategies.Add(parameterName, parameterBindingInfo);
        }

        public IDbModelPropertyStrategy Property(string parameterName, Expression<Func<TModel, object>> propertySpecifier)
        {
            var parserProvider = _container.Resolve<IDbQueryExpressionMemberNameParserProvider>();

            if (parserProvider == null)
                throw new InvalidOperationException("DbServiceProvider can't find query expression member name parser provider.");

            var propertyName = ExtractPropertyInfo(propertySpecifier);

            var prop = typeof(TModel).GetProperty(propertyName);

            if (prop == null)
                throw new ArgumentException("ERROR_PROPERTY_NOT_FOUND");

            IDbModelPropertyStrategy propertyBindingInfo = new DbModelPropertyStrategy(prop);
            AddOrReplaceParameterBindingInfo(prop.Name, propertyBindingInfo);
            return propertyBindingInfo;
        }

        private string ExtractPropertyInfo(Expression<Func<TModel, object>> propertySpecifier)
        {
            var expression = propertySpecifier.Body;
            string name = null;

            switch (expression.NodeType)
            {
                case ExpressionType.Constant:

                    if (!(expression.Type == typeof(string)))
                        throw new InvalidOperationException("ERROR_MAPPING_NAME_MUST_BE_STRING");

                    name = ((ConstantExpression)expression).Value.ToString();

                    break;

                case ExpressionType.Convert:
                    expression = ((UnaryExpression)expression).Operand;
                    break;
            }

            if (expression is MemberExpression)
                name = ((MemberExpression)expression).Member.Name;

            return name;
        }
    }
}
