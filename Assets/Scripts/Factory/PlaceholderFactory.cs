using VContainer;

namespace Factory
{
    public class PlaceholderFactory<T> : IFactory<T> where T : class
    {
        private readonly IObjectResolver _resolver;

        protected PlaceholderFactory(IObjectResolver resolver)
        {
            _resolver = resolver;
        }   

        public T Create()
        {
            return _resolver.Resolve<T>();
        }
    }
}