using System;
using UnityEngine;

public class UnitSelectionBox : MonoBehaviour
{
    private RectTransform _rectTransform;
    public RectTransform RectTransform => _rectTransform;
    
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }
}
