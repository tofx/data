using System.Collections.Generic;

namespace tofx.Data.Abstractions
{
    public interface IDbParamChainProvider
    {
        IEnumerable<IDbParameterChainNode> GetDbParamChain(IDbParameterParser parser);
    }
}
