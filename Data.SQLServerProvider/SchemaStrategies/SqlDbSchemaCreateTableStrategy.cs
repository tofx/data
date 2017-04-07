using TOF.Data.Providers.SqlServer.MapType;
using TOF.Core.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TOF.Data.Abstractions;

namespace TOF.Data.Providers.SqlServer.SchemaStrategies
{
    public class SqlDbSchemaCreateTableStrategy : IDbSchemaStrategy
    {
        protected IDbModelStrategy ModelStrategy = null;

        public SqlDbSchemaCreateTableStrategy(IDbModelStrategy modelStrategy)
        {
            ParameterChecker.NotNull(modelStrategy);
            this.ModelStrategy = modelStrategy;
        }

        public DbSchemaStrategyTypes Type { get { return DbSchemaStrategyTypes.Create; } }

        public string GetDbSchemaScript()
        {
            var builder = new StringBuilder();

            builder.AppendLine("CREATE TABLE " + ModelStrategy.DbTableName);
            builder.AppendLine("(");

            var propertyBindings = ModelStrategy.PropertyStrategies;
            var mapDbType = new SqlDbTypeFinder();

            for (int i = 0; i < propertyBindings.Count(); i++)
            {
                var propertyBinding = propertyBindings.ElementAt(i);
                var propDbType = propertyBinding.GetMapDbType();
                var propType = propertyBinding.GetPropertyInfo().PropertyType;
                var propertyInfo = propertyBinding.GetPropertyInfo();
                SqlDbType targetDbType = SqlDbType.Udt;

                builder.Append("\t");
                builder.Append(string.Format("[{0}]", propertyBinding.GetParameterName()));
                builder.Append(" ");

                if (propType == typeof(string))
                {
                    int length = (propertyBinding.GetLength() == null) ? 4000 : propertyBinding.GetLength().Value;
                    bool isMaxLength = propertyBinding.IsMaxLength();

                    if (propDbType == DbType.Guid)
                    {
                        if (!mapDbType.FindByDbType(propDbType.Value, out targetDbType))
                            throw new InvalidOperationException("ERROR_UNKNOWN_SQL_SERVER_DATA_TYPE");

                        builder.Append(string.Format("[{0}]", targetDbType.ToString()));
                    }
                    else
                    {
                        if (propDbType == null)
                        {
                            targetDbType = SqlDbType.NVarChar;
                        }
                        else
                        {
                            if (!mapDbType.FindByDbType(propDbType.Value, out targetDbType))
                                throw new InvalidOperationException("ERROR_UNKNOWN_SQL_SERVER_DATA_TYPE");
                        }

                        if (isMaxLength)
                        {
                            builder.Append(string.Format("[{0}](MAX)", targetDbType.ToString()));
                        }
                        else
                        {
                            builder.Append(string.Format("[{0}]({1})", targetDbType.ToString(), length));
                        }
                    }
                }
                else if (propType == typeof(byte[]))
                {
                    int length = (propertyBinding.GetLength() == null) ? 8000 : propertyBinding.GetLength().Value;
                    bool isMaxLength = propertyBinding.IsMaxLength();

                    if (propDbType == null)
                    {
                        targetDbType = SqlDbType.VarBinary;
                    }
                    else
                    {
                        if (!mapDbType.FindByDbType(propDbType.Value, out targetDbType))
                            throw new InvalidOperationException("ERROR_UNKNOWN_SQL_SERVER_DATA_TYPE");
                    }

                    if (isMaxLength)
                        builder.Append(string.Format("[{0}](MAX)", targetDbType.ToString()));
                    else
                        builder.Append(string.Format("[{0}]({1})", targetDbType.ToString(), length));
                }
                else
                {
                    if (propDbType == null)
                    {
                        if (!mapDbType.FindByClrType(propType, out targetDbType))
                            throw new InvalidOperationException("ERROR_UNKNOWN_SQL_SERVER_DATA_TYPE");
                    }
                    else
                    {
                        if (!mapDbType.FindByDbType(propDbType.Value, out targetDbType))
                            throw new InvalidOperationException("ERROR_UNKNOWN_SQL_SERVER_DATA_TYPE");
                    }

                    builder.Append(string.Format("[{0}]", targetDbType.ToString()));
                }

                builder.Append(" ");
                builder.Append(propertyBinding.IsAllowNull() ? "NULL" : "NOT NULL");

                // handle NOT NULL and AUTO-GENERATED column.
                // auto generated column supports GUID and DATETIME only.
                if ((!propertyBinding.IsAllowNull()) && propertyBinding.IsAutoGenerated())
                {
                    if (propDbType != null)
                    {
                        if (propDbType == DbType.Guid)
                        {
                            builder.Append(string.Format(" CONSTRAINT [DF_{0}_{1}] DEFAULT (NEWID()) ",
                                ModelStrategy.DbTableName, propertyBinding.GetParameterName()));
                        }
                        else if (propDbType == DbType.DateTime)
                        {
                            builder.Append(string.Format(" CONSTRAINT [DF_{0}_{1}] DEFAULT (GETDATE()) ",
                                ModelStrategy.DbTableName, propertyBinding.GetParameterName()));
                        }
                        else if (propDbType == DbType.Int16 || propDbType == DbType.Int32 || propDbType == DbType.Int64)
                        {
                            builder.Append(string.Format(" IDENTITY ",
                                ModelStrategy.DbTableName, propertyBinding.GetParameterName()));
                        }
                        else
                            throw new ArgumentNullException("E_AUTOGEN_MAP_DB_TYPE_NOT_FOUND");
                    }
                    else
                    {
                        if (propType == typeof(Guid))
                        {
                            builder.Append(string.Format(" CONSTRAINT [DF_{0}_{1}] DEFAULT (NEWID()) ",
                                ModelStrategy.DbTableName, propertyBinding.GetParameterName()));
                        }
                        else if (propType == typeof(DateTime))
                        {
                            builder.Append(string.Format(" CONSTRAINT [DF_{0}_{1}] DEFAULT (GETDATE()) ",
                                ModelStrategy.DbTableName, propertyBinding.GetParameterName()));
                        }
                        else if (propType == typeof(short) || propType == typeof(int) || propType == typeof(long))
                        {
                            builder.Append(string.Format(" IDENTITY ",
                                ModelStrategy.DbTableName, propertyBinding.GetParameterName()));
                        }
                        else
                            throw new ArgumentNullException("E_AUTOGEN_MAP_DB_TYPE_NOT_FOUND");
                    }
                }

                if (i < propertyBindings.Count() - 1)
                    builder.Append(",");

                builder.Append(Environment.NewLine);
            }

            // process primary key operation.
            var keyProps = ModelStrategy.PropertyStrategies.Where(p => p.IsKey());

            if (keyProps.Any())
            {
                builder.Append(",");
                List<string> keys = new List<string>();

                foreach (var key in keyProps)
                    keys.Add(key.GetParameterName());

                builder.Append(string.Format(
                    " CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED ({1}) ",
                    ModelStrategy.DbTableName,
                    string.Join(",", keys.ToArray())));
                builder.Append(Environment.NewLine);
            }

            builder.AppendLine(");");

            string sql = builder.ToString();
            return sql;
        }
    }
}
