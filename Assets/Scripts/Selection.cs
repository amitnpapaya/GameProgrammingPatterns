using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Selection : MonoBehaviour
{
    [SerializeField] private Image _selection;
    private Transform _selectionTransform;
    public float _rotationSpeed = 3f;

    private void Awake()
    {
        _selectionTransform = _selection.transform;
    }

    void Start()
    {
        RotateContinuously();
    }

    void RotateContinuously()
    {
        _selectionTransform.DORotate(new Vector3(90, 0, 360), _rotationSpeed, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Incremental);
    }
}
