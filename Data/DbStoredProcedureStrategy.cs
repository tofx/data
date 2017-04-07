using TOF.Core.DependencyInjection;
using TOF.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Reflection;
using TOF.Data.Abstractions;

namespace TOF.Data
{
    public class DbStoredProcedureStrategy : IDbStoredProcedureStrategy
    {
        private readonly Container _container;
        private readonly Type _modelType;
        private readonly string _procedureName;
        private readonly IDictionary<string, IDbModelPropertyStrategy> _dbProcParamsBindingInfo;

        private DbStoredProcedureStrategy()
        {
            _container = App.ServiceProviders;
        }

        public DbStoredProcedureStrategy(
            Type modelType, string procedureName, IDictionary<string, IDbModelPropertyStrategy> procedurePropertyBindings = null)
            : this()
        {
            _modelType = modelType;
            _procedureName = procedureName;

            _dbProcParamsBindingInfo = (procedurePropertyBindings == null)
              ? new Dictionary<string, IDbModelPropertyStrategy>()
              : new Dictionary<string, IDbModelPropertyStrategy>(procedurePropertyBindings);
        }

        public IDbModelPropertyStrategy Parameter<T>(string parameterName, Expression<Func<T, object>> propertySpecifier)
        {
            if (typeof(T) != _modelType)
                throw new ArgumentException("ERROR_MODEL_TYPE_MISMATCH");

            var parserProvider = _container.Resolve<IDbQueryExpressionMemberNameParserProvider>();

            if (parserProvider == null)
                throw new InvalidOperationException("DbServiceProvider can't find query expression member name parser provider.");

            var propertyName = ExtractPropertyInfo(propertySpecifier);

            var prop = typeof(T).GetProperty(propertyName);

            if (prop == null)
                throw new ArgumentException("ERROR_PROPERTY_NOT_FOUND");

            IDbModelPropertyStrategy propertyBindingInfo = new DbModelPropertyStrategy(prop);
            AddOrReplaceParameterBindingInfo(parameterName, propertyBindingInfo);
            return propertyBindingInfo;
        }

        public string GetDbProcName()
        {
            return _procedureName;
        }

        public IEnumerable<IDbDataParameter> GetDbParameters()
        {
            foreach (var propBinding in _dbProcParamsBindingInfo)
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
            return _dbProcParamsBindingInfo;
        }

        private void AddOrReplaceParameterBindingInfo(string parameterName, IDbModelPropertyStrategy modelPropertyBindingInfo)
        {
            if (_dbProcParamsBindingInfo.ContainsKey(parameterName))
                _dbProcParamsBindingInfo[parameterName] = modelPropertyBindingInfo;
            else
                _dbProcParamsBindingInfo.Add(parameterName, modelPropertyBindingInfo);
        }

        private string ExtractPropertyInfo<T>(Expression<Func<T, object>> propertySpecifier)
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
