using UnityEngine;
using Cinemachine;
using NaughtyAttributes;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    
    [Header("Orbital Move")]
    [SerializeField] private string _orbitMoveInputAxis = "Mouse X";
    [SerializeField] private float _orbitalSensitivity = 400f;

    [Header("Vertical Move")]
    [SerializeField, MinMaxSlider(0f, 100f)] private Vector2 _verticalMinMax;
    [SerializeField] private float _verticalSensitivity = 1f;
    private bool _verticalMoveEnable;
    
    [Header("Zoom")]
    [SerializeField, MinMaxSlider(0f, 100f)] private Vector2 _zoomMinMax;
    [SerializeField] private float _zoomSensitivity = 1f;

    private CinemachineOrbitalTransposer _orbitalTransposer;

    private void Start()
    {
        _orbitalTransposer = _virtualCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
        _orbitalTransposer.m_XAxis.m_InputAxisName = "";
    }

    private void Update()
    {
        SwitchOrbitMove();
        Zoom(-Input.mouseScrollDelta.y * _zoomSensitivity * Time.deltaTime);
        VerticalMove();
    }

    private void SwitchOrbitMove()
    {
        if (Input.GetMouseButtonDown(1) == true)
        {
            _orbitalTransposer.m_XAxis.m_MaxSpeed = _orbitalSensitivity;
            _orbitalTransposer.m_XAxis.m_InputAxisName = _orbitMoveInputAxis;
        }

        if (Input.GetMouseButtonUp(1) == true)
        {
            _orbitalTransposer.m_XAxis.m_InputAxisName = "";
            _orbitalTransposer.m_XAxis.m_InputAxisValue = 0f;
        }

        if (Input.GetMouseButtonDown(2) == true)
            _verticalMoveEnable = true;

        if (Input.GetMouseButtonUp(2) == true)
            _verticalMoveEnable = false;
    }

    private void Zoom(float value)
    {
        Vector3 offset = _orbitalTransposer.m_FollowOffset;
        offset.y = Mathf.Clamp(offset.y + value, _zoomMinMax.x, _zoomMinMax.y);
        offset.z = Mathf.Clamp(offset.z + value, _zoomMinMax.x, _zoomMinMax.y);
        _orbitalTransposer.m_FollowOffset = offset;
    }

    private void VerticalMove()
    {
        if (_verticalMoveEnable == false) return;

        float input = Input.GetAxis("Mouse Y");
        Vector3 newOffset = _orbitalTransposer.m_FollowOffset + (Vector3.up * _verticalSensitivity * input * Time.deltaTime);
        newOffset.y = Mathf.Clamp(newOffset.y, _verticalMinMax.x, _verticalMinMax.y);
        _orbitalTransposer.m_FollowOffset = newOffset;
    }
}