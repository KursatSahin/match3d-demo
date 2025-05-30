using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenFitter : MonoBehaviour
{
    #region Inspector

    [SerializeField] private Camera _camera;
    [SerializeField] private float _referenceWidth;
    [SerializeField] private float _referenceHeight;

    #endregion

    private void Start()
    {
        FitGround();
    }

    private void FitGround()
    {
        var targetAspect = (float)Screen.width / Screen.height;
        var referenceAspect = _referenceWidth / _referenceHeight;

        var camHeight = _camera.orthographicSize * 2f;
        var camWidth = camHeight * targetAspect;

        var scale = transform.localScale;

        if (targetAspect >= referenceAspect)
        {
            scale.x = camWidth;
            scale.y = camWidth * (_referenceHeight / _referenceWidth);
        }
        else
        {
            scale.y = camHeight;
            scale.x = camHeight * (_referenceWidth / _referenceHeight);
        }

        transform.localScale = scale;
    }
}
