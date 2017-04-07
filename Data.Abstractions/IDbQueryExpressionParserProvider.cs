namespace TOF.Data.Abstractions
{
    public interface IDbQueryExpressionParserProvider
    {
        IDbQueryExpressionParser GetExpressionParser();
    }
}
