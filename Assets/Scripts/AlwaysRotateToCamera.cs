using UnityEngine;

public class AlwaysRotateToCamera : MonoBehaviour
{
    [SerializeField] private float _zRotationOffset;

    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(_camera.transform.forward, _camera.transform.up);
        transform.rotation *= Quaternion.Euler(0f, 0f, _zRotationOffset);
    }
}