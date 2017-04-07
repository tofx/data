using TOF.Core.DependencyInjection;
using TOF.Core.Infrastructure;
using TOF.Data.Abstractions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace TOF.Data
{
    public class DbInspector
    {
        private IDbServiceProvider _provider = null;
        private Container _container = null;

        public DbInspector()
        {
            _container = App.ServiceProviders;
            _provider = _container.Resolve<IDbServiceProvider>();
        }

        public DbInspector(string connectionString)
        {
            _container = App.ServiceProviders;
            _provider = _container.Resolve<IDbServiceProvider>(connectionString);
        }

        public IEnumerable<DbTableDescriptor> EnumerateDbTableDescriptors()
        {
            var strategy = _container.Resolve<IDbTableInspectorStrategy>();

            if (strategy == null)
                throw new ArgumentNullException("EnumerateTableDescriptorStrategyNotFound");

            var reader = _provider.QueryGetReader(strategy.GetDbQueryScript(), strategy.GetDbParameters());
            var dbTypeFinder = _container.Resolve<IDbTypeFinder>();
            List<DbTableDescriptor> tables = new List<DbTableDescriptor>();
            DbTableDescriptor table = null;

            while (reader.Read())
            {
                string tableName = reader.GetString(0);
                string tableDesc = reader.GetString(1);

                // detect table property.
                if (table == null)
                {
                    table = new DbTableDescriptor();
                    table.TableName = tableName;
                    table.LegalEntityName = tableName.Replace(" ", "_");
                    table.Columns = new List<DbColumnDescriptor>();
                    table.Description = tableDesc;
                }
                else if (string.Compare(table.TableName, tableName, StringComparison.InvariantCultureIgnoreCase) != 0)
                {
                    // add table into tables.
                    tables.Add(table);
                    // create new instance for next table.
                    table = new DbTableDescriptor();
                    table.Columns = new List<DbColumnDescriptor>();
                    table.TableName = tableName;
                    table.LegalEntityName = tableName.Replace(" ", "_");
                    table.Description = tableDesc;
                }

                // load column information.
                string columnName = reader.GetString(2);
                int dbTypeNumber = reader.GetInt32(3);
                int length = Convert.ToInt32(reader.GetValue(5));
                bool isNullable = reader.GetBoolean(6);
                bool isIdentity = reader.GetBoolean(7);
                bool isComputed = reader.GetBoolean(8);
                bool isPrimaryKey = Convert.ToBoolean(reader.GetValue(10));
                string columnDesc = reader.GetString(11);
                DbType dbType;

                if (!dbTypeFinder.FindByTypeNumber(dbTypeNumber, false, out dbType))
                    throw new InvalidOperationException("InvalidDbTypeNumber:" + dbTypeNumber.ToString());

                // workaround: ignore column had been added.
                if (!table.Columns.Where(c => c.ColumnName == columnName).Any())
                {
                    table.Columns.Add(new DbColumnDescriptor()
                    {
                        ColumnDbType = dbType,
                        ColumnName = columnName,
                        Description = columnDesc,
                        LegalColumnName = columnName.Replace(" ", "_"),
                        IsComputed = isComputed,
                        IsIdentity = isIdentity,
                        IsPrimaryKey = isPrimaryKey,
                        IsNullable = isNullable,
                        Length = length
                    });
                }
            }

            reader.Close();

            return tables;
        }

        public DbTableDescriptor GetDbTableDescriptor(string TableName)
        {
            var strategy = _container.Resolve<IDbTableInspectorStrategy>();

            if (strategy == null)
                throw new ArgumentNullException("TableDescriptorStrategyNotFound");

            var parameters = strategy.GetDbParameters();
            parameters.Where(p => p.ParameterName == "@tablename").First().Value = TableName;

            var reader = _provider.QueryGetReader(strategy.GetDbQueryScript(), parameters);
            var dbTypeFinder = _container.Resolve<IDbTypeFinder>();
            DbTableDescriptor table = null;

            while (reader.Read())
            {
                string tableName = reader.GetString(0);
                string tableDesc = reader.GetString(1);

                table = new DbTableDescriptor();
                table.TableName = tableName;
                table.LegalEntityName = tableName.Replace(" ", "_");
                table.Columns = new List<DbColumnDescriptor>();
                table.Description = tableDesc;

                // load column information.
                string columnName = reader.GetString(2);
                int dbTypeNumber = reader.GetInt32(3);
                int length = Convert.ToInt32(reader.GetValue(5));
                bool isNullable = reader.GetBoolean(6);
                bool isIdentity = reader.GetBoolean(7);
                bool isComputed = reader.GetBoolean(8);
                bool isPrimaryKey = Convert.ToBoolean(reader.GetValue(10));
                string columnDesc = reader.GetString(11);
                DbType dbType;

                if (!dbTypeFinder.FindByTypeNumber(dbTypeNumber, false, out dbType))
                    throw new InvalidOperationException("InvalidDbTypeNumber:" + dbTypeNumber.ToString());

                // workaround: ignore column had been added.
                if (!table.Columns.Where(c => c.ColumnName == columnName).Any())
                {
                    table.Columns.Add(new DbColumnDescriptor()
                    {
                        ColumnDbType = dbType,
                        ColumnName = columnName,
                        Description = columnDesc,
                        LegalColumnName = columnName.Replace(" ", "_"),
                        IsComputed = isComputed,
                        IsIdentity = isIdentity,
                        IsPrimaryKey = isPrimaryKey,
                        IsNullable = isNullable,
                        Length = length
                    });
                }
            }

            reader.Close();

            return table;
        }
    }
}
