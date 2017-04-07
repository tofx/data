namespace tofx.Data.Abstractions
{
    public interface IDbSchemaStrategyProvider
    {
        IDbSchemaStrategy GetCreateTableStrategy(IDbModelStrategy modelStrategy);
        IDbSchemaStrategy GetAlterTableStrategy(IDbModelStrategy modelStrategy);
        IDbSchemaStrategy GetDropTableStrategy(IDbModelStrategy modelStrategy);
        IDbSchemaStrategy GetLookupSchemaExistsStrategy(IDbModelStrategy modelStrategy);
    }
}
