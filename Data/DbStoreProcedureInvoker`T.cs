using tofx.Core.Utils;
using tofx.Core.Utils.TypeConverters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using tofx.Data.Abstractions;

namespace tofx.Data
{
    public class DbStoreProcdureInvoker<TModel> : IDbStoredProcedureInvoker<TModel> where TModel : class, new()
    {
        private readonly IDbStoredProcedureStrategy _strategy;
        private readonly IDbServiceProvider _serviceProvider;

        public DbStoreProcdureInvoker(IDbStoredProcedureStrategy storedProcStrategy, IDbServiceProvider dbServiceProvider)
        {
            _strategy = storedProcStrategy;
            _serviceProvider = dbServiceProvider;
        }

        public void Invoke(TModel model)
        {
            var parameters = BindingModelPropertyToParameter(
                model,
                _strategy.GetModelPropertyBindings(),
                _strategy.GetDbParameters()).ToList();

            try
            {
                _serviceProvider.Open();

                if (_serviceProvider.RunStoredProc(_strategy.GetDbProcName(), parameters) == 0)
                {
                    throw new DbOperationException(
                        "ERROR_SQL_PROCEDURE_EXECUTION_FAILED",
                        _strategy.GetDbProcName(),
                        parameters);
                }
            }
            catch (Exception exception)
            {
                throw new DbOperationException(
                    "ERROR_SQL_PROCEDURE_EXECUTION_FAILED",
                    exception,
                    _strategy.GetDbProcName(),
                    parameters);
            }
            finally
            {
                _serviceProvider.Close();
            }
        }

        public void Invoke(IEnumerable<TModel> models)
        {
            var transactionProvider = (IDbTransactionProvider)_serviceProvider;
            List<IDbDataParameter> parameters = null;

            try
            {
                _serviceProvider.Open();

                transactionProvider.BeginTransaction();

                foreach (var model in models)
                {
                    parameters = BindingModelPropertyToParameter(
                        model,
                        _strategy.GetModelPropertyBindings(),
                        _strategy.GetDbParameters()).ToList();

                    if (_serviceProvider.RunStoredProc(_strategy.GetDbProcName(), parameters) != 0) continue;

                    transactionProvider.Rollback();

                    throw new DbOperationException(
                        "ERROR_SQL_PROCEDURE_EXECUTION_FAILED",
                        _strategy.GetDbProcName(),
                        parameters);
                }

                transactionProvider.Commit();
            }
            catch (Exception exception)
            {
                transactionProvider.Rollback();

                throw new DbOperationException(
                    "ERROR_SQL_PROCEDURE_EXECUTION_FAILED",
                    exception,
                    _strategy.GetDbProcName(),
                    parameters);
            }
            finally
            {
                _serviceProvider.Close();
            }
        }

        public IEnumerable<dynamic> InvokeGet(TModel model)
        {
            var parameters = BindingModelPropertyToParameter(
                model,
                _strategy.GetModelPropertyBindings(),
                _strategy.GetDbParameters()).ToList();

            try
            {
                _serviceProvider.Open();
                var reader = _serviceProvider.RunStoredProcGetReader(_strategy.GetDbProcName(), parameters);

                var items = new List<dynamic>();
                var records = DbModelBindingHelper.GetDataRecords(reader);

                foreach (var record in records)
                {
                    dynamic item = new ExpandoObject();
                    var itemProps = (IDictionary<string, object>)item;

                    for (var i = 0; i < record.FieldCount; i++)
                    {
                        try
                        {
                            itemProps.Add(record.GetName(i).Trim(), reader.GetValue(i));
                        }
                        catch (IndexOutOfRangeException)
                        {
                            itemProps.Add(record.GetName(i).Trim(), DBNull.Value);
                        }
                    }

                    items.Add(item);
                }

                reader.Close();
                return items;
            }
            catch (Exception exception)
            {
                throw new DbOperationException(
                    "ERROR_SQL_PROCEDURE_EXECUTION_FAILED",
                    exception,
                    _strategy.GetDbProcName(),
                    parameters);
            }
            finally
            {
                _serviceProvider.Close();
            }
        }

        private static IEnumerable<IDbDataParameter> BindingModelPropertyToParameter(
            TModel model, IDictionary<string, IDbModelPropertyStrategy> modelPropertyBindings, IEnumerable<IDbDataParameter> dbParams)
        {
            var parameters = new List<IDbDataParameter>(dbParams);
            var converterFactory = TypeConverterFactory.GetTypeConverterFactory();

            foreach (var parameter in parameters)
            {
                var propBindingQuery = modelPropertyBindings.Where(c => c.Key == parameter.ParameterName).ToList();

                if (!propBindingQuery.Any()) continue;

                var propertyBindingInfo = propBindingQuery.First();
                var modelProperty = propertyBindingInfo.Value.GetPropertyInfo();
                var value = modelProperty.GetValue(model, null);

                if (value != null)
                {
                    var converter = converterFactory.GetConvertType(modelProperty.PropertyType);

                    if (converter != null)
                    {
                        if (converter is EnumConverter)
                            parameter.Value =
                                (converter as EnumConverter).Convert(modelProperty.PropertyType, value);
                        else
                            parameter.Value = converter.Convert(value);
                    }
                    else
                        parameter.Value = value;
                }
                else
                {
                    if (propertyBindingInfo.Value.IsAllowNull())
                        parameter.Value = DBNull.Value;
                    else
                    {
                        if (propertyBindingInfo.Value.GetPropertyInfo().PropertyType.IsValueType)
                        {
                            parameter.Value = Activator.CreateInstance(
                                propertyBindingInfo.Value.GetPropertyInfo().PropertyType);
                        }
                        else
                            parameter.Value = string.Empty;
                    }
                }
            }

            return parameters;
        }
    }
}
