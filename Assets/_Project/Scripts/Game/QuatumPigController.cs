using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuatumPigController : MonoBehaviour
{
    [SerializeField] float acceleration = 5f;
    [SerializeField] float maxSpeed = 5f;
    [SerializeField] float rotationSpeed = 100f;

    float _horizontalInput;
    float _verticalInput;
    float _mouseXInput;
    float _mouseYInput;

    Rigidbody _rigidbody;

    float _currentForwardSpeed;
    float _currentStrafeSpeed;


    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");
        _mouseXInput = Input.GetAxis("Mouse X");
        _mouseYInput = Input.GetAxis("Mouse Y");
    }

    void FixedUpdate()
    {
        // Handle movement
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        _currentForwardSpeed = Mathf.MoveTowards(_currentForwardSpeed, _verticalInput * maxSpeed, acceleration * Time.fixedDeltaTime);
        _currentStrafeSpeed = Mathf.MoveTowards(_currentStrafeSpeed, _horizontalInput * maxSpeed, acceleration * Time.fixedDeltaTime);

        Vector3 movement = _currentForwardSpeed * forward + _currentStrafeSpeed * right;
        movement = Vector3.ClampMagnitude(movement, maxSpeed);
        _rigidbody.AddForce(movement * Time.fixedDeltaTime, ForceMode.VelocityChange);

        // // Handle rotation
        // float yaw = _mouseXInput * rotationSpeed * Time.fixedDeltaTime;
        // float pitch = -_mouseYInput * rotationSpeed * Time.fixedDeltaTime;

        // Vector3 torque = new Vector3(pitch, yaw, 0);
        // _rigidbody.AddTorque(torque, ForceMode.VelocityChange);

        // // Constrain roll
        // Vector3 currentEulerAngles = _rigidbody.rotation.eulerAngles;
        // currentEulerAngles.z = 0; // Constrain roll to 0
        // _rigidbody.rotation = Quaternion.Euler(currentEulerAngles);


        // // Handle movement
        // Vector3 forward = transform.forward;
        // Vector3 right = transform.right;
        // _currentForwardSpeed = Mathf.MoveTowards(_currentForwardSpeed, _verticalInput * maxSpeed, acceleration * Time.fixedDeltaTime);
        // _currentStrafeSpeed = Mathf.MoveTowards(_currentStrafeSpeed, _horizontalInput * maxSpeed, acceleration * Time.fixedDeltaTime);

        // Vector3 movement = _currentForwardSpeed * forward + _currentStrafeSpeed * right;
        // movement = Vector3.ClampMagnitude(movement, maxSpeed);
        // _rigidbody.MovePosition(_rigidbody.position + movement * Time.fixedDeltaTime);



        // Vector3 movement = _verticalInput * maxSpeed * Time.fixedDeltaTime * transform.forward;
        // movement += _horizontalInput * maxSpeed * Time.fixedDeltaTime * transform.right;
        // movement = Vector3.ClampMagnitude(movement, maxSpeed);
        // _rigidbody.MovePosition(_rigidbody.position + movement);


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
        targetRotation = Quaternion.Euler(targetEulerAngles);

        _rigidbody.MoveRotation(Quaternion.Slerp(currentRotation, targetRotation, 50 * Time.fixedDeltaTime));    }
}
