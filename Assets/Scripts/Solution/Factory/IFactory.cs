namespace Solution.Factory
{
    public interface IFactory<T>
    {
        T Create();
    }
}