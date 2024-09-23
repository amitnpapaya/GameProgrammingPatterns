// using Audio;
// using UnityEngine;
//
// namespace InputController
// {
//     public partial class InputController : MonoBehaviour
//     {
//         void Update()
//         {
//             var leftClick = Input.GetMouseButtonDown(0);
//             var rightClick = Input.GetMouseButtonDown(1);
//         
//             if (!leftClick && !rightClick) 
//                 return;
//         
//             var ray = _controlCamera.ScreenPointToRay(Input.mousePosition);
//             
//             if (!Physics.Raycast(ray, out var hit)) 
//                 return;
//     
//             if (leftClick)
//             {
//                 if (hit.collider.gameObject.CompareTag("PlayerActor"))
//                 {
//                     _actor.SetSelected(true);
//                     _audioManager.PlaySound(SoundType.Select);
//                 }
//                 else
//                 {
//                     _actor.SetSelected(false);
//                 }
//             }
//         
//             else if (rightClick)
//             {
//                 if (_actor.Selected)
//                 {
//                     var destinationVector = hit.point;
//                     _actor.SetMovementDestination(destinationVector);   
//                     _audioManager.PlaySound(SoundType.Move);
//                 }
//             }
//         }
//     }
// }