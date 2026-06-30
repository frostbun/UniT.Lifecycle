#nullable enable
namespace UniT.Lifecycle
{
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;

    public interface ILifecycleManager
    {
        public UniTask LoadAsync(IProgress<float>? progress = null, CancellationToken cancellationToken = default);
    }
}