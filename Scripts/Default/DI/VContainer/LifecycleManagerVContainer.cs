#nullable enable
namespace UniT.Lifecycle.Default.DI
{
    using System.Collections.Generic;
    using UniT.Logging;
    using VContainer;
    using VContainer.Internal;

    public static class LifecycleManagerVContainer
    {
        public static void RegisterLifecycleManager(this IContainerBuilder builder)
        {
            builder.Register<VContainerLifecycleManager>(Lifetime.Singleton).AsImplementedInterfaces();
        }

        private sealed class VContainerLifecycleManager : LifecycleManager
        {
            [Preserve]
            public VContainerLifecycleManager(
                ContainerLocal<IReadOnlyList<ILoadOrder>>         loadableServices,
                ContainerLocal<IReadOnlyList<IUpdatable>>         updatableServices,
                ContainerLocal<IReadOnlyList<ILateUpdatable>>     lateUpdatableServices,
                ContainerLocal<IReadOnlyList<IFixedUpdatable>>    fixedUpdatableServices,
                ContainerLocal<IReadOnlyList<IFocusLostListener>> focusLostListeners,
                ContainerLocal<IReadOnlyList<IFocusGainListener>> focusGainListeners,
                ContainerLocal<IReadOnlyList<IPauseListener>>     pauseListeners,
                ContainerLocal<IReadOnlyList<IResumeListener>>    resumeListeners,
                ILoggerManager                                    loggerManager
            ) : base(
                loadableServices.Value,
                updatableServices.Value,
                lateUpdatableServices.Value,
                fixedUpdatableServices.Value,
                focusLostListeners.Value,
                focusGainListeners.Value,
                pauseListeners.Value,
                resumeListeners.Value,
                loggerManager
            )
            {
            }
        }
    }
}