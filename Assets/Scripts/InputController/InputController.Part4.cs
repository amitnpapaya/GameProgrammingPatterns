using System;
using System.Collections.Generic;
using System.Linq;
using Audio;
using UnityEngine;

namespace InputController
{
    // public partial class InputController : MonoBehaviour
    // {
    //     
    // }
}


//
// public interface IVoidCommand<TArg>
// {
//     void Execute(TArg arguments);
// }
//
// public interface ICommand<TRes>
// {
//     TRes Execute();
// }
//
// public interface ICommand<TArg, TRes>
// {
//     TRes Execute(TArg arguments);
// }
//
// public class SetActorSelectedStateCommand : IVoidCommand<(Actor actor, bool state)>
// {
//     public void Execute((Actor actor, bool state) arguments)
//     {
//         arguments.actor.SetSelected(arguments.state);
//         if (selectionState)
//             _audioManager.PlaySound(SoundType.Select);
//     }
// }