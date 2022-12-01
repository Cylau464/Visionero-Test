using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    [SerializeField] private float _minCameraDistance = 10f;
    [SerializeField] private float _maxCameraDistance = 30f;
    [SerializeField] private float _zoomSensitivity = 1f;
    [SerializeField] private string _orbitMoveInputAxis = "Mouse X";

    private CinemachineOrbitalTransposer _orbitalTransposer;

    private void Start()
    {
        _orbitalTransposer = _virtualCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
        _orbitalTransposer.m_XAxis.m_InputAxisName = "";
    }

    private void Update()
    {
        SwitchOrbitMove();
        Zoom(-Input.mouseScrollDelta.y * _zoomSensitivity);
    }

    private void SwitchOrbitMove()
    {
        if (Input.GetMouseButtonDown(1) == true)
            _orbitalTransposer.m_XAxis.m_InputAxisName = _orbitMoveInputAxis;

        if (Input.GetMouseButtonUp(1) == true)
        {
            _orbitalTransposer.m_XAxis.m_InputAxisName = "";
            _orbitalTransposer.m_XAxis.m_InputAxisValue = 0f;
        }
    }

    private void Zoom(float value)
    {
        Vector3 offset = _orbitalTransposer.m_FollowOffset;
        float distance = offset.y;
        distance = Mathf.Clamp(distance + value, _minCameraDistance, _maxCameraDistance);
        offset = new Vector3(offset.x, distance, distance);
        _orbitalTransposer.m_FollowOffset = offset;
    }
}