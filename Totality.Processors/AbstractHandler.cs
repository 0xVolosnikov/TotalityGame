using Totality.Handlers.News;
using Totality.Model;
using Totality.Model.Interfaces;

namespace Totality.Handlers
{
    public class AbstractHandler : AbstractLoggable
    {
        protected IDataLayer _dataLayer;
        protected NewsHandler _newsHandler;

        public AbstractHandler(NewsHandler newsHandler, IDataLayer dataLayer, ILogger logger) : base (logger)
        {
            _newsHandler = newsHandler;
            _dataLayer = dataLayer;
        }
    }
}
