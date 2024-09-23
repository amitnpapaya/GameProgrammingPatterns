using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class Actor : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private NavMeshAgent _navMeshAgent;
    [SerializeField] private float _boredTimerSeconds;
    [FormerlySerializedAs("_selection")] [SerializeField] private SelectionIndicator _selectionIndicator;

    private CancellationTokenSource _boredCancellationToken;    
    
    private static readonly int BoredParameter = Animator.StringToHash("Bored");
    private static readonly int SpeedParameter = Animator.StringToHash("Speed");
    
    private bool _selected;
    public bool Selected => _selected;

    private void Awake()
    {
        SetSelected(false);
    }

    public void SetSelected(bool selected)
    {
        _selected = selected;
        _selectionIndicator.gameObject.SetActive(selected);
    }

    private void Update()
    {
        HandleAnimations();
    }

    private void HandleAnimations()
    {
        HandleBored();
        HandleSpeed();
    }

    private void HandleSpeed()
    {
        _animator.SetFloat(SpeedParameter, _navMeshAgent.velocity.magnitude);
    }

    private void HandleBored()
    {
        var shouldCancelBoredState = _navMeshAgent.velocity.magnitude > 0 && _boredCancellationToken != null;
        if (shouldCancelBoredState)
        {
            _boredCancellationToken.Cancel();
            _boredCancellationToken = null;
            _animator.SetBool(BoredParameter, false);
            return;
        }
        
        var shouldEnterBoredState = _navMeshAgent.velocity.magnitude == 0 && _boredCancellationToken == null;
        if (shouldEnterBoredState)
        {
            _boredCancellationToken = new CancellationTokenSource();
            StartBoredTimer().Forget();
            return;
        }
    }
    
    private async UniTask StartBoredTimer()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_boredTimerSeconds), cancellationToken: _boredCancellationToken.Token);
        _animator.SetBool(BoredParameter, true);
    }

    public void SetMovementDestination(Vector3 destination)
    {
        _navMeshAgent.SetDestination(destination);
    }
}
