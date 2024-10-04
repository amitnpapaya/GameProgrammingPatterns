using Audio;
using Factory;
using Solution.Commands;
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
        }
    }
}