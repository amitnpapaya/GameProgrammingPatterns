using System;
using System.Collections.Generic;
using System.Linq;
using Audio;
using Factory;
using Solution.Commands;
using UnityEngine;
using VContainer;

namespace Solution.PlayerInput
{
    public class InputController : MonoBehaviour
    {
        [SerializeField] private Camera _controlCamera;
        private IAudioManager _audioManager;

        // dependencies we should normally inject
        private UnitSelectionBox _canvasSelectionBox;
        private List<Actor> _actors;

        // configuration
        private readonly float _minimalSelectBoxSize = 3f;

        // actions => command mapping
        private Action _leftDownClickCommand;
        private Action _leftClickCommand;
        private Action _leftUpClickCommand;
        private Action _rightDownClickCommand;
        private Action _rightClickCommand;
        private Action _rightUpClickCommand;

        // stateful vars
        private Vector2 _selectionStartPosition;
        private Vector2 _selectionEndPosition;
        private Rect _selectionRect;

        // factories
        private IFactory<MoveActorCommand> _moveActorCommandFactory;

        [Inject]
        public void Construct(IAudioManager audioManager, IFactory<MoveActorCommand> moveActorCommandFactory)
        {
            _audioManager = audioManager;
            _moveActorCommandFactory = moveActorCommandFactory;
        }

        private void Awake()
        {
            _canvasSelectionBox = FindObjectOfType<UnitSelectionBox>();
            _canvasSelectionBox.gameObject.SetActive(true);
            SetCanvasSelectionBox(Vector2.zero, Vector2.zero);

            _actors = FindObjectsOfType<Actor>().ToList();
            MapControls();
        }

        private void MapControls()
        {
            _leftDownClickCommand = SelectDownClickCommand;
            _leftClickCommand = SelectClickCommand;
            _leftUpClickCommand = SelectUpClickCommand;

            _rightDownClickCommand = MoveDownClickCommand;
        }

        private void Start()
        {
            DrawVisual();
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
                _leftDownClickCommand?.Invoke();
            }

            if (leftClicking)
            {
                _leftClickCommand?.Invoke();
            }

            if (leftClickUp)
            {
                _leftUpClickCommand?.Invoke();
            }

            if (rightClickDown)
            {
                _rightDownClickCommand?.Invoke();
            }

            if (rightClicking)
            {
                _rightClickCommand?.Invoke();
            }

            if (rightClickUp)
            {
                _rightUpClickCommand?.Invoke();
            }
        }

        private void SelectDownClickCommand()
        {
            _selectionStartPosition = Input.mousePosition;
        }

        private void SelectClickCommand()
        {
            _selectionEndPosition = Input.mousePosition;
            _selectionRect = new Rect();
            DrawVisual();
            SetSelectionRect();
        }

        private void SelectUpClickCommand()
        {
            if (_selectionRect.size.magnitude > _minimalSelectBoxSize)
            {
                SelectActorsInSelectionRect();
                _selectionStartPosition = Vector2.zero;
                _selectionEndPosition = Vector2.zero;
                DrawVisual();
            }
            else
            {
                SelectActorWithRaycast();
            }
        }

        private void SelectActorWithRaycast()
        {
            var raycastResult = Raycast();
            if (!raycastResult.success)
                return;

            if (raycastResult.hit.collider.gameObject.CompareTag("PlayerActor"))
            {
                var selectedActor = raycastResult.hit.collider.gameObject.GetComponent<Actor>();
                if (selectedActor != null)
                {
                    SetActorsSelectionState(_actors, false);
                    SetActorsSelectionState(selectedActor, true);
                }
            }
            else
            {
                SetActorsSelectionState(_actors, false);
            }
        }

        private (bool success, RaycastHit hit) Raycast()
        {
            var ray = _controlCamera.ScreenPointToRay(Input.mousePosition);
            var result = Physics.Raycast(ray, out var hit);
            return (result, hit);
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

        void SelectActorsInSelectionRect()
        {
            var selectedActors = _actors.Where(actor =>
                _selectionRect.Contains(_controlCamera.WorldToScreenPoint(actor.transform.position))).ToArray();
            var unselectedActors = _actors.Except(selectedActors);

            SetActorsSelectionState(selectedActors, true);
            SetActorsSelectionState(unselectedActors, false);
        }

        private void SetActorsSelectionState(Actor actor, bool selectionState)
        {
            actor.SetSelected(selectionState);
            if (selectionState)
                _audioManager.PlaySound(SoundType.Select);
        }

        private void SetActorsSelectionState(IEnumerable<Actor> actors, bool selectionState)
        {
            foreach (var actor in actors)
            {
                actor.SetSelected(selectionState);
            }

            if (selectionState)
                _audioManager.PlaySound(SoundType.Select);
        }

        private void MoveDownClickCommand()
        {
            var raycastResult = Raycast();
            if (!raycastResult.success)
                return;

            var destinationVector = raycastResult.hit.point;
            var selectedActors = _actors.Where(actor => actor.Selected);
            foreach (var selectedActor in selectedActors)
            {
                var command = _moveActorCommandFactory.Create();
                command.Execute((selectedActor, destinationVector));
            }

            _audioManager.PlaySound(SoundType.Move);
        }
    }
}