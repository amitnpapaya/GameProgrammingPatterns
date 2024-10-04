using Audio;
using Solution.Commands;
using Solution.Factory;
using Solution.PlayerInput;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Solution
{
    public class SolutionLifetimeScope : LifetimeScope
    {
        [SerializeField] private AudioManager _audioManager;
        [SerializeField] private InputController _inputController;
    
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent<IAudioManager>(_audioManager);
            builder.RegisterComponent(_inputController);
            
            builder.RegisterFactory<MoveActorCommand, MoveActorCommand.Factory>();
            builder.RegisterFactory<SetActorSelectedCommand, SetActorSelectedCommand.Factory>();
            builder.RegisterFactory<SetActorsSelectedCommand, SetActorsSelectedCommand.Factory>();
        }
    }
}