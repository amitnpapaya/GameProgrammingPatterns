using Audio;
using UnityEngine;
using VContainer;

namespace InputController
{
    public partial class InputController : MonoBehaviour
    {
        [SerializeField] private Camera _controlCamera;
        [SerializeField] private Actor _actor;
        private IAudioManager _audioManager;

        [Inject]
        public void Construct(IAudioManager audioManager)
        {
            _audioManager = audioManager;
        }
    }
}