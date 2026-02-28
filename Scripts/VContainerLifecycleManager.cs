#if UNIT_VCONTAINER
#nullable enable
namespace UniT.Lifecycle
{
    using System.Collections.Generic;
    using UniT.Logging;
    using UnityEngine.Scripting;
    using VContainer.Internal;

    public sealed class VContainerLifecycleManager : LifecycleManager
    {
        [Preserve]
        public VContainerLifecycleManager(
            ContainerLocal<IEnumerable<IEarlyLoadable>>        earlyLoadableServices,
            ContainerLocal<IEnumerable<IAsyncEarlyLoadable>>   asyncEarlyLoadableServices,
            ContainerLocal<IEnumerable<ILoadable>>             loadableServices,
            ContainerLocal<IEnumerable<IAsyncLoadable>>        asyncLoadableServices,
            ContainerLocal<IEnumerable<ILateLoadable>>         lateLoadableServices,
            ContainerLocal<IEnumerable<IAsyncLateLoadable>>    asyncLateLoadableServices,
            ContainerLocal<IEnumerable<IUpdatable>>            updatableServices,
            ContainerLocal<IEnumerable<ILateUpdatable>>        lateUpdatableServices,
            ContainerLocal<IEnumerable<IFixedUpdatable>>       fixedUpdatableServices,
            ContainerLocal<IEnumerable<IFocusChangedListener>> focusChangedListeners,
            ContainerLocal<IEnumerable<IPauseChangedListener>> pauseChangedListeners,
            ILoggerManager                                     loggerManager
        ) : base(
            earlyLoadableServices.Value,
            asyncEarlyLoadableServices.Value,
            loadableServices.Value,
            asyncLoadableServices.Value,
            lateLoadableServices.Value,
            asyncLateLoadableServices.Value,
            updatableServices.Value,
            lateUpdatableServices.Value,
            fixedUpdatableServices.Value,
            focusChangedListeners.Value,
            pauseChangedListeners.Value,
            loggerManager
        )
        {
        }
    }
}
#endif