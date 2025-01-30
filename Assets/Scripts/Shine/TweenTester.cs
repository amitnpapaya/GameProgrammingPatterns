using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Shine
{
    public class TweenTester : MonoBehaviour
    {
        [SerializeField] private GameObject _gameObject;
        [SerializeField] private Renderer _gameObjectRenderer;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var sequence = Sequence.Get();
                sequence.Append(_gameObject.transform.TweenMove(Vector3.zero, Vector3.one, 1f, EaseType.EaseInOutQuad));
                sequence.Append(_gameObject.transform.TweenMove(Vector3.one, Vector3.zero, 2f, EaseType.Linear));
                sequence.Append(_gameObjectRenderer.TweenColor(Color.white, Color.red, 0.5f, EaseType.Linear));
                sequence.Play().Forget();
            }
        }
    }
}