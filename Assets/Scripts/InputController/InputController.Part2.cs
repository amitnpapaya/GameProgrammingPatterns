using System;
using Audio;
using UnityEngine;

namespace InputController
{
    public partial class InputController : MonoBehaviour
    {
        private Action _leftClickCommand;
        private Action _rightClickCommand;

        private void Awake()
        {
            MapControls();
        }
         
        private void MapControls()
        {
            // TODO: READ FROM AN INPUT FILE / CONFIGURATION / USE NEW INPUT SYSTEM
            _leftClickCommand = MoveCommand;
            _rightClickCommand = SelectDeselectCommand;
        }
        
        void Update()
        {
            var leftClick = Input.GetMouseButtonDown(0);
            var rightClick = Input.GetMouseButtonDown(1);
        
            if (!leftClick && !rightClick) 
                return;
            
            if (leftClick)
            {
                _leftClickCommand?.Invoke();
            }
        
            else if (rightClick)
            {
                _rightClickCommand?.Invoke();
            }
        }
        
        private void SelectDeselectCommand()
        {
            var raycastResult = Raycast();
            if (!raycastResult.success)
                return;
                
            if (raycastResult.hit.collider.gameObject.CompareTag("PlayerActor"))
            {
                var selectedActor = raycastResult.hit.collider.gameObject.GetComponent<Actor>();
                if (selectedActor != null)
                {
                    SelectActor(selectedActor);
                }
            }
            else
            {
                UnselectActor();
            }
        }
        
        private void MoveCommand()
        {
            var raycastResult = Raycast();
            if (!raycastResult.success)
                return;
                
            var destinationVector = raycastResult.hit.point;
            MoveActor(destinationVector);
        }

        private (bool success, RaycastHit hit) Raycast()
        {
            var ray = _controlCamera.ScreenPointToRay(Input.mousePosition);
            var result = Physics.Raycast(ray, out var hit);
            return (result, hit);
        }

        private void SelectActor(Actor selectedActor)
        {
            _currentlySelectedActor = selectedActor;
            _currentlySelectedActor.SetSelected(true);
            _audioManager.PlaySound(SoundType.Select);
        }
        
        private void UnselectActor()
        {
            if (_currentlySelectedActor == null) return;
            _currentlySelectedActor.SetSelected(false);
            _currentlySelectedActor = null;
        }
        
        private void MoveActor(Vector3 destination)
        {
            if (_currentlySelectedActor != null)
            {
                _currentlySelectedActor.SetMovementDestination(destination);   
                _audioManager.PlaySound(SoundType.Move);
            }
        }
    }
}