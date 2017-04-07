using System;
using System.Collections.Generic;
using System.Linq;
using TOF.Core.Infrastructure;
using TOF.Core.Utils;
using TOF.Data.Abstractions;
using System.Data;
using System.Dynamic;
using TOF.Core.DependencyInjection;

namespace TOF.Data
{
    public class DbClient : IDbClient, IDisposable
    {
        private readonly IDbServiceProvider _provider;
        private readonly Container _container;
        
        public DbClient() : this(App.Configuration["connectionStrings/default"])
        {
        }

        public DbClient(string connectionString)
        {
            ParameterChecker.NotNullOrEmpty(connectionString);
            _container = App.ServiceProviders;
            _provider = _container.Resolve<IDbServiceProvider>(new NamedParameter("ConnectionString", connectionString));
        }

        public DbClient(IDbServiceProvider provider)
        {
            ParameterChecker.NotNull(provider);
            _provider = provider;
        }

        public void Dispose()
        {
            if (_provider.IsConnectionOpen())
                _provider.Close();

            _provider.Dispose();
        }

        public void Open()
        {
            _provider.Open();
        }

        public void Close()
        {
            _provider.Close();
        }

        public int Execute(string sql, object dataParams = null)
        {
            var autoClose = !(_provider.IsConnectionOpen());

            if (autoClose)
                _provider.Open();

            var dbParams = GetQueryDataParameters(dataParams);
            var affactedRows = _provider.Execute(sql, dbParams);

            if (autoClose)
                _provider.Close();

            return affactedRows;
        }

        public int Execute(string sql, object[] dataParamsArray)
        {
            var autoClose = !(_provider.IsConnectionOpen());

            if (autoClose)
                _provider.Open();

            var affactedRows = dataParamsArray
                .Select(GetQueryDataParameters)
                .Select(dbParams => _provider.Execute(sql, dbParams)).Sum();

            if (autoClose)
                _provider.Close();

            return affactedRows;
        }

        public IEnumerable<dynamic> ExecuteQuery(string sql, object queryParams = null)
        {
            var autoClose = !(_provider.IsConnectionOpen());

            if (autoClose)
                _provider.Open();

            var dbParams = GetQueryDataParameters(queryParams);
            var reader = _provider.QueryGetReader(sql, dbParams);
            var records = DbModelBindingHelper.GetDataRecords(reader);
            var result = FormatDataRecordToDynamicModel(records);

            reader.Close();

            if (autoClose)
                _provider.Close();

            return result;
        }

        public IEnumerable<T> ExecuteQuery<T>(string sql, object queryParams = null) where T : class, new()
        {
            var autoClose = !(_provider.IsConnectionOpen());

            if (autoClose)
                _provider.Open();

            var dbParams = GetQueryDataParameters(queryParams);
            var reader = _provider.QueryGetReader(sql, dbParams);
            var records = DbModelBindingHelper.GetDataRecords(reader);
            var result = FormatDataRecordToModel<T>(records);

            reader.Close();

            if (autoClose)
                _provider.Close();

            return result;
        }

        public object ExecuteScalar(string sql, object queryParams = null)
        {
            var autoClose = !(_provider.IsConnectionOpen());

            if (autoClose)
                _provider.Open();

            var dbParams = GetQueryDataParameters(queryParams);
            var scalar = _provider.QueryGetScalar(sql, dbParams);

            if (autoClose)
                _provider.Close();

            return scalar;
        }

        public T ExecuteScalar<T>(string sql, object queryParams = null)
        {
            var typeFactory = TypeConverterFactory.GetTypeConverterFactory();
            var converter = typeFactory.GetConvertType<T>();

            if (converter == null)
                throw new InvalidOperationException("Type converter not found.");

            var v = ExecuteScalar(sql, queryParams);
            return (T)converter.Convert(v);
        }

        private static IEnumerable<dynamic> FormatDataRecordToDynamicModel(IEnumerable<IDataRecord> dataRecords)
        {
            var items = new List<dynamic>();

            foreach (var record in dataRecords)
            {
                dynamic item = new ExpandoObject();
                var itemProps = (IDictionary<string, object>)item;

                for (var i = 0; i < record.FieldCount; i++)
                {
                    var column = record.GetName(i);

                    try
                    {
                        itemProps.Add(column,
                            (record.GetOrdinal(column) >= 0) ? record.GetValue(i) : DBNull.Value);
                    }
                    catch (IndexOutOfRangeException)
                    {
                        itemProps.Add(column, DBNull.Value);
                    }
                }

                items.Add(item);
            }

            return items;
        }

        private static IEnumerable<TModel> FormatDataRecordToModel<TModel>(IEnumerable<IDataRecord> dataRecords) where TModel : class, new()
        {
            var models = new List<TModel>();
            var converterFactory = TypeConverterFactory.GetTypeConverterFactory();
            var properties = typeof(TModel).GetProperties();

            foreach (var record in dataRecords)
            {
                var model = new TModel();

                for (var i = 0; i < record.FieldCount; i++)
                {
                    var column = record.GetName(i);
                    var propertyQuery = properties.Where(p => p.Name == column).ToList();

                    if (!propertyQuery.Any())
                        continue;

                    var property = propertyQuery.First();
                    var converter = converterFactory.GetConvertType(property.PropertyType);

                    if (converter == null)
                        continue;

                    property.SetValue(model, converter.Convert(record.GetValue(i)));
                }

                models.Add(model);
            }

            return models;
        }

        private IEnumerable<IDbDataParameter> GetQueryDataParameters(object value)
        {
            var strategyFactory = _container.Resolve<IDbQueryParameterParsingStrategyFactory>();
            return strategyFactory.GetParsingStrategy(value).Parse();
        }
    }
}
