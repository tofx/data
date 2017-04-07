namespace tofx.Data.Abstractions
{
    public interface IDbQueryExpressionMemberNameParserProvider
    {
        IDbQueryExpressionMemberNameParser GetParser(IDbModelStrategy modelStrategy = null);
    }
}
