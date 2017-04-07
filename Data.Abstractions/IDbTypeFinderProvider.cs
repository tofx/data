namespace TOF.Data.Abstractions
{
    public interface IDbTypeFinderProvider
    {
        IDbTypeFinder GetDbTypeFinder();
    }
}
