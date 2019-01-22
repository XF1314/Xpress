

using Xpress.Core.Collections;

namespace Xpress.Core.EventBus.Local
{
    public class LocalEventBusOptions
    {
        public ITypeList<ILocalEventHandler> Handlers { get; }

        public LocalEventBusOptions()
        {
            Handlers = new TypeList<ILocalEventHandler>();
        }
    }
}