using System.Data;

namespace tofx.Data.Abstractions
{
    public interface IDbParameterChainNode
    {
        IDbDataParameter GetDbParameter();
    }
}