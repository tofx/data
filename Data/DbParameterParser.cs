using TOF.Core.DependencyInjection;
using TOF.Core.Infrastructure;
using System.Data;
using System.Linq;
using System.Reflection;
using TOF.Data.Abstractions;

namespace TOF.Data
{
    public class DbParameterParser : IDbParameterParser
    {
        private Container _container = null;
        private IDbParameterChainNode NodeStart { get; set; }
        private IDbModelPropertyStrategy ModelPropertyStrategy { get; set; }
        private PropertyInfo PropertyInfo { get; set; }

        public DbParameterParser(IDbModelPropertyStrategy modelPropertyStrategy)
        {
            NodeStart = null;
            this.ModelPropertyStrategy = modelPropertyStrategy;
            Initialize();
        }

        public DbParameterParser(PropertyInfo propertyInfo)
        {
            NodeStart = null;
            this.PropertyInfo = propertyInfo;
            Initialize();
        }

        private void Initialize()
        {
            _container = App.ServiceProviders;
            NodeStart = PrepareParameterChains();
        }

        private IDbParameterChainNode PrepareParameterChains()
        {
            var nodes = _container.Resolve<IDbParamChainProvider>().GetDbParamChain(this);

            return nodes.First();
        }

        public IDbDataParameter GetDbParameter()
        {
            return NodeStart.GetDbParameter();
        }

        public IDbModelPropertyStrategy GetModelPropertyStrategy()
        {
            return ModelPropertyStrategy;
        }

        public PropertyInfo GetPropertyInfo()
        {
            return PropertyInfo;
        }
    }
}
