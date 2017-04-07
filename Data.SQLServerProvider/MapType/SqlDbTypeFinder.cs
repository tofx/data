using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml;
using TOF.Data.Abstractions;

namespace TOF.Data.Providers.SqlServer.MapType
{
    public class SqlDbTypeFinder : IDbTypeFinder
    {
        private static List<SqlDbTypeMap> _sDbTypeMaps = new List<SqlDbTypeMap>();

        static SqlDbTypeFinder()
        {
            _sDbTypeMaps.Add(new SqlDbTypeMap()
            {
                DbTypeNumber = 34,
                SqlDbType = SqlDbType.Image,
                DbType = DbType.Binary,
                ClrType = typeof(byte[]),
                Default = false
            });
            _sDbTypeMaps.Add(new SqlDbTypeMap()
            {
                DbTypeNumber = 35,
                SqlDbType = SqlDbType.Text,
                DbType = DbType.String,
                ClrType = typeof(string),
                Default = false
            });
            _sDbTypeMaps.Add(new SqlDbTypeMap()
            {
                DbTypeNumber = 36,
                SqlDbType = SqlDbType.UniqueIdentifier,
                DbType = DbType.Guid,
                ClrType = typeof(Guid),
                Default = true
            });
            _sDbTypeMaps.Add(new SqlDbTypeMap()
            {
                DbTypeNumber = 40,
                SqlDbType = SqlDbType.Date,
                DbType = DbType.Date,
                ClrType = typeof(DateTime),
                Default = false
            });
            _sDbTypeMaps.Add(new SqlDbTypeMap()
            {
                DbTypeNumber = 41,
                SqlDbType = SqlDbType.Time,
                DbType = DbType.Time,
                ClrType = typeof(TimeSpan),
                Default = false
            });
            _sDbTypeMaps.Add(new SqlDbTypeMap()
            {
                DbTypeNumber = 42,
                SqlDbType = SqlDbType.DateTime2,
                DbType = DbType.DateTime2,
                ClrType = typeof(DateTime),
                Default = false
            });
            _sDbTypeMaps.Add(new SqlDbTypeMap()
            {
                DbTypeNumber = 43,
                SqlDbType = SqlDbType.DateTimeOffset,
                DbType = DbType.DateTimeOffset,
                ClrType = typeof(DateTimeOffset),
                Default = true
            });
            _sDbTypeMaps.Add(new SqlDbTypeMap()
            {
                DbTypeNumber = 48,
                SqlDbType = SqlDbType.TinyInt,
                DbType = DbType.Byte,
                ClrType = typeof(byte),
                Default = true
            });
            _sDbTypeMaps.Add(new SqlDbTypeMap()
            {
                DbTypeNumber = 52,
                SqlDbType = SqlDbType.SmallInt,
                DbType = DbType.Int16,
                ClrType = typeof(short),
                Default = true
            });
            _sDbTypeMaps.Add(new SqlDbTypeMap()
            {
                DbTypeNumber = 56,
                SqlDbType = SqlDbType.Int,
                DbType = DbType.Int32,
                ClrType = typeof(int),
                Default = true
            });
            _sDbTypeMaps.Add(new SqlDbTypeMap()
            {
                DbTypeNumber = 58,
                SqlDbType = SqlDbType.SmallDateTime,
                DbType = DbType.DateTime,
                ClrType = typeof(DateTime),
                Default = false
            });
            _sDbTypeMaps.Add(new SqlDbTypeMap()
            {
                DbTypeNumber = 59,
                SqlDbType = SqlDbType.Real,
                DbType = DbType.Single,
                ClrType = typeof(float),
                Default = true
            });
            _sDbTypeMaps.Add(new SqlDbTypeMap()
            {
                DbTypeNumber = 60,
                SqlDbType = SqlDbType.Money,
                DbType = DbType.Decimal,
                ClrType = typeof(decimal),
                Default = false
            });
            _sDbTypeMaps.Add(new SqlDbTypeMap()
            {
                DbTypeNumber = 61,
                SqlDbType = SqlDbType.DateTime,
                DbType = DbType.DateTime,
                ClrType = typeof(DateTime),
                Default = true
            });
            _sDbTypeMaps.Add(new SqlDbTypeMap()
            {
                DbTypeNumber = 62,
                SqlDbType = SqlDbType.Float,
                DbType = DbType.Double,
                ClrType = typeof(double),
                Default = true
            });
            _sDbTypeMaps.Add(new SqlDbTypeMap()
            {
                DbTypeNumber = 98,
                SqlDbType = SqlDbType.Variant,
                DbType = DbType.Object,
                ClrType = typeof(object),
                Default = true
            });
            _sDbTypeMaps.Add(new SqlDbTypeMap()
            {
                DbTypeNumber = 99,
                SqlDbType = SqlDbType.NText,
                DbType = DbType.String,
                ClrType = typeof(string),
                Default = false
            });
            _sDbTypeMaps.Add(new SqlDbTypeMap()
            {
                DbTypeNumber = 104,
                SqlDbType = SqlDbType.Bit,
                DbType = DbType.Boolean,
                ClrType = typeof(bool),
                Default = true
            });
            _sDbTypeMaps.Add(new SqlDbTypeMap()
            {
                DbTypeNumber = 106,
                SqlDbType = SqlDbType.Decimal,
                DbType = DbType.Decimal,
                ClrType = typeof(decimal),
                Default = false
            });
            _sDbTypeMaps.Add(new SqlDbTypeMap()
            {
                DbTypeNumber = 108,
                SqlDbType = SqlDbType.Decimal,
                DbType = DbType.Decimal,
                ClrType = typeof(decimal),
                Default = true
            });
            _sDbTypeMaps.Add(new SqlDbTypeMap()
            {
                DbTypeNumber = 122,
                SqlDbType = SqlDbType.SmallMoney,
                DbType = DbType.Decimal,
                ClrType = typeof(decimal),
                Default = false
            });
            _sDbTypeMaps.Add(new SqlDbTypeMap()
            {
                DbTypeNumber = 127,
                SqlDbType = SqlDbType.BigInt,
                DbType = DbType.Int64,
                ClrType = typeof(long),
                Default = true
            });
            _sDbTypeMaps.Add(new SqlDbTypeMap()
            {
                DbTypeNumber = 165,
                SqlDbType = SqlDbType.VarBinary,
                DbType = DbType.Binary,
                ClrType = typeof(byte[]),
                Default = true
            });
            _sDbTypeMaps.Add(new SqlDbTypeMap()
            {
                DbTypeNumber = 167,
                SqlDbType = SqlDbType.VarChar,
                DbType = DbType.AnsiString,
                ClrType = typeof(string),
                Default = false
            });
            _sDbTypeMaps.Add(new SqlDbTypeMap()
            {
                DbTypeNumber = 173,
                SqlDbType = SqlDbType.Binary,
                DbType = DbType.Binary,
                ClrType = typeof(byte[]),
                Default = false
            });
            _sDbTypeMaps.Add(new SqlDbTypeMap()
            {
                DbTypeNumber = 175,
                SqlDbType = SqlDbType.Char,
                DbType = DbType.AnsiString,
                ClrType = typeof(char),
                Default = true
            });
            _sDbTypeMaps.Add(new SqlDbTypeMap()
            {
                DbTypeNumber = 189,
                SqlDbType = SqlDbType.Timestamp,
                DbType = DbType.Binary,
                ClrType = typeof(byte[]),
                Default = true
            });
            _sDbTypeMaps.Add(new SqlDbTypeMap()
            {
                DbTypeNumber = 231,
                SqlDbType = SqlDbType.NVarChar,
                DbType = DbType.String,
                ClrType = typeof(string),
                Default = true
            });
            _sDbTypeMaps.Add(new SqlDbTypeMap()
            {
                DbTypeNumber = 239,
                SqlDbType = SqlDbType.NChar,
                DbType = DbType.String,
                ClrType = typeof(char),
                Default = false
            });
            _sDbTypeMaps.Add(new SqlDbTypeMap()
            {
                DbTypeNumber = 241,
                SqlDbType = SqlDbType.Xml,
                DbType = DbType.Xml,
                ClrType = typeof(XmlDocument),
                Default = true
            });
            _sDbTypeMaps.Add(new SqlDbTypeMap()
            {
                DbTypeNumber = 256,
                SqlDbType = SqlDbType.VarChar,
                DbType = DbType.AnsiString,
                ClrType = typeof(string),
                Default = false
            });
        }

        public bool FindByDbTypeNumber(int dbTypeNumber, out object dbType)
        {
            dbType = SqlDbType.Variant;
            var query = _sDbTypeMaps.Where(t => t.DbTypeNumber == dbTypeNumber && t.Default);

            if (query.Any())
            {
                dbType = query.First().SqlDbType;
                return true;
            }
            else
                return false;
        }

        public bool FindByTypeNumber(int dbTypeNumber, out DbType dbType)
        {
            dbType = DbType.Object;
            var query = _sDbTypeMaps.Where(t => t.DbTypeNumber == dbTypeNumber && t.Default);

            if (query.Any())
            {
                dbType = query.First().DbType;
                return true;
            }
            else
                return false;
        }

        public bool FindByTypeNumber(int dbTypeNumber, out Type clrType)
        {
            clrType = null;
            var query = _sDbTypeMaps.Where(t => t.DbTypeNumber == dbTypeNumber && t.Default);

            if (query.Any())
            {
                clrType = query.First().ClrType;
                return true;
            }
            else
                return false;
        }

        public bool FindByTypeNumber(int dbTypeNumber, bool returnDefaultOnly, out DbType dbType)
        {
            dbType = DbType.Object;
            IEnumerable<SqlDbTypeMap> query = null;

            if (returnDefaultOnly)
                query = _sDbTypeMaps.Where(t => t.DbTypeNumber == dbTypeNumber && t.Default);
            else
                query = _sDbTypeMaps.Where(t => t.DbTypeNumber == dbTypeNumber);

            if (query.Any())
            {
                dbType = query.First().DbType;
                return true;
            }
            else
                return false;
        }


        public bool FindBySqlDbType(SqlDbType dbType, out int typeNumber)
        {
            typeNumber = 0;
            var query = _sDbTypeMaps.Where(t => t.SqlDbType == dbType && t.Default);

            if (query.Any())
            {
                typeNumber = query.First().DbTypeNumber;
                return true;
            }
            else
                return false;
        }

        public bool FindBySqlDbType(SqlDbType sqlDbType, out DbType outDbType)
        {
            outDbType = DbType.Object;
            var query = _sDbTypeMaps.Where(t => t.SqlDbType == sqlDbType && t.Default);

            if (query.Any())
            {
                outDbType = query.First().DbType;
                return true;
            }
            else
                return false;
        }

        public bool FindBySqlDbType(SqlDbType dbType, out Type clrType)
        {
            clrType = null;
            var query = _sDbTypeMaps.Where(t => t.SqlDbType == dbType && t.Default);

            if (query.Any())
            {
                clrType = query.First().ClrType;
                return true;
            }
            else
                return false;
        }

        public bool FindByDbType(DbType dbType, out int typeNumber)
        {
            typeNumber = 0;
            var query = _sDbTypeMaps.Where(t => t.DbType == dbType && t.Default);

            if (query.Any())
            {
                typeNumber = query.First().DbTypeNumber;
                return true;
            }
            else
                return false;
        }

        public bool FindByDbType(DbType dbType, out SqlDbType sqlDbType)
        {
            sqlDbType = SqlDbType.Variant;
            var query = _sDbTypeMaps.Where(t => t.DbType == dbType && t.Default);

            if (query.Any())
            {
                sqlDbType = query.First().SqlDbType;
                return true;
            }
            else
                return false;
        }

        public bool FindByDbType(DbType dbType, out Type clrType)
        {
            clrType = null;
            var query = _sDbTypeMaps.Where(t => t.DbType == dbType && t.Default);

            if (query.Any())
            {
                clrType = query.First().ClrType;
                return true;
            }
            else
                return false;
        }

        public bool FindByClrType(Type clrType, out DbType dbType)
        {
            dbType = DbType.Object;
            var query = _sDbTypeMaps.Where(t => t.ClrType == clrType && t.Default);

            if (query.Any())
            {
                if (query.Count() > 1)
                {
                    var typeQuery = query.ToList().OrderBy(t => Marshal.SizeOf(t));
                    dbType = typeQuery.First().DbType;
                    return true;
                }
                else
                    dbType = query.First().DbType;

                return true;
            }
            else
                return false;
        }

        public bool FindByClrType(Type clrType, out SqlDbType dbType)
        {
            dbType = SqlDbType.Variant;
            var query = _sDbTypeMaps.Where(t => t.ClrType == clrType && t.Default);

            if (query.Any())
            {
                if (query.Count() > 1)
                {
                    var typeQuery = query.ToList().OrderBy(t => Marshal.SizeOf(t));
                    dbType = typeQuery.First().SqlDbType;
                    return true;
                }
                else
                    dbType = query.First().SqlDbType;

                return true;
            }
            else
                return false;
        }

        public bool FindByClrType(Type clrType, out int typeNumber)
        {
            typeNumber = 0;
            var query = _sDbTypeMaps.Where(t => t.ClrType == clrType && t.Default);

            if (query.Any())
            {
                if (query.Count() > 1)
                {
                    var typeQuery = query.ToList().OrderBy(t => Marshal.SizeOf(t));
                    typeNumber = typeQuery.First().DbTypeNumber;
                    return true;
                }
                else
                    typeNumber = query.First().DbTypeNumber;

                return true;
            }
            else
                return false;
        }
    }
}
