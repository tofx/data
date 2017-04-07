namespace tofx.Data.Abstractions
{
    public interface IDbProviderOptions
    {
        string ConnectionString { get; }
        string ConnectionStringName { get; }
    }
}
