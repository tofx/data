namespace TOF.Data.Abstractions
{
    public interface IDbProviderOptions
    {
        string ConnectionString { get; }
        string ConnectionStringName { get; }
    }
}
