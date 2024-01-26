using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleTargetCamera : MonoBehaviour
{
    [SerializeField] private Camera _mainCam;
    [SerializeField] private List<Transform> _targets;
    [SerializeField] private Vector3 _offset = Vector3.zero;
    [SerializeField] private float _cameraYPos = 0.0f;
    [SerializeField] private float _smoothTime = 0.5f;
    [SerializeField] private float _minZoom = 40.0f, _maxZoom = 10.0f, _zoomTime = 20.0f;

    private Vector3 _velocity;
    private float _greatestDistanceBetweenPlayers;

    private void LateUpdate()
    {
        if (_targets.Count == 0)
            return;

        MoveSmoothly();
        Zoom();
    }

    private Vector3 GetCenterPoint()
    {
        if (_targets.Count == 1)
            return _targets[0].position;

        Bounds bounds = new Bounds(_targets[0].position, Vector3.zero);

        for (int i = 0; i < _targets.Count; i++)
            bounds.Encapsulate(_targets[i].position);

        _greatestDistanceBetweenPlayers = bounds.size.x;
        return bounds.center;
    }

    private void MoveSmoothly()
    {
        Vector3 centerPoint = GetCenterPoint();
        centerPoint.y = _cameraYPos;
        Vector3 newPos = centerPoint + _offset;
        transform.position = Vector3.SmoothDamp(transform.position, newPos, ref _velocity, _smoothTime);
    }
    private void Zoom()
    {
        float newZoom = Mathf.Lerp(_maxZoom, _minZoom, _greatestDistanceBetweenPlayers / _zoomTime);
        _mainCam.orthographicSize = Mathf.Lerp(_mainCam.orthographicSize, newZoom, Time.deltaTime);
    }
}
