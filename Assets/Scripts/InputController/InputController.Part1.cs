// using Audio;
// using UnityEngine;
//
// namespace InputController
// {
//     public partial class InputController : MonoBehaviour
//     {
//         private Actor _currentlySelectedActor;
//         
//         void Update()
//         {
//             var leftClick = Input.GetMouseButtonDown(0);
//             var rightClick = Input.GetMouseButtonDown(1);
//         
//             if (!leftClick && !rightClick) 
//                 return;
//             
//             if (leftClick)
//             {
//                 var raycastResult = Raycast();
//                 if (!raycastResult.success)
//                     return;
//                 
//                 if (raycastResult.hit.collider.gameObject.CompareTag("PlayerActor"))
//                 {
//                     var selectedActor = raycastResult.hit.collider.gameObject.GetComponent<Actor>();
//                     if (selectedActor != null)
//                     {
//                         SelectActor(selectedActor);
//                     }
//                 }
//                 else
//                 {
//                     UnselectActor();
//                 }
//             }
//         
//             else if (rightClick)
//             {
//                 var raycastResult = Raycast();
//                 if (!raycastResult.success)
//                     return;
//                 
//                 var destinationVector = raycastResult.hit.point;
//                 MoveActor(destinationVector);
//             }
//         }
//
//         private (bool success, RaycastHit hit) Raycast()
//         {
//             var ray = _controlCamera.ScreenPointToRay(Input.mousePosition);
//             var result = Physics.Raycast(ray, out var hit);
//             return (result, hit);
//         }
//
//         private void SelectActor(Actor selectedActor)
//         {
//             _currentlySelectedActor = selectedActor;
//             _currentlySelectedActor.SetSelected(true);
//             _audioManager.PlaySound(SoundType.Select);
//         }
//         
//         private void UnselectActor()
//         {
//             if (_currentlySelectedActor == null) return;
//             _currentlySelectedActor.SetSelected(false);
//             _currentlySelectedActor = null;
//         }
//         
//         private void MoveActor(Vector3 destination)
//         {
//             if (_currentlySelectedActor != null)
//             {
//                 _currentlySelectedActor.SetMovementDestination(destination);   
//                 _audioManager.PlaySound(SoundType.Move);
//             }
//         }
//     }
// }