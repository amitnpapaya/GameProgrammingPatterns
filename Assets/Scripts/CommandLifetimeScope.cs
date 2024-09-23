using Audio;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class CommandLifetimeScope : LifetimeScope
{
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private InputController.InputController _inputController;
    
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent<IAudioManager>(_audioManager);
        builder.RegisterComponent(_inputController);
    }
}
