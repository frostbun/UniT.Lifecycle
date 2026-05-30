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
                ContainerLocal<IEnumerable<ILoadOrder>>         loadableServices,
                ContainerLocal<IEnumerable<IUpdatable>>         updatableServices,
                ContainerLocal<IEnumerable<ILateUpdatable>>     lateUpdatableServices,
                ContainerLocal<IEnumerable<IFixedUpdatable>>    fixedUpdatableServices,
                ContainerLocal<IEnumerable<IFocusLostListener>> focusLostListeners,
                ContainerLocal<IEnumerable<IFocusGainListener>> focusGainListeners,
                ContainerLocal<IEnumerable<IPauseListener>>     pauseListeners,
                ContainerLocal<IEnumerable<IResumeListener>>    resumeListeners,
                ILoggerManager                                  loggerManager
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