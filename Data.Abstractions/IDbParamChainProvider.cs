using System.Collections.Generic;

namespace TOF.Data.Abstractions
{
    public interface IDbParamChainProvider
    {
        IEnumerable<IDbParameterChainNode> GetDbParamChain(IDbParameterParser parser);
    }
}
