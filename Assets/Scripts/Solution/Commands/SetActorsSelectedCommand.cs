using System.Collections.Generic;
using Audio;
using Solution.Command;

namespace Solution.Commands
{
    public class SetActorsSelectedCommand : Command<SetActorsSelectedCommand, (IEnumerable<Actor> actors, bool selectedState)>
    {
        private readonly IAudioManager _audioManager;

        public SetActorsSelectedCommand(IAudioManager audioManager)
        {
            _audioManager = audioManager;
        }

        public override void Execute((IEnumerable<Actor> actors, bool selectedState) arguments)
        {
            foreach (var actor in arguments.actors)
            {
                actor.SetSelected(arguments.selectedState);
            }

            if (arguments.selectedState)
                _audioManager.PlaySound(SoundType.Select);
        }
    }
}