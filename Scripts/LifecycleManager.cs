#nullable enable
namespace UniT.Lifecycle
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UniT.Extensions;
    using UniT.Logging;
    using UnityEngine;
    using ILogger = UniT.Logging.ILogger;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public abstract class LifecycleManager : ILifecycleManager
    {
        private readonly IReadOnlyList<IEarlyLoadable>      earlyLoadableServices;
        private readonly IReadOnlyList<IAsyncEarlyLoadable> asyncEarlyLoadableServices;
        private readonly IReadOnlyList<ILoadable>           loadableServices;
        private readonly IReadOnlyList<IAsyncLoadable>      asyncLoadableServices;
        private readonly IReadOnlyList<ILateLoadable>       lateLoadableServices;
        private readonly IReadOnlyList<IAsyncLateLoadable>  asyncLateLoadableServices;

        private readonly IReadOnlyList<IUpdatable>            updatableServices;
        private readonly IReadOnlyList<ILateUpdatable>        lateUpdatableServices;
        private readonly IReadOnlyList<IFixedUpdatable>       fixedUpdatableServices;
        private readonly IReadOnlyList<IFocusChangedListener> focusChangedListeners;
        private readonly IReadOnlyList<IPauseChangedListener> pauseChangedListeners;

        private readonly ILogger logger;

        protected LifecycleManager(
            IEnumerable<IEarlyLoadable>        earlyLoadableServices,
            IEnumerable<IAsyncEarlyLoadable>   asyncEarlyLoadableServices,
            IEnumerable<ILoadable>             loadableServices,
            IEnumerable<IAsyncLoadable>        asyncLoadableServices,
            IEnumerable<ILateLoadable>         lateLoadableServices,
            IEnumerable<IAsyncLateLoadable>    asyncLateLoadableServices,
            IEnumerable<IUpdatable>            updatableServices,
            IEnumerable<ILateUpdatable>        lateUpdatableServices,
            IEnumerable<IFixedUpdatable>       fixedUpdatableServices,
            IEnumerable<IFocusChangedListener> focusChangedListeners,
            IEnumerable<IPauseChangedListener> pauseChangedListeners,
            ILoggerManager                     loggerManager
        )
        {
            this.earlyLoadableServices      = earlyLoadableServices.ToArray();
            this.asyncEarlyLoadableServices = asyncEarlyLoadableServices.ToArray();
            this.loadableServices           = loadableServices.ToArray();
            this.asyncLoadableServices      = asyncLoadableServices.ToArray();
            this.lateLoadableServices       = lateLoadableServices.ToArray();
            this.asyncLateLoadableServices  = asyncLateLoadableServices.ToArray();

            this.updatableServices      = updatableServices.ToArray();
            this.lateUpdatableServices  = lateUpdatableServices.ToArray();
            this.fixedUpdatableServices = fixedUpdatableServices.ToArray();
            this.focusChangedListeners  = focusChangedListeners.ToArray();
            this.pauseChangedListeners  = pauseChangedListeners.ToArray();

            this.logger = loggerManager.GetLogger(this);
            this.logger.Debug("Constructed");
        }

        private bool isLoading;

        #if UNIT_UNITASK
        async UniTask ILifecycleManager.LoadAsync(IProgress<float>? progress, CancellationToken cancellationToken)
        {
            if (this.isLoading)
            {
                await UniTask.WaitUntil(this, @this => !@this.isLoading, cancellationToken: cancellationToken);
            }

            if (this.eventListener) return;

            this.isLoading = true;
            try
            {
                var subProgresses = progress.CreateSubProgresses(3).ToArray();
                this.logger.Debug("Early loading");
                this.earlyLoadableServices.ForEach(service =>
                {
                    this.logger.Debug($"Loading {service.GetType().Name}");
                    service.Load();
                    this.logger.Debug($"Loaded {service.GetType().Name}");
                });
                await this.asyncEarlyLoadableServices.ForEachAsync(
                    async (service, progress, cancellationToken) =>
                    {
                        this.logger.Debug($"Loading {service.GetType().Name}");
                        await service.LoadAsync(progress, cancellationToken);
                        this.logger.Debug($"Loaded {service.GetType().Name}");
                    },
                    subProgresses[0],
                    cancellationToken
                );
                this.logger.Debug("Early loaded");
                subProgresses[0]?.Report(1);
                this.logger.Debug("Loading");
                this.loadableServices.ForEach(service =>
                {
                    this.logger.Debug($"Loading {service.GetType().Name}");
                    service.Load();
                    this.logger.Debug($"Loaded {service.GetType().Name}");
                });
                await this.asyncLoadableServices.ForEachAsync(
                    async (service, progress, cancellationToken) =>
                    {
                        this.logger.Debug($"Loading {service.GetType().Name}");
                        await service.LoadAsync(progress, cancellationToken);
                        this.logger.Debug($"Loaded {service.GetType().Name}");
                    },
                    subProgresses[1],
                    cancellationToken
                );
                this.logger.Debug("Loaded");
                subProgresses[1]?.Report(1);
                this.logger.Debug("Late loading");
                this.lateLoadableServices.ForEach(service =>
                {
                    this.logger.Debug($"Loading {service.GetType().Name}");
                    service.Load();
                    this.logger.Debug($"Loaded {service.GetType().Name}");
                });
                await this.asyncLateLoadableServices.ForEachAsync(
                    async (service, progress, cancellationToken) =>
                    {
                        this.logger.Debug($"Loading {service.GetType().Name}");
                        await service.LoadAsync(progress, cancellationToken);
                        this.logger.Debug($"Loaded {service.GetType().Name}");
                    },
                    subProgresses[2],
                    cancellationToken
                );
                this.logger.Debug("Late loaded");
                subProgresses[2]?.Report(1);
                this.Load();
            }
            finally
            {
                this.isLoading = false;
            }
        }
        #else
        IEnumerator ILifecycleManager.LoadAsync(Action? callback, IProgress<float>? progress)
        {
            if (this.isLoading)
            {
                yield return new WaitUntil(() => !this.isLoading);
            }

            if (this.eventListener) yield break;

            this.isLoading = true;
            try
            {
                var subProgresses = progress.CreateSubProgresses(3).ToArray();
                this.logger.Debug("Early loading");
                this.earlyLoadableServices.ForEach(service =>
                {
                    this.logger.Debug($"Loading {service.GetType().Name}");
                    service.Load();
                    this.logger.Debug($"Loaded {service.GetType().Name}");
                });
                yield return this.asyncEarlyLoadableServices.ForEachAsync(
                    (service, progress) =>
                    {
                        this.logger.Debug($"Loading {service.GetType().Name}");
                        return service.LoadAsync(() => this.logger.Debug($"Loaded {service.GetType().Name}"), progress);
                    },
                    progress: subProgresses[0]
                );
                this.logger.Debug("Early loaded");
                subProgresses[0]?.Report(1);
                this.logger.Debug("Loading");
                this.loadableServices.ForEach(service =>
                {
                    this.logger.Debug($"Loading {service.GetType().Name}");
                    service.Load();
                    this.logger.Debug($"Loaded {service.GetType().Name}");
                });
                yield return this.asyncLoadableServices.ForEachAsync(
                    (service, progress) =>
                    {
                        this.logger.Debug($"Loading {service.GetType().Name}");
                        return service.LoadAsync(() => this.logger.Debug($"Loaded {service.GetType().Name}"), progress);
                    },
                    progress: subProgresses[1]
                );
                this.logger.Debug("Loaded");
                subProgresses[1]?.Report(1);
                this.logger.Debug("Late loading");
                this.lateLoadableServices.ForEach(service =>
                {
                    this.logger.Debug($"Loading {service.GetType().Name}");
                    service.Load();
                    this.logger.Debug($"Loaded {service.GetType().Name}");
                });
                yield return this.asyncLateLoadableServices.ForEachAsync(
                    (service, progress) =>
                    {
                        this.logger.Debug($"Loading {service.GetType().Name}");
                        return service.LoadAsync(() => this.logger.Debug($"Loaded {service.GetType().Name}"), progress);
                    },
                    progress: subProgresses[2]
                );
                this.logger.Debug("Late loaded");
                subProgresses[2]?.Report(1);
                this.Load();
            }
            finally
            {
                this.isLoading = false;
            }

            callback?.Invoke();
        }
        #endif

        private EventListener eventListener = null!;

        private void Load()
        {
            this.eventListener = new GameObject(nameof(LifecycleManager)).AddComponent<EventListener>().DontDestroyOnLoad();

            this.updatableServices.ForEach(service => this.eventListener.Updating           += service.Update);
            this.lateUpdatableServices.ForEach(service => this.eventListener.LateUpdating   += service.LateUpdate);
            this.fixedUpdatableServices.ForEach(service => this.eventListener.FixedUpdating += service.FixedUpdate);

            this.focusChangedListeners.ForEach(listener =>
            {
                this.eventListener.FocusGain += listener.OnFocusGain;
                this.eventListener.FocusLost += listener.OnFocusLost;
            });

            this.pauseChangedListeners.ForEach(listener =>
            {
                this.eventListener.Paused  += listener.OnPause;
                this.eventListener.Resumed += listener.OnResume;
            });
        }

        private void Unload()
        {
            if (!this.eventListener) return;

            this.updatableServices.ForEach(service => this.eventListener.Updating           -= service.Update);
            this.lateUpdatableServices.ForEach(service => this.eventListener.LateUpdating   -= service.LateUpdate);
            this.fixedUpdatableServices.ForEach(service => this.eventListener.FixedUpdating -= service.FixedUpdate);

            this.focusChangedListeners.ForEach(listener =>
            {
                this.eventListener.FocusGain -= listener.OnFocusGain;
                this.eventListener.FocusLost -= listener.OnFocusLost;
            });

            this.pauseChangedListeners.ForEach(listener =>
            {
                this.eventListener.Paused  -= listener.OnPause;
                this.eventListener.Resumed -= listener.OnResume;
            });

            this.eventListener.gameObject.Destroy();
        }

        void IDisposable.Dispose() => this.Unload();

        ~LifecycleManager() => this.Unload();

        private sealed class EventListener : MonoBehaviour
        {
            public event Action? Updating;
            public event Action? LateUpdating;
            public event Action? FixedUpdating;

            public event Action? FocusGain;
            public event Action? FocusLost;

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