namespace tofx.Data.Abstractions
{
    public interface IDbTypeFinderProvider
    {
        IDbTypeFinder GetDbTypeFinder();
    }
}
