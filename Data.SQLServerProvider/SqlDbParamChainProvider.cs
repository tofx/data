using System.Collections.Generic;
using tofx.Data.Abstractions;
using tofx.Data.Providers.SqlServer.ParameterNodes;

namespace tofx.Data.Providers.SqlServer
{
    public class SqlDbParamChainProvider : IDbParamChainProvider
    {
        public IEnumerable<IDbParameterChainNode> GetDbParamChain(IDbParameterParser parser)
        {
            var nodes = new List<IDbParameterChainNode>()
            {
                new SqlDbParamStringNode(parser),
                new SqlDbParamIntegerNode(parser),
                new SqlDbParamDateTimeNode(parser),
                new SqlDbParamByteNode(parser),
                new SqlDbParamBytesNode(parser),
                new SqlDbParamLongNode(parser),
                new SqlDbParamGuidNode(parser),
                new SqlDbParamBooleanNode(parser),
                new SqlDbParamCharNode(parser),
                new SqlDbParamDecimalNode(parser),
                new SqlDbParamShortNode(parser),
                new SqlDbParamDoubleNode(parser),
                new SqlDbParamSingleNode(parser),
                new SqlDbParamUIntNode(parser),
                new SqlDbParamULongNode(parser),
                new SqlDbParamUShortNode(parser)
            };

            for (var i = 0; i < nodes.Count - 1; i++)
            {
                ((DbParameterNode)nodes[i]).NextNode(nodes[i + 1]);
            }

            return nodes;
        }

    }
}
