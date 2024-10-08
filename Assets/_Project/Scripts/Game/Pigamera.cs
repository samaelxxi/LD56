using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteAlways]
public class Pigamera : MonoBehaviour
{
    [SerializeField] Transform _target;
    [SerializeField] Vector3 _offset;
    [SerializeField] Vector3 _rotationOffset;

    [SerializeField] float smoothSpeed = 0.125f;
    [SerializeField] float rotationSmoothSpeed = 0.125f;


    public enum UpdateType { Update, LateUpdate, FixedUpdate }
    [SerializeField] UpdateType _updateType = UpdateType.LateUpdate;


    void OnValidate()
    {
        if (_target == null) return;

        // Calculate the relative offset
        Vector3 relativeOffset = _target.TransformDirection(_offset);
        transform.position = _target.position + relativeOffset;

        var targetRotation = _target.rotation.eulerAngles + _rotationOffset;
        transform.rotation = Quaternion.Euler(targetRotation);
    }


    void LateUpdate()
    {
        if (_updateType == UpdateType.LateUpdate)
            UpdateThings();
    }

    void Update()
    {
        if (_updateType == UpdateType.Update)
            UpdateThings();
    }

    void FixedUpdate()
    {
        if (_updateType == UpdateType.FixedUpdate)
            UpdateThings();
    }

    void UpdateThings()
    {
        Vector3 relativeOffset = _target.TransformDirection(_offset);
        Vector3 desiredPosition = _target.position + relativeOffset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        var targetRotation = _target.rotation.eulerAngles + _rotationOffset;
        Quaternion desiredRotation = Quaternion.Euler(targetRotation);
        Quaternion smoothedRotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSmoothSpeed);
        transform.rotation = smoothedRotation;
    }
}
