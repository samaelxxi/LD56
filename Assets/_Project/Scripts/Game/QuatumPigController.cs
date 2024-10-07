using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuatumPigController : MonoBehaviour
{
    [SerializeField] float acceleration = 5f;
    [SerializeField] float maxSpeed = 5f;
    [SerializeField] float rotationSpeed = 100f;


    public event Action OnEatElectron;
    public event Action OnCollectElectron;
    public event Action OnShootElectron;
    public event Action ScrollWheelUp;
    public event Action ScrollWheelDown;
    public event Action OnEscapePressed;

    public float TotalForwardSpeed => _totalForwardSpeed;

    float _horizontalInput;
    float _forwardInput;
    float _verticalInput;
    float _mouseXInput;
    float _mouseYInput;

    Rigidbody _rigidbody;

    float _currentForwardSpeed;
    float _currentStrafeSpeed;
    float _currentVerticalSpeed;
    float _totalForwardSpeed;



    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _forwardInput = Input.GetAxis("Vertical");
        _verticalInput = Input.GetAxis("RealVertical");


        float newMouseX = Input.GetAxis("Mouse X") * GameSettings.MoveSensetivity;
        float newMouseY = Input.GetAxis("Mouse Y") * GameSettings.MoveSensetivity;


        _mouseXInput = Mathf.Lerp(_mouseXInput, newMouseX, 0.2f *  GameSettings.MoveSensetivity);
        _mouseYInput = Mathf.Lerp(_mouseYInput, newMouseY, 0.2f *  GameSettings.MoveSensetivity);

        if (Input.GetKeyDown(KeyCode.Q))
            OnEatElectron?.Invoke();
        if (Input.GetKeyDown(KeyCode.E))
            OnCollectElectron?.Invoke();

        if (Input.GetMouseButtonDown(0))
            OnShootElectron?.Invoke();
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
            ScrollWheelUp?.Invoke();
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
            ScrollWheelDown?.Invoke();

        if (Input.GetKeyDown(KeyCode.Escape))
            OnEscapePressed?.Invoke();

        _rigidbody.angularVelocity = Vector3.zero;
    }

    public void AddSomeForce(Vector3 force)
    {
        _rigidbody.AddForce(force, ForceMode.Impulse);
    }

    void FixedUpdate()
    {
        // Handle movement
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        _horizontalInput *= 0.7f;
        _verticalInput *= 0.7f;
        _currentForwardSpeed = Mathf.MoveTowards(_currentForwardSpeed, _forwardInput * maxSpeed, acceleration * Time.fixedDeltaTime);
        _currentStrafeSpeed = Mathf.MoveTowards(_currentStrafeSpeed, _horizontalInput * maxSpeed, acceleration * Time.fixedDeltaTime);
        _currentVerticalSpeed = Mathf.MoveTowards(_currentVerticalSpeed, _verticalInput * maxSpeed, acceleration * Time.fixedDeltaTime);

        Vector3 movement = _currentForwardSpeed * forward + _currentStrafeSpeed * right + _currentVerticalSpeed * Vector3.up;
        movement = Vector3.ClampMagnitude(movement, maxSpeed);
        _totalForwardSpeed = _currentForwardSpeed;
        _rigidbody.AddForce(movement * Time.fixedDeltaTime, ForceMode.VelocityChange);


        // Handle rotation
        float yaw = _mouseXInput * rotationSpeed * Time.fixedDeltaTime;
        float pitch = -_mouseYInput * rotationSpeed * Time.fixedDeltaTime;

        Quaternion currentRotation = _rigidbody.rotation;
        Quaternion yawRotation = Quaternion.Euler(0, yaw, 0);
        Quaternion pitchRotation = Quaternion.Euler(pitch, 0, 0);

        Quaternion targetRotation = currentRotation * yawRotation * pitchRotation;

        // Constrain roll
        Vector3 targetEulerAngles = targetRotation.eulerAngles;
        targetEulerAngles.z = 0; // Constrain roll to 0
        targetEulerAngles.x = Mathf.Clamp(targetEulerAngles.x > 180 ? targetEulerAngles.x - 360 : targetEulerAngles.x, -60, 60);



        targetRotation = Quaternion.Euler(targetEulerAngles);
        var rot = Quaternion.RotateTowards(currentRotation, targetRotation, rotationSpeed * Time.fixedDeltaTime * GameSettings.MoveSensetivity);

        // rot = Quaternion.Slerp(currentRotation, targetRotation, 50 * Time.fixedDeltaTime);
        _rigidbody.MoveRotation(rot);
    }
}
