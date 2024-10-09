using System;
using System.Collections.Generic;
using System.Linq;
using Audio;
using Solution.Command;
using Solution.Commands;
using Solution.Factory;
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
        private Action _leftDownClick;
        private Action _leftClick;
        private Action _leftUpClick;
        private Action _rightDownClick;
        private Action _rightClick;
        private Action _rightUpClick;

        // stateful vars
        private Vector2 _selectionStartPosition;
        private Vector2 _selectionEndPosition;
        private Rect _selectionRect;

        // factories
        private IFactory<MoveActorCommand> _moveActorCommandFactory;
        private IFactory<SetActorSelectedCommand> _setActorSelectedCommandFactory;
        private IFactory<SetActorsSelectedCommand> _setActorsSelectedCommandFactory;

        private CommandsQueue _playerCommandsQueue;
        
        [Inject]
        public void Construct(IAudioManager audioManager, 
            UnitSelectionBox unitSelectionBox,
            IFactory<MoveActorCommand> moveActorCommandFactory,
            IFactory<SetActorSelectedCommand> setActorSelectedCommandFactory,
            IFactory<SetActorsSelectedCommand> setActorsSelectedCommandFactory)
        {
            _audioManager = audioManager;
            _canvasSelectionBox = unitSelectionBox;
            _moveActorCommandFactory = moveActorCommandFactory;
            _setActorSelectedCommandFactory = setActorSelectedCommandFactory;
            _setActorsSelectedCommandFactory = setActorsSelectedCommandFactory;

            _playerCommandsQueue = new CommandsQueue(10);
        }

        private void Awake()
        {
            _canvasSelectionBox.gameObject.SetActive(true);
            SetCanvasSelectionBox(Vector2.zero, Vector2.zero);

            _actors = FindObjectsOfType<Actor>().ToList();
            MapControls();
        }

        private void MapControls()
        {
            _leftDownClick = OnSelectDownClick;
            _leftClick = OnSelectClick;
            _leftUpClick = OnSelectUpClick;

            _rightDownClick = OnMoveDownClick;
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
                _leftDownClick?.Invoke();
            }

            if (leftClicking)
            {
                _leftClick?.Invoke();
            }

            if (leftClickUp)
            {
                _leftUpClick?.Invoke();
            }

            if (rightClickDown)
            {
                _rightDownClick?.Invoke();
            }

            if (rightClicking)
            {
                _rightClick?.Invoke();
            }

            if (rightClickUp)
            {
                _rightUpClick?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.U))
            {
                _playerCommandsQueue.Undo();
            }
            if (Input.GetKeyDown(KeyCode.I))
            {
                _playerCommandsQueue.Redo();
            }
        }

        private void OnSelectDownClick()
        {
            _selectionStartPosition = Input.mousePosition;
        }

        private void OnSelectClick()
        {
            _selectionEndPosition = Input.mousePosition;
            _selectionRect = new Rect();
            DrawVisual();
            SetSelectionRect();
        }

        private void OnSelectUpClick()
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
        
        private void OnMoveDownClick()
        {
            var raycastResult = Raycast();
            if (!raycastResult.success)
                return;

            var destinationVector = raycastResult.hit.point;
            var selectedActors = _actors.Where(actor => actor.Selected);
            foreach (var selectedActor in selectedActors)
            {
                _moveActorCommandFactory.Create().AddToQueue(_playerCommandsQueue).Execute((selectedActor, destinationVector));
            }

            _audioManager.PlaySound(SoundType.Move);
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
                    _setActorsSelectedCommandFactory.Create().Execute((_actors, false));
                    _setActorSelectedCommandFactory.Create().Execute((selectedActor, true));
                }
            }
            else
            {
                _setActorsSelectedCommandFactory.Create().Execute((_actors, false));
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

            _setActorsSelectedCommandFactory.Create().Execute((selectedActors, true));
            _setActorsSelectedCommandFactory.Create().Execute((unselectedActors, false));
        }
    }
}