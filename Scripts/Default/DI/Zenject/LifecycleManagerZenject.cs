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
                [InjectLocal] IReadOnlyList<ILoadOrder>         loadableServices,
                [InjectLocal] IReadOnlyList<IUpdatable>         updatableServices,
                [InjectLocal] IReadOnlyList<ILateUpdatable>     lateUpdatableServices,
                [InjectLocal] IReadOnlyList<IFixedUpdatable>    fixedUpdatableServices,
                [InjectLocal] IReadOnlyList<IFocusLostListener> focusLostListeners,
                [InjectLocal] IReadOnlyList<IFocusGainListener> focusGainListeners,
                [InjectLocal] IReadOnlyList<IPauseListener>     pauseListeners,
                [InjectLocal] IReadOnlyList<IResumeListener>    resumeListeners,
                ILoggerManager                                  loggerManager
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