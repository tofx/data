using System.Data;

namespace TOF.Data.Abstractions
{
    public interface IDbParameterChainNode
    {
        IDbDataParameter GetDbParameter();
    }
}