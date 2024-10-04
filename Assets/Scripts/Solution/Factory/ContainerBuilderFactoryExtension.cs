using VContainer;

namespace Solution.Factory
{
    public static class ContainerBuilderFactoryExtension
    {
        public static void RegisterFactory<TClass, TFactory>(this IContainerBuilder builder) 
            where TClass : class
            where TFactory : PlaceholderFactory<TClass> 
        {
            builder.Register<TFactory>(Lifetime.Singleton).As<IFactory<TClass>>();
            builder.Register<TClass>(Lifetime.Transient);
        }
    }
}