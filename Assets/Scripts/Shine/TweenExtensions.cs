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
            return TweenProperty
                (target, start, end, duration, easing, onComplete, (t, value) => target.position = value);
        }

        public static Tweener TweenScale(this Transform target, Vector3 start, Vector3 end, float duration, EaseType easing, Action onComplete = null)
        {
            return TweenProperty
                (target, start, end, duration, easing, onComplete, (t, value) => target.localScale = value);
        }

        public static Tweener TweenColor(this Renderer target, Color start, Color end, float duration, EaseType easing, Action onComplete = null)
        {
            return TweenProperty
                (target, start, end, duration, easing, onComplete, (t, value) => target.material.color = value);
        }

        private static Tweener TweenProperty<T>(Component target, T start, T end, float duration, EaseType easing,
            Action onComplete, Action<Tweener, T> updateAction)
        {
            return Tweener.Get
            (
                target,
                duration,
                Tweener.GetEasingFunction(easing),
                onComplete,
                (t) => updateAction(null, Lerp(start, end, t))
            );
        }

        private static T Lerp<T>(T start, T end, float t)
        {
            if (typeof(T) == typeof(Vector3))
                return (T)(object)Vector3.Lerp((Vector3)(object)start, (Vector3)(object)end, t);
            if (typeof(T) == typeof(Color))
                return (T)(object)Color.Lerp((Color)(object)start, (Color)(object)end, t);

            throw new InvalidOperationException($"Lerp is not supported for type {typeof(T)}");
        }
    }

    public abstract class Tween
    {
        public Component Target { get; protected set; }
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
        private Func<float, float> _easingFunction;
        private float _duration;
        public float Duration => _duration;
        private Action _onComplete;
        private float _elapsed;
        private bool _isPaused;
        private bool _isCanceled;

        private Tweener() { }

        public static Tweener Get(Component target, float duration, Func<float, float> easingFunction, Action onComplete, Action<float> onUpdate)
        {
            var tweener = Pool.Count > 0 ? Pool.Dequeue() : new Tweener();
            tweener.Initialize(target, duration, easingFunction, onComplete, onUpdate);
            return tweener;
        }

        private void Initialize(Component target, float duration, Func<float, float> easingFunction, Action onComplete, Action<float> onUpdate)
        {
            Target = target;
            _duration = duration;
            _easingFunction = easingFunction;
            _onComplete = onComplete;
            _onUpdate = onUpdate;
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
                    _onUpdate?.Invoke(_easingFunction(Mathf.Clamp01(_elapsed / _duration)));
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
            _onUpdate?.Invoke(_easingFunction(_elapsed / _duration));
        }

        private void ReleaseToPool()
        {
            _onUpdate = null;
            _onComplete = null;
            _easingFunction = null;
            Target = null;
            Pool.Enqueue(this);
        }

        private static readonly Dictionary<EaseType, Func<float, float>> EasingFunctions = new()
        {
            { EaseType.Linear, t => t },
            { EaseType.EaseInOutQuad, t => t < 0.5f ? 2 * t * t : 1 - Mathf.Pow(-2 * t + 2, 2) / 2 }
        };

        public static Func<float, float> GetEasingFunction(EaseType ease) => EasingFunctions[ease];
    }
    
    public class Sequence : Tween
    {
        private static readonly Queue<Sequence> Pool = new();
        internal readonly List<Tween> _tweens = new();
        private bool _isPaused;
        private bool _isCanceled;
        private float _elapsedTime;
        private int _currentTweenIndex;

        private Sequence() { }

        public static Sequence Get()
        {
            return Pool.Count > 0 ? Pool.Dequeue() : new Sequence();
        }

        public override async UniTask Play(CancellationToken cancellationToken = default)
        {
            _currentTweenIndex = 0;
            _elapsedTime = 0;

            while (_currentTweenIndex < _tweens.Count)
            {
                if (_isCanceled || cancellationToken.IsCancellationRequested)
                {
                    ReleaseToPool();
                    return;
                }

                if (!_isPaused)
                {
                    var tween = _tweens[_currentTweenIndex];
                    await tween.Play(cancellationToken);
                    _currentTweenIndex++;
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
            _elapsedTime = 0;
            _currentTweenIndex = 0;

            foreach (var tween in _tweens)
            {
                if (tween is Tweener t)
                {
                    if (_elapsedTime + t.Duration >= time)
                    {
                        t.JumpTo(time - _elapsedTime);
                        return;
                    }
                    _elapsedTime += t.Duration;
                    _currentTweenIndex++;
                }
            }
        }

        private void ReleaseToPool()
        {
            _tweens.Clear();
            _isPaused = false;
            _isCanceled = false;
            _currentTweenIndex = 0;
            Pool.Enqueue(this);
        }
    }
    
    public static class SequenceExtensions
    {
        public static Sequence Append(this Sequence sequence, Tween tween)
        {
            sequence._tweens.Add(tween);
            return sequence;
        }
    }
    
    public struct RemoteTweenData
    {
        public Vector3 Position;
        public float Time;
    }

    public static class RemoteTweenSystem
    {
        private static readonly Dictionary<string, ServerSyncedTween> ActiveRemoteTweens = new();

        public static void ListenToRemoteTween(string remoteObjectID, Action<RemoteTweenData> remoteTweenData)
        {
            // the external, provided method, maybe extern?
        }
        
        public static void RegisterToRemoteTween(Transform localTarget, string remoteObjectID)
        {
            if (!ActiveRemoteTweens.TryGetValue(remoteObjectID, out var tween))
            {
                tween = ServerSyncedTween.Get(localTarget);
                ActiveRemoteTweens[remoteObjectID] = tween;
                ListenToRemoteTween(remoteObjectID, tween.OnServerUpdate);
            }
        }
    }

    public class ServerSyncedTween : Tween
    {
        private static readonly Queue<ServerSyncedTween> Pool = new();
        private List<RemoteTweenData> _updates = new();
        private bool _isPaused;
        private bool _isCanceled;
        private float _startTime;

        private ServerSyncedTween() { }

        public static ServerSyncedTween Get(Transform target)
        {
            var tween = Pool.Count > 0 ? Pool.Dequeue() : new ServerSyncedTween();
            tween.Initialize(target);
            return tween;
        }

        public void OnServerUpdate(RemoteTweenData data)
        {
            _updates.Add(data);
        }
        
        private void Initialize(Transform target)
        {
            Target = target;
            _updates.Clear();
            _isPaused = false;
            _isCanceled = false;
            _startTime = Time.time;
        }

        public override async UniTask Play(CancellationToken cancellationToken = default)
        {
            while (!_isCanceled)
            {
                if (_updates.Count > 1)
                {
                    _updates.Sort((a, b) => a.Time.CompareTo(b.Time));
                }

                if (_updates.Count > 0 && !_isPaused)
                {
                    var latestData = _updates[^1];
                    Target.transform.position = Vector3.Lerp(Target.transform.position, latestData.Position, 0.1f);
                }

                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            }
            ReleaseToPool();
        }

        public override void Cancel() => _isCanceled = true;
        public override void Pause() => _isPaused = true;
        public override void Resume() => _isPaused = false;
        public override void JumpTo(float time)
        {
            if (_updates.Count == 0) return;

            _updates.Sort((a, b) => a.Time.CompareTo(b.Time));
            foreach (var data in _updates)
            {
                if (data.Time >= time)
                {
                    Target.transform.position = data.Position;
                    return;
                }
            }
        }

        private void ReleaseToPool()
        {
            _updates.Clear();
            _isPaused = false;
            _isCanceled = false;
            Pool.Enqueue(this);
        }
    }
}