using Audio;
using Solution.Command;

namespace Solution.Commands
{
    public class SetActorSelectedCommand : Command<SetActorSelectedCommand, (Actor actor, bool selectedState)>
    {
        private readonly IAudioManager _audioManager;

        public SetActorSelectedCommand(IAudioManager audioManager)
        {
            _audioManager = audioManager;
        }
        
        public override void Execute((Actor actor, bool selectedState) arguments)
        {
            arguments.actor.SetSelected(arguments.selectedState);
            if (arguments.selectedState)
                _audioManager.PlaySound(SoundType.Select);
        }
    }
}