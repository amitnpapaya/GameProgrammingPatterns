// using System;
// using Audio;
// using UnityEngine;
//
// namespace Workshop.PlayerInput
// {
//     public partial class InputController : MonoBehaviour
//     {
//         private Actor _currentlySelectedActor;
//         
//         private Action _leftClick;
//         private Action _rightClick;
//
//         private void Awake()
//         {
//             MapControls();
//         }
//          
//         private void MapControls()
//         {
//             // TODO: READ FROM AN INPUT FILE / CONFIGURATION / USE NEW INPUT SYSTEM
//             _leftClick = Move;
//             _rightClick = SelectDeselect;
//         }
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
//                 _leftClick?.Invoke();
//             }
//         
//             else if (rightClick)
//             {
//                 _rightClick?.Invoke();
//             }
//         }
//         
//         private void SelectDeselect()
//         {
//             var raycastResult = Raycast();
//             if (!raycastResult.success)
//                 return;
//                 
//             if (raycastResult.hit.collider.gameObject.CompareTag("PlayerActor"))
//             {
//                 var selectedActor = raycastResult.hit.collider.gameObject.GetComponent<Actor>();
//                 if (selectedActor != null)
//                 {
//                     SelectActor(selectedActor);
//                 }
//             }
//             else
//             {
//                 UnselectActor();
//             }
//         }
//         
//         private void Move()
//         {
//             var raycastResult = Raycast();
//             if (!raycastResult.success)
//                 return;
//                 
//             var destinationVector = raycastResult.hit.point;
//             MoveActor(destinationVector);
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