using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleTargetCamera : MonoBehaviour
{
    [Header("Camera & Targets")]
    [SerializeField] private Camera _mainCam;
    public Camera MainCam => _mainCam;

    [SerializeField] private List<Transform> _targets = new();
    public List<Transform> Targets => _targets;

    [Header("Offset")]
    [SerializeField] private Vector3 _offset = new (0.0f, -12.0f, -0.35f);

    [Header("Details")]
    [SerializeField] private float _smoothTime = 0.5f;
    [SerializeField] private float _minZoom = 40.0f, _maxZoom = 10.0f, _zoomTime = 20.0f;

    private Vector3 _camVelocity;
    private float _greatestDistanceBetweenPlayers;
    private float _minYPos, _maxYPos;

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

    private void UpdateMinMaxY()
    {
        float cameraHeight = _mainCam.orthographicSize;
        _minYPos = cameraHeight;
        _maxYPos = Screen.height - cameraHeight;
    }
    private void MoveSmoothly()
    {
        Vector3 newPos = GetCenterPoint() + _offset;

        // clamp min and max Y
        _minYPos = newPos.y - _mainCam.orthographicSize;
        _maxYPos = newPos.y + _mainCam.orthographicSize;

        //UpdateMinMaxY();

        newPos.y = Mathf.Max(newPos.y, _minYPos, _maxYPos);
        transform.position = Vector3.SmoothDamp(transform.position, newPos, ref _camVelocity, _smoothTime);
    }
    private void Zoom()
    {
        float newZoom = Mathf.Lerp(_maxZoom, _minZoom, _greatestDistanceBetweenPlayers / _zoomTime);
        _mainCam.orthographicSize = Mathf.Lerp(_mainCam.orthographicSize, newZoom, Time.deltaTime);
    }
}
