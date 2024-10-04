using Cysharp.Threading.Tasks;
using UnityEditor;

namespace Solution.Command
{
    public interface ICommandBase
    {
        void Undo();
        void Redo();
    }
    
    public interface ICommand : ICommandBase
    {
        UniTask Execute();
    }

    public interface ICommand<TArg> : ICommandBase
    {
        void Execute(TArg arguments);
    }

    public interface ICommandWithResult<TRes> : ICommandBase
    {
        UniTask<TRes> Execute();
    }

    public interface ICommandWithResult<TArg, TRes> : ICommandBase
    {
        UniTask<TRes> Execute(TArg arguments);
    }
}