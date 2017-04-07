namespace tofx.Data.Abstractions
{
    public interface IDbObject
    {
        string Name { get; }
        DbObjectTypes Type { get; }
    }
}