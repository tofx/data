﻿using TOF.Data.Abstractions;
using System.Data;
using System.Reflection;
using System;

namespace TOF.Data
{
    public class DbModelPropertyStrategy : IDbModelPropertyStrategy
    {
        private readonly PropertyInfo _propertyInfo;
        private string _name;
        private DbType? _dbType;
        private int? _length;
        private bool _isKey;
        private bool _allowNull;
        private bool _isIncremental;
        private bool _isAutogenerated;
        private bool _isAutogeneratedAtUpdate;
        private bool _isMaxLength;
        private bool _isIgnore;
        private DbIgnorePropertyKinds _ignorePropertyKinds = DbIgnorePropertyKinds.InsertAndUpdate;
        private ParameterDirection _parameterDirection = ParameterDirection.Input;

        public DbModelPropertyStrategy(PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;
            _name = _propertyInfo.Name;
        }

        public PropertyInfo GetPropertyInfo()
        {
            return _propertyInfo;
        }

        public string GetParameterName()
        {
            return _name;
        }

        public DbType? GetMapDbType()
        {
            return _dbType;
        }

        public int? GetLength()
        {
            return _length;
        }

        public bool IsKey()
        {
            return _isKey;
        }

        public bool IsAllowNull()
        {
            return _allowNull;
        }

        public bool IsIncremental()
        {
            return _isIncremental;
        }

        public bool IsAutoGenerated()
        {
            return _isAutogenerated;
        }

        public bool IsAutoGeneratedAtUpdate()
        {
            return _isAutogeneratedAtUpdate;
        }

        public bool IsIgnoreProperty()
        {
            return _isIgnore;
        }

        public DbIgnorePropertyKinds GetIgnorePropertyKinds()
        {
            return _ignorePropertyKinds;
        }

        public IDbModelPropertyStrategy DbName(string name)
        {
            _name = name;
            return this;
        }

        public IDbModelPropertyStrategy AsKey()
        {
            _isKey = true;
            return this;
        }

        public IDbModelPropertyStrategy MapDbType(DbType type)
        {
            _dbType = type;
            return this;
        }

        public IDbModelPropertyStrategy AllowNull()
        {
            _allowNull = true;
            return this;
        }

        public IDbModelPropertyStrategy AsIncremental()
        {
            _isIncremental = true;
            return this;
        }

        public IDbModelPropertyStrategy AsAutoGenerated()
        {
            _isAutogenerated = true;
            return this;
        }

        public IDbModelPropertyStrategy AsAutoGeneratedAtUpdate()
        {
            _isAutogeneratedAtUpdate = true;
            return this;
        }

        public IDbModelPropertyStrategy Length(int length)
        {
            _length = length;
            return this;
        }

        public bool IsMaxLength()
        {
            return _isMaxLength;
        }

        public IDbModelPropertyStrategy AsMaxLength()
        {
            _isMaxLength = true;
            return this;
        }

        public IDbModelPropertyStrategy IgnoreProperty(DbIgnorePropertyKinds ignorePropertyKinds)
        {
            _isIgnore = true;
            _ignorePropertyKinds = ignorePropertyKinds;
            return this;
        }

        public ParameterDirection GetParameterDirection()
        {
            return _parameterDirection;
        }

        public IDbModelPropertyStrategy AsOutputParameter()
        {
            _parameterDirection = ParameterDirection.Output;
            return this;
        }

        public IDbModelPropertyStrategy AsReturnParameter()
        {
            _parameterDirection = ParameterDirection.ReturnValue;
            return this;
        }

        public IDbModelPropertyStrategy AsInputOutputParameter()
        {
            _parameterDirection = ParameterDirection.InputOutput;
            return this;
        }
    }
}
