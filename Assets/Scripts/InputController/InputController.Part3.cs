using System;
using System.Collections.Generic;
using System.Linq;
using Audio;
using UnityEngine;

namespace InputController
{
    public partial class InputController : MonoBehaviour
    {
        private Action _leftClickCommand;
        private Action _rightClickCommand;
        
        // dependencies we should normally inject
        private UnitSelectionBox _canvasSelectionBox;
        private List<Actor> _actors;
        private Camera _camera;
        
        private Vector2 _selectionStartPosition;
        private Vector2 _selectionEndPosition;
        private Rect _selectionRect;

        private void Awake()
        {
            _camera = FindObjectOfType<Camera>();
            _canvasSelectionBox = FindObjectOfType<UnitSelectionBox>();
            _canvasSelectionBox.gameObject.SetActive(true);
            SetCanvasSelectionBox(Vector2.zero, Vector2.zero);
                
            _actors = FindObjectsOfType<Actor>().ToList();
            MapControls();
        }

        private void Start()
        {
            DrawVisual();
        }

        private void MapControls()
        {
            
        }
        
        void Update()
        {
            var leftClickDown = Input.GetMouseButtonDown(0);
            var leftClickUp = Input.GetMouseButtonUp(0);
            var leftClicking = Input.GetMouseButton(0);
            var rightClickDown = Input.GetMouseButtonDown(1);
            var rightClickUp = Input.GetMouseButtonUp(1);
            var rightClicking = Input.GetMouseButton(1);
        
            if (leftClickDown)
            {
                _selectionStartPosition = Input.mousePosition;
            }
            if (leftClicking)
            {
                _selectionEndPosition = Input.mousePosition;
                _selectionRect = new Rect();
                DrawVisual();
                SetSelectionRect();
            }
            if (leftClickUp)
            {
                SelectActors();
                _selectionStartPosition = Vector2.zero;
                _selectionEndPosition = Vector2.zero;
                DrawVisual();
            }
            
            if (rightClickDown)
            {
                
            }
            if (rightClicking)
            {
                
            }
            if (rightClickUp)
            {
                
            }
        }

        private void DrawVisual()
        {
            var boxCenter = (_selectionStartPosition + _selectionEndPosition) / 2;
            var boxSize = new Vector2(Mathf.Abs(_selectionStartPosition.x - _selectionEndPosition.x),
                Mathf.Abs(_selectionStartPosition.y - _selectionEndPosition.y));
            
            SetCanvasSelectionBox(boxCenter, boxSize);
        }

        private void SetCanvasSelectionBox(Vector2 boxCenter, Vector2 boxSize)
        {
            _canvasSelectionBox.RectTransform.position = boxCenter;
            _canvasSelectionBox.RectTransform.sizeDelta = boxSize;
        }

        private void SetSelectionRect()
        {
            if (Input.mousePosition.x < _selectionStartPosition.x)
            {
                _selectionRect.xMin = Input.mousePosition.x;
                _selectionRect.xMax = _selectionStartPosition.x;
            }
            else
            {
                _selectionRect.xMin = _selectionStartPosition.x;
                _selectionRect.xMax = Input.mousePosition.x;
            }
 
 
            if (Input.mousePosition.y < _selectionStartPosition.y)
            {
                _selectionRect.yMin = Input.mousePosition.y;
                _selectionRect.yMax = _selectionStartPosition.y;
            }
            else
            {
                _selectionRect.yMin = _selectionStartPosition.y;
                _selectionRect.yMax = Input.mousePosition.y;
            }
        }
        
        void SelectActors()
        {
            foreach (var actor in _actors)
            {
                if (_selectionRect.Contains(_camera.WorldToScreenPoint(actor.transform.position)))
                {
                    actor.SetSelected(true);
                    _audioManager.PlaySound(SoundType.Select);
                }
                else
                {
                    actor.SetSelected(false);
                }
            }
        }
    }
}