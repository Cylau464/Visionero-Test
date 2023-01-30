using UnityEngine;
using Cinemachine;
using NaughtyAttributes;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;

    [Header("Horizontal Move")]
    [SerializeField] private string _horizontalMoveInputAxis = "Mouse X";
    [SerializeField] private float _horizontalSensitivity = 400f;

    [Header("Vertical Move")]
    [SerializeField, MinMaxSlider(0f, 100f)] private Vector2 _verticalMinMax;
    [SerializeField] private string _verticalMoveInputAxis = "Mouse Y";
    [SerializeField] private float _verticalSensitivity = 400f;
    private bool _verticalMoveEnable;
    
    [Header("Zoom")]
    [SerializeField, MinMaxSlider(0f, 100f)] private Vector2 _zoomMinMax;
    [SerializeField] private float _zoomSensitivity = 1f;

    private CinemachineOrbitalTransposer _orbitalTransposer;
    private CinemachinePOV _POV;
    private CinemachineFramingTransposer _framingTransposer;

    private void Start()
    {
        //_orbitalTransposer = _virtualCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
        _framingTransposer = _virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        _POV = _virtualCamera.GetCinemachineComponent<CinemachinePOV>();
        //_orbitalTransposer.m_XAxis.m_InputAxisName = "";
        _POV.m_VerticalAxis.m_InputAxisName = "";
        _POV.m_HorizontalAxis.m_InputAxisName = "";
        _POV.m_VerticalAxis.m_MinValue = _verticalMinMax.x;
        _POV.m_VerticalAxis.m_MaxValue = _verticalMinMax.y;
    }

    private void Update()
    {
        SwitchOrbitMove();
        Zoom(-Input.mouseScrollDelta.y * _zoomSensitivity * Time.deltaTime);
    }

    private void SwitchOrbitMove()
    {
        if (Input.GetMouseButtonDown(1) == true)
        {
            //_orbitalTransposer.m_XAxis.m_MaxSpeed = _orbitalSensitivity;
            //_orbitalTransposer.m_XAxis.m_InputAxisName = _orbitMoveInputAxis;

            _POV.m_HorizontalAxis.m_MaxSpeed = _horizontalSensitivity;
            _POV.m_HorizontalAxis.m_InputAxisName = _horizontalMoveInputAxis;

        }

        if (Input.GetMouseButtonUp(1) == true)
        {
            //_orbitalTransposer.m_XAxis.m_InputAxisName = "";
            //_orbitalTransposer.m_XAxis.m_InputAxisValue = 0f;

            _POV.m_HorizontalAxis.m_InputAxisName = "";
            _POV.m_HorizontalAxis.m_InputAxisValue = 0f;
        }

        if (Input.GetMouseButtonDown(2) == true)
        {
            _POV.m_VerticalAxis.m_MaxSpeed = _verticalSensitivity;
            _POV.m_VerticalAxis.m_InputAxisName = _verticalMoveInputAxis;
        }

        if (Input.GetMouseButtonUp(2) == true)
        {
            _POV.m_VerticalAxis.m_InputAxisName = "";
            _POV.m_VerticalAxis.m_InputAxisValue = 0f;
        }
    }

    private void Zoom(float value)
    {
        float distance = Mathf.Clamp(_framingTransposer.m_CameraDistance + value, _zoomMinMax.x, _zoomMinMax.y);
        _framingTransposer.m_CameraDistance = distance;
        //Vector3 offset = _orbitalTransposer.m_FollowOffset;
        //offset.y = Mathf.Clamp(offset.y + value, _zoomMinMax.x, _zoomMinMax.y);
        //offset.z = Mathf.Clamp(offset.z + value, _zoomMinMax.x, _zoomMinMax.y);
        //_orbitalTransposer.m_FollowOffset = offset;
    }

    //private void VerticalMove()
    //{
    //    if (_verticalMoveEnable == false) return;

    //    float input = Input.GetAxis("Mouse Y");
    //    Vector3 newOffset = _orbitalTransposer.m_FollowOffset + (Vector3.up * _verticalSensitivity * input * Time.deltaTime);
    //    newOffset.y = Mathf.Clamp(newOffset.y, _verticalMinMax.x, _verticalMinMax.y);
    //    _orbitalTransposer.m_FollowOffset = newOffset;
    //}
}