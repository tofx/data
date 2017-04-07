using TOF.Core.DependencyInjection;
using TOF.Core.Infrastructure;
using TOF.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using TOF.Data.Abstractions;

namespace TOF.Data.Schema
{
    public class DbSchemaActionProvider : IDbSchemaActionProvider
    {
        private string _connectionString = null;
        private IDbModelStrategy _modelStrategy = null;
        private IEnumerable<IDbSchemaStrategy> _schemaStrategies = null;
        private Container _container = null;

        private DbSchemaActionProvider()
        {
            _container = App.ServiceProviders;
        }
        
        public DbSchemaActionProvider(
            IDbModelStrategy ModelStrategy, 
            IEnumerable<IDbSchemaStrategy> SchemaStrategies, 
            string ConnectionString)
        {
            ParameterChecker.NotNull(ModelStrategy);
            ParameterChecker.NotNull(SchemaStrategies);
            ParameterChecker.NotNullOrEmpty(ConnectionString);

            this._modelStrategy = ModelStrategy;
            this._schemaStrategies = SchemaStrategies;
            this._connectionString = ConnectionString;
        }

        public void Alter()
        {
            throw new NotImplementedException();
        }

        public void Create()
        {
            bool schemaExists = SchemaExists();

            if (schemaExists)
                return;

            if (!this._schemaStrategies.Where(s => s.Type == DbSchemaStrategyTypes.Create).Any())
                throw new InvalidOperationException("E_CREATE_TABLE_DATA_DEF_STRATEGY_NOT_FOUND");

            var createTableStrategy =
                this._schemaStrategies.Where(s => s.Type == DbSchemaStrategyTypes.Create).First();

            using (DbClient db = new DbClient(_connectionString))
            { 
                db.Execute(createTableStrategy.GetDbSchemaScript());
            }
        }

        public void Drop()
        {
            bool schemaExists = SchemaExists();

            if (!schemaExists)
                return;

            var dropStrategyQuery =
                _schemaStrategies.Where(s => s.Type == DbSchemaStrategyTypes.Drop);

            if (!dropStrategyQuery.Any())
                throw new InvalidOperationException("E_DROP_TABLE_DATA_DEF_STRATEGY_NOT_FOUND");

            var dropTableStrategy = dropStrategyQuery.First();

            using (DbClient db = new DbClient(this._connectionString))
            {
                db.Execute(dropTableStrategy.GetDbSchemaScript());
            }
        }

        public bool SchemaExists()
        {
            var schemaExistsStrategyQuery =
                _schemaStrategies.Where(s => s.Type == DbSchemaStrategyTypes.LookupSchemaExists);

            if (!schemaExistsStrategyQuery.Any())
                throw new InvalidOperationException("E_LOOKUP_SCHEMA_DATA_DEF_STRATEGY_NOT_FOUND");

            var lookupSchemaStrategy = schemaExistsStrategyQuery.First();
            bool result = false;

            using (DbClient db = new DbClient(_connectionString))
            {
                var queryResult = db.ExecuteQuery(lookupSchemaStrategy.GetDbSchemaScript());
                result = queryResult.Any();
            }

            return result;
        }
    }
}
