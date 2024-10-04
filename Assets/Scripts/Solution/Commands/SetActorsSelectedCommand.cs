using System.Collections.Generic;
using Audio;
using Solution.Command;

namespace Solution.Commands
{
    public class SetActorsSelectedCommand : Command<SetActorsSelectedCommand, (IEnumerable<Actor> actors, bool selectedState)>
    {
        private readonly IAudioManager _audioManager;
        
        private IEnumerable<Actor> _actors;
        private bool _selectedState;

        public SetActorsSelectedCommand(IAudioManager audioManager)
        {
            _audioManager = audioManager;
        }

        public override void Execute((IEnumerable<Actor> actors, bool selectedState) arguments)
        {
            _actors = arguments.actors;
            _selectedState = arguments.selectedState;
            
            foreach (var actor in _actors)
            {
                actor.SetSelected(_selectedState);
            }

            if (_selectedState)
                _audioManager.PlaySound(SoundType.Select);
        }

        public override void Undo()
        {
            foreach (var actor in _actors)
            {
                actor.SetSelected(!_selectedState);
            }
        }

        public override void Redo()
        {
            foreach (var actor in _actors)
            {
                actor.SetSelected(_selectedState);
            }
        }
    }
}