using Cysharp.Threading.Tasks;

namespace Solution.Command
{
    public interface ICommand
    {
        UniTask Execute();
    }

    public interface ICommand<TArg>
    {
        void Execute(TArg arguments);
    }

    public interface ICommandWithResult<TRes>
    {
        UniTask<TRes> Execute();
    }

    public interface ICommandWithResult<TArg, TRes>
    {
        UniTask<TRes> Execute(TArg arguments);
    }
}