using Audio;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Workshop.PlayerInput;

namespace Workshop
{
    public class WorkshopLifetimeScope : LifetimeScope
    {
        [SerializeField] private AudioManager _audioManager;
        [SerializeField] private InputController _inputController;
    
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent<IAudioManager>(_audioManager);
            builder.RegisterComponent(_inputController);
        }
    }
}