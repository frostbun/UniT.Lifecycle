#if UNIT_ZENJECT
#nullable enable
namespace UniT.Lifecycle
{
    using System.Collections.Generic;
    using UniT.Logging;
    using UnityEngine.Scripting;
    using Zenject;

    public sealed class ZenjectLifecycleManager : LifecycleManager
    {
        [Preserve]
        public ZenjectLifecycleManager(
            [InjectLocal] IEnumerable<IEarlyLoadable>      earlyLoadableServices,
            [InjectLocal] IEnumerable<IAsyncEarlyLoadable> asyncEarlyLoadableServices,
            [InjectLocal] IEnumerable<ILoadable>           loadableServices,
            [InjectLocal] IEnumerable<IAsyncLoadable>      asyncLoadableServices,
            [InjectLocal] IEnumerable<ILateLoadable>       lateLoadableServices,
            [InjectLocal] IEnumerable<IAsyncLateLoadable>  asyncLateLoadableServices,
            [InjectLocal] IEnumerable<IUpdatable>          updatableServices,
            [InjectLocal] IEnumerable<ILateUpdatable>      lateUpdatableServices,
            [InjectLocal] IEnumerable<IFixedUpdatable>     fixedUpdatableServices,
            [InjectLocal] IEnumerable<IFocusLostListener>  focusLostListeners,
            [InjectLocal] IEnumerable<IFocusGainListener>  focusGainListeners,
            [InjectLocal] IEnumerable<IPauseListener>      pauseListeners,
            [InjectLocal] IEnumerable<IResumeListener>     resumeListeners,
            ILoggerManager                                 loggerManager
        ) : base(
            earlyLoadableServices,
            asyncEarlyLoadableServices,
            loadableServices,
            asyncLoadableServices,
            lateLoadableServices,
            asyncLateLoadableServices,
            updatableServices,
            lateUpdatableServices,
            fixedUpdatableServices,
            focusLostListeners,
            focusGainListeners,
            pauseListeners,
            resumeListeners,
            loggerManager
        )
        {
        }
    }
}
#endif