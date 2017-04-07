namespace tofx.Data.Abstractions
{
    public interface IDbQueryExpressionParserProvider
    {
        IDbQueryExpressionParser GetExpressionParser();
    }
}
