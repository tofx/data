namespace TOF.Data.Abstractions
{
    public interface IDbQueryExpressionMemberNameParserProvider
    {
        IDbQueryExpressionMemberNameParser GetParser(IDbModelStrategy modelStrategy = null);
    }
}
