using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private static CameraManager _instance;
    public static CameraManager Instance {
        get { return _instance; }
        set { if (_instance == null) _instance = value; else { value.enabled = false; } } 
    }

    private Transform _target;
    private Vector3 _offset = new Vector3(0,1.5f,-4f);
    private Vector3 _nextPosition;
    private Vector3 _currentVelocity;
    private Quaternion _nextRotation;
    private float _smoothDamp = 0.4f;
    [SerializeField]
    private float _turnSpeedRadius = 900f;

    private void Awake()
    {
        Instance = this;
        _nextPosition = transform.position;
        _nextRotation = transform.rotation;
    }

    public static void SetTarget(Transform target)
    {
        if (Instance == null) return;

        Instance._target = target;
        Instance.transform.position = target.position;
    }

    private void Update()
    {
        if (_target == null) return;

        AboveCameraMovement();
    }

    private void AboveCameraMovement()
    {
        _nextPosition = Vector3.SmoothDamp(_nextPosition, _target.position + _target.rotation * _offset, ref _currentVelocity, _smoothDamp);
        _nextRotation = Quaternion.RotateTowards(_nextRotation, Quaternion.LookRotation(_target.position - transform.position, Vector3.up), _turnSpeedRadius * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (_target == null) return;
        transform.position = _nextPosition;
        transform.rotation = _nextRotation;
    }
}
