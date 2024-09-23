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
//                     var selectedActor = hit.collider.gameObject.GetComponent<Actor>();
//                     if (selectedActor != null)
//                     {
//                         _currentlySelectedActor = selectedActor;
//                         _currentlySelectedActor.SetSelected(true);
//                         _audioManager.PlaySound(SoundType.Select);   
//                     }
//                 }
//                 else
//                 {
//                     if (_currentlySelectedActor != null)
//                     {
//                         _currentlySelectedActor.SetSelected(false);
//                         _currentlySelectedActor = null;
//                     }
//                 }
//             }
//         
//             else if (rightClick)
//             {
//                 if (_currentlySelectedActor != null)
//                 {
//                     var destinationVector = hit.point;
//                     _currentlySelectedActor.SetMovementDestination(destinationVector);   
//                     _audioManager.PlaySound(SoundType.Move);
//                 }
//             }
//         }
//     }
// }