#nullable enable
namespace UniT.Lifecycle.Default.DI
{
    using System.Collections.Generic;
    using UniT.Logging;
    using Zenject;
    using Zenject.Internal;

    public static class LifecycleManagerZenject
    {
        public static void BindLifecycleManager(this DiContainer container)
        {
            container.BindInterfacesTo<ZenjectLifecycleManager>().AsSingle();
        }

        private sealed class ZenjectLifecycleManager : LifecycleManager
        {
            [Preserve]
            public ZenjectLifecycleManager(
                [InjectLocal] IEnumerable<ILoadOrder>         loadableServices,
                [InjectLocal] IEnumerable<IUpdatable>         updatableServices,
                [InjectLocal] IEnumerable<ILateUpdatable>     lateUpdatableServices,
                [InjectLocal] IEnumerable<IFixedUpdatable>    fixedUpdatableServices,
                [InjectLocal] IEnumerable<IFocusLostListener> focusLostListeners,
                [InjectLocal] IEnumerable<IFocusGainListener> focusGainListeners,
                [InjectLocal] IEnumerable<IPauseListener>     pauseListeners,
                [InjectLocal] IEnumerable<IResumeListener>    resumeListeners,
                ILoggerManager                                loggerManager
            ) : base(
                loadableServices,
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
}