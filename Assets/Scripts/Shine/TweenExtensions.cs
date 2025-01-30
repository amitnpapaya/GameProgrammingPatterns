using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Shine
{
    public enum EaseType
    {
        Linear,
        EaseInOutQuad
    }

    public static class TweenExtensions
    {
        public static Tweener TweenMove(this Transform target, Vector3 start, Vector3 end, float duration, EaseType easing, Action onComplete = null)
        {
            return Tweener.Get(target.gameObject, t => target.position = Vector3.Lerp(start, end, Tweener.GetEasingFunction(easing)(t)), duration, onComplete);
        }

        public static Tweener TweenScale(this Transform target, Vector3 start, Vector3 end, float duration, EaseType easing, Action onComplete = null)
        {
            return Tweener.Get(target.gameObject, t => target.localScale = Vector3.Lerp(start, end, Tweener.GetEasingFunction(easing)(t)), duration, onComplete);
        }

        public static Tweener TweenColor(this Renderer target, Color start, Color end, float duration, EaseType easing, Action onComplete = null)
        {
            return Tweener.Get(target.gameObject, t => target.material.color = Color.Lerp(start, end, Tweener.GetEasingFunction(easing)(t)), duration, onComplete);
        }
    }

    public abstract class Tween
    {
        public GameObject Target { get; protected set; }
        public abstract UniTask Play(CancellationToken cancellationToken = default);
        public abstract void Cancel();
        public abstract void Pause();
        public abstract void Resume();
        public abstract void JumpTo(float time);
    }

    public class Tweener : Tween
    {
        private static readonly Queue<Tweener> Pool = new();

        private Action<float> _onUpdate;
        private float _duration;
        public float Duration => _duration;
        private Action _onComplete;
        private float _elapsed;
        private bool _isPaused;
        private bool _isCanceled;

        private Tweener() { }
        
        public static Tweener Get(GameObject target, Action<float> onUpdate, float duration, Action onComplete)
        {
            var tweener = Pool.Count > 0 ? Pool.Dequeue() : new Tweener();
            tweener.Initialize(target, onUpdate, duration, onComplete);
            return tweener;
        }

        private void Initialize(GameObject target, Action<float> onUpdate, float duration, Action onComplete)
        {
            Target = target;
            _onUpdate = onUpdate;
            _duration = duration;
            _onComplete = onComplete;
            _elapsed = 0;
            _isPaused = false;
            _isCanceled = false;
        }

        public override async UniTask Play(CancellationToken cancellationToken = default)
        {
            while (_elapsed < _duration)
            {
                if (_isCanceled || cancellationToken.IsCancellationRequested)
                {
                    ReleaseToPool();
                    return;
                }
                if (!_isPaused)
                {
                    _elapsed += Time.deltaTime;
                    _onUpdate?.Invoke(Mathf.Clamp01(_elapsed / _duration));
                }
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            }
            _onComplete?.Invoke();
            ReleaseToPool();
        }

        public override void Cancel() => _isCanceled = true;
        public override void Pause() => _isPaused = true;
        public override void Resume() => _isPaused = false;

        public override void JumpTo(float time)
        {
            _elapsed = Mathf.Clamp(time, 0, _duration);
            _onUpdate?.Invoke(_elapsed / _duration);
        }

        private void ReleaseToPool()
        {
            _onUpdate = null;
            _onComplete = null;
            Target = null;
            Pool.Enqueue(this);
        }
        
        public static Func<float, float> GetEasingFunction(EaseType ease)
        {
            return ease switch
            {
                EaseType.Linear => t => t,
                EaseType.EaseInOutQuad => t => t < 0.5f ? 2 * t * t : 1 - Mathf.Pow(-2 * t + 2, 2) / 2,
                _ => t => t
            };
        }
    }

    public class Sequence : Tween
    {
        private static readonly Queue<Sequence> Pool = new();
        private readonly Queue<Tween> _tweens = new();
        private bool _isPaused;
        private bool _isCanceled;
        private float _totalDuration;

        private Sequence() { }

        public static Sequence Get()
        {
            return Pool.Count > 0 ? Pool.Dequeue() : new Sequence();
        }

        public Sequence Append(Tween tween)
        {
            _tweens.Enqueue(tween);
            _totalDuration += tween is Tweener t ? t.Duration : 0;
            return this;
        }

        public override async UniTask Play(CancellationToken cancellationToken = default)
        {
            while (_tweens.Count > 0)
            {
                if (_isCanceled || cancellationToken.IsCancellationRequested)
                {
                    ReleaseToPool();
                    return;
                }
                if (!_isPaused)
                {
                    var tween = _tweens.Dequeue();
                    await tween.Play(cancellationToken);
                }
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            }
            ReleaseToPool();
        }

        public override void Cancel()
        {
            _isCanceled = true;
            foreach (var tween in _tweens)
                tween.Cancel();
        }

        public override void Pause() => _isPaused = true;
        public override void Resume() => _isPaused = false;

        public override void JumpTo(float time)
        {
            float elapsedTime = 0;
            foreach (var tween in _tweens)
            {
                if (tween is Tweener t)
                {
                    if (elapsedTime + t.Duration >= time)
                    {
                        t.JumpTo(time - elapsedTime);
                        return;
                    }
                    elapsedTime += t.Duration;
                }
            }
        }

        private void ReleaseToPool()
        {
            _tweens.Clear();
            _isPaused = false;
            _isCanceled = false;
            Pool.Enqueue(this);
        }
    }
}
