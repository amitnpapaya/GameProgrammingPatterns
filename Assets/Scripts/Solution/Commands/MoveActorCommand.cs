using Command;
using UnityEngine;

namespace Solution.Commands
{
    public class MoveActorCommand : Command<MoveActorCommand, (Actor actor, Vector3 destination)>
    {
        public override void Execute((Actor actor, Vector3 destination) arguments)
        {
            arguments.actor.SetMovementDestination(arguments.destination);
        }
    }
}