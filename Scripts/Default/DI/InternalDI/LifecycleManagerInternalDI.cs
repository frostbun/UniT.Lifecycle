#nullable enable
namespace UniT.Lifecycle.Default.DI
{
    using UniT.DI;

    public static class LifecycleManagerInternalDI
    {
        public static void AddLifecycleManager(this DependencyContainer container)
        {
            container.AddInterfaces<LifecycleManager>();
        }
    }
}