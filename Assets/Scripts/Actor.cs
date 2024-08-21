using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class Actor : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private NavMeshAgent _navMeshAgent;
    [SerializeField] private float _boredTimerSeconds;
    
    private CancellationTokenSource _boredCancellationToken = new();
    
    private static readonly int BoredParameter = Animator.StringToHash("Bored");
    private static readonly int SpeedParameter = Animator.StringToHash("Speed");
    
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
}
