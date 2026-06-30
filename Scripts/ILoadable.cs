#nullable enable
namespace UniT.Lifecycle
{
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;

    public interface ILoadOrder
    {
        public int Order => 0;
    }

    public interface ILoadable : ILoadOrder
    {
        public void Load();
    }

    public interface IAsyncLoadable : ILoadOrder
    {
        public UniTask LoadAsync(IProgress<float>? progress = null, CancellationToken cancellationToken = default);
    }

    public interface IEarlyLoadable : ILoadable
    {
        int ILoadOrder.Order => int.MinValue / 2;
    }

    public interface IAsyncEarlyLoadable : IAsyncLoadable
    {
        int ILoadOrder.Order => int.MinValue / 2;
    }

    public interface ILateLoadable : ILoadable
    {
        int ILoadOrder.Order => int.MaxValue / 2;
    }

    public interface IAsyncLateLoadable : IAsyncLoadable
    {
        int ILoadOrder.Order => int.MaxValue / 2;
    }
}