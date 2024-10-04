using Audio;
using Solution.Command;

namespace Solution.Commands
{
    public class SetActorSelectedCommand : Command<SetActorSelectedCommand, (Actor actor, bool selectedState)>
    {
        private readonly IAudioManager _audioManager;
        
        private Actor _actor;
        private bool _selectedState;
        private bool _originalSelectedState;

        public SetActorSelectedCommand(IAudioManager audioManager)
        {
            _audioManager = audioManager;
        }
        
        public override void Execute((Actor actor, bool selectedState) arguments)
        {
            _actor = arguments.actor;
            _selectedState = arguments.selectedState;
            _originalSelectedState = _actor.Selected;
            
            _actor.SetSelected(_selectedState);
            if (_selectedState)
                _audioManager.PlaySound(SoundType.Select);
        }

        public override void Undo()
        {
            _actor.SetSelected(_originalSelectedState);
        }

        public override void Redo()
        {
            _actor.SetSelected(_selectedState);
        }
    }
}