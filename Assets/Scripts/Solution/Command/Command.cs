using Cysharp.Threading.Tasks;
using Solution.Factory;
using VContainer;

namespace Solution.Command
{
    public abstract class Command<T> : ICommand where T: Command<T>
    {
        public abstract UniTask Execute();

        public class Factory : PlaceholderFactory<T>
        {
            public Factory(IObjectResolver resolver) : base(resolver)
            {
                
            }
        } 
    }
    
    public abstract class Command<T, TArg> : ICommand<TArg> where T: Command<T, TArg>
    {
        public abstract void Execute(TArg arguments);
        
        public class Factory : PlaceholderFactory<T>
        {
            public Factory(IObjectResolver resolver) : base(resolver)
            {
                
            }
        } 
    }

    public abstract class CommandWithResultBase<T, TRes> : ICommandWithResult<TRes> where T: CommandWithResultBase<T, TRes>
    {
        public abstract UniTask<TRes> Execute();
        
        public class Factory : PlaceholderFactory<T>
        {
            public Factory(IObjectResolver resolver) : base(resolver)
            {
                
            }
        } 
    }
    
    public abstract class CommandWithResultBase<T, TArg, TRes> : ICommandWithResult<TArg, TRes> where T: CommandWithResultBase<T, TArg, TRes>
    {
        public abstract UniTask<TRes> Execute(TArg arguments);
        
        public class Factory : PlaceholderFactory<T>
        {
            public Factory(IObjectResolver resolver) : base(resolver)
            {
                
            }
        } 
    }
}