﻿namespace tofx.Data.Abstractions
{
    public interface IDbSchemaActionProvider
    {
        void Create();
        void Alter();
        void Drop();
        bool SchemaExists();
    }
}
