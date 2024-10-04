using Solution.Command;
using UnityEngine;

namespace Solution.Commands
{
    public class MoveActorCommand : Command<MoveActorCommand, (Actor actor, Vector3 destination)>
    {
        private Actor _actor;
        private Vector3 _destination;
        private Vector3 _originalPosition;

        public override void Execute((Actor actor, Vector3 destination) arguments)
        {
            _actor = arguments.actor;
            _destination = arguments.destination;
            _originalPosition = _actor.transform.position;
            
            _actor.SetMovementDestination(arguments.destination);
        }
        
        public override void Undo()
        {
            _actor.Halt();
            _actor.transform.position = _originalPosition;
        }
        
        public override void Redo()
        {
            _actor.Halt();
            _actor.transform.position = _destination;
        }
    }
}