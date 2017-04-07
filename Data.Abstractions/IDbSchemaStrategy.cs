namespace TOF.Data.Abstractions
{
    public interface IDbSchemaStrategy
    {
        DbSchemaStrategyTypes Type { get; }
        string GetDbSchemaScript();
    }
}
