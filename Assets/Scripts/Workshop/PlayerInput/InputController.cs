using Audio;
using UnityEngine;
using VContainer;

namespace Workshop.PlayerInput
{
    public partial class InputController : MonoBehaviour
    {
        [SerializeField] private Camera _controlCamera;
        private IAudioManager _audioManager;
        private UnitSelectionBox _canvasSelectionBox;
        
        [Inject]
        public void Construct(IAudioManager audioManager, UnitSelectionBox unitSelectionBox)
        {
            _audioManager = audioManager;
            _canvasSelectionBox = unitSelectionBox;
        }
    }
}