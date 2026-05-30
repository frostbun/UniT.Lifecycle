#nullable enable
namespace UniT.Lifecycle.Default
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UniT.Extensions;
    using UniT.Logging;
    using UnityEngine;
    using UnityEngine.Scripting;
    using ILogger = UniT.Logging.ILogger;
    using Object = UnityEngine.Object;

    public class LifecycleManager : ILifecycleManager
    {
        private readonly IReadOnlyList<ILoadOrder>         loadableServices;
        private readonly IReadOnlyList<IUpdatable>         updatableServices;
        private readonly IReadOnlyList<ILateUpdatable>     lateUpdatableServices;
        private readonly IReadOnlyList<IFixedUpdatable>    fixedUpdatableServices;
        private readonly IReadOnlyList<IFocusLostListener> focusLostListeners;
        private readonly IReadOnlyList<IFocusGainListener> focusGainListeners;
        private readonly IReadOnlyList<IPauseListener>     pauseListeners;
        private readonly IReadOnlyList<IResumeListener>    resumeListeners;

        private readonly ILogger logger;

        [Preserve]
        public LifecycleManager(
            IEnumerable<ILoadOrder>         loadableServices,
            IEnumerable<IUpdatable>         updatableServices,
            IEnumerable<ILateUpdatable>     lateUpdatableServices,
            IEnumerable<IFixedUpdatable>    fixedUpdatableServices,
            IEnumerable<IFocusLostListener> focusLostListeners,
            IEnumerable<IFocusGainListener> focusGainListeners,
            IEnumerable<IPauseListener>     pauseListeners,
            IEnumerable<IResumeListener>    resumeListeners,
            ILoggerManager                  loggerManager
        )
        {
            this.loadableServices       = loadableServices.ToArray();
            this.updatableServices      = updatableServices.ToArray();
            this.lateUpdatableServices  = lateUpdatableServices.ToArray();
            this.fixedUpdatableServices = fixedUpdatableServices.ToArray();
            this.focusLostListeners     = focusLostListeners.ToArray();
            this.focusGainListeners     = focusGainListeners.ToArray();
            this.pauseListeners         = pauseListeners.ToArray();
            this.resumeListeners        = resumeListeners.ToArray();

            this.logger = loggerManager.GetLogger(this);
            this.logger.Debug("Constructed");
        }

        private EventListener eventListener = null!;
        private bool          isLoading;

        async UniTask ILifecycleManager.LoadAsync(IProgress<float>? progress, CancellationToken cancellationToken)
        {
            if (this.isLoading) await UniTask.WaitUntil(this, static @this => !@this.isLoading, cancellationToken: cancellationToken);
            if (this.eventListener) return;
            this.isLoading = true;
            try
            {
                await this.loadableServices.GroupBy(service => service.Order)
                    .OrderBy(group => group.Key)
                    .ForEachAwaitAsync(async (group, progress, cancellationToken) =>
                    {
                        this.logger.Debug($"Loading group {group.Key}");

                        var (sync, async) = group.Split(service => service is ILoadable);

                        var task = async.Cast<IAsyncLoadable>().ForEachAsync(async (service, progress, cancellationToken) =>
                        {
                            this.logger.Debug($"Loading {service.GetType().Name}");
                            await service.LoadAsync(progress, cancellationToken);
                            this.logger.Debug($"Loaded {service.GetType().Name}");
                        }, progress, cancellationToken);

                        sync.Cast<ILoadable>().ForEach(static (service, @this) =>
                        {
                            @this.logger.Debug($"Loading {service.GetType().Name}");
                            service.Load();
                            @this.logger.Debug($"Loaded {service.GetType().Name}");
                        }, this);

                        await task;

                        this.logger.Debug($"Loaded group {group.Key}");
                    }, progress, cancellationToken);

                this.eventListener = new GameObject(nameof(LifecycleManager)).AddComponent<EventListener>().DontDestroyOnLoad();

                foreach (var service in this.updatableServices) this.eventListener.Updating           += service.Update;
                foreach (var service in this.lateUpdatableServices) this.eventListener.LateUpdating   += service.LateUpdate;
                foreach (var service in this.fixedUpdatableServices) this.eventListener.FixedUpdating += service.FixedUpdate;
                foreach (var service in this.focusLostListeners) this.eventListener.FocusLost         += service.OnFocusLost;
                foreach (var service in this.focusGainListeners) this.eventListener.FocusGain         += service.OnFocusGain;
                foreach (var service in this.pauseListeners) this.eventListener.Paused                += service.OnPause;
                foreach (var service in this.resumeListeners) this.eventListener.Resumed              += service.OnResume;
            }
            finally
            {
                this.isLoading = false;
            }
        }

        private void Unload()
        {
            if (!this.eventListener) return;

            foreach (var service in this.updatableServices) this.eventListener.Updating           -= service.Update;
            foreach (var service in this.lateUpdatableServices) this.eventListener.LateUpdating   -= service.LateUpdate;
            foreach (var service in this.fixedUpdatableServices) this.eventListener.FixedUpdating -= service.FixedUpdate;
            foreach (var service in this.focusLostListeners) this.eventListener.FocusLost         -= service.OnFocusLost;
            foreach (var service in this.focusGainListeners) this.eventListener.FocusGain         -= service.OnFocusGain;
            foreach (var service in this.pauseListeners) this.eventListener.Paused                -= service.OnPause;
            foreach (var service in this.resumeListeners) this.eventListener.Resumed              -= service.OnResume;

            Object.Destroy(this.eventListener.gameObject);
        }

        void IDisposable.Dispose()
        {
            this.Unload();
            this.logger.Debug("Disposed");
        }

        ~LifecycleManager()
        {
            this.Unload();
            this.logger.Debug("Finalized");
        }

        private sealed class EventListener : MonoBehaviour
        {
            public event Action? Updating;
            public event Action? LateUpdating;
            public event Action? FixedUpdating;

            public event Action? FocusLost;
            public event Action? FocusGain;

            public event Action? Paused;
            public event Action? Resumed;

            private void Update()
            {
                this.Updating?.Invoke();
            }

            private void LateUpdate()
            {
                this.LateUpdating?.Invoke();
            }

            private void FixedUpdate()
            {
                this.FixedUpdating?.Invoke();
            }

            private void OnApplicationFocus(bool hasFocus)
            {
                if (hasFocus)
                {
                    this.FocusGain?.Invoke();
                }
                else
                {
                    this.FocusLost?.Invoke();
                }
            }

            private void OnApplicationPause(bool pauseStatus)
            {
                if (pauseStatus)
                {
                    this.Paused?.Invoke();
                }
                else
                {
                    this.Resumed?.Invoke();
                }
            }
        }
    }
}