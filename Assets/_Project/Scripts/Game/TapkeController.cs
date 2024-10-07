using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapkeController : MonoBehaviour
{
    [SerializeField] private float _speed = 3f;
    [SerializeField] private float _rotationSpeed = 30f;

    public bool IsMoving => _isMoving;


    bool _straightMovement = false;
    bool _isMoving = false;
    bool _movingAroundPigtom = false;
    Vector3 _target;

    Vector3 _bezierStart;
    Vector3 _bezierControlPoint;
    float _bezierTime = 0;
    float _bezierDistance = 0;
    float _bezierPathTime = 0;
    float _currentDt = 0;

    float _pigtomRadius = 1.5f;

    public void SetStraightMovementTarget(Vector3 target)
    {
        _movingAroundPigtom = false;
        _straightMovement = true;
        _target = target;
        _isMoving = true;
    }

    public void SetBezierMovementTarget(Vector3 target, Vector3 controlPoint)
    {
        _movingAroundPigtom = false;
        _straightMovement = false;
        _target = target;
        _isMoving = true;

        _bezierStart = transform.position;
        _bezierControlPoint = controlPoint;
        _bezierTime = 0;
        _bezierDistance = MathExtensions.CalculateBezierDistance(_bezierStart, controlPoint, target);
        _bezierPathTime = _bezierDistance / _speed;
        _currentDt = 0;
    }

    public void RotateForward(Vector3 nextPosition)
    {
        Vector3 direction = (nextPosition - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        float speed = _rotationSpeed * Time.deltaTime;
        if (_movingAroundPigtom)
            speed *= 5;
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, speed);
    }

    void FixedUpdate()
    {
        if (!_isMoving)
            return;

        if (_movingAroundPigtom)
        {
            MoveAroundPigtom();
        }
        else
        {
            MoveToTarget();
        }

        
        RotateForward(_nextPosition);
        transform.position = _nextPosition;
    }


    Vector3 _nextPosition;
    private void MoveToTarget()
    {
        if (_straightMovement)
        {
            _nextPosition = Vector3.MoveTowards(transform.position, _target, _speed * Time.deltaTime);
        }
        else
        {
            // idk some stupid formulas I found shoved up my Ass
            float targetDT = 1 / (_bezierDistance / _speed) * Time.fixedDeltaTime;
            float remainingRealTime = (1 - _bezierTime) * _bezierDistance / _speed;
            if (remainingRealTime < _bezierPathTime / 5.0f)
            {
                float delta = targetDT / ((_bezierPathTime / 5.0f) * 50f) / 2;
                targetDT /= 7;
                _currentDt = Mathf.MoveTowards(_currentDt, targetDT, delta);
            }
            else
            {
                _currentDt = Mathf.MoveTowards(_currentDt, targetDT, targetDT/50f);
            }


            _bezierTime += _currentDt;
            _nextPosition = MathExtensions.Bezier(_bezierStart, _bezierControlPoint, _target, _bezierTime);
        }

        if (Vector3.Distance(transform.position, _target) < 0.1f)
        {
            _isMoving = false;
        }
    }


    float longitude, latitude;
    private void MoveAroundPigtom()
    {
        float radius = _pigtomRadius + 1;
        float linearDistance = _speed * Time.deltaTime / 2;

        float angularDisplacement = linearDistance / radius;
        float deltaLongitude = angularDisplacement * Mathf.Rad2Deg;
        longitude = (longitude + deltaLongitude) % 360f;
        latitude = (latitude + deltaLongitude) % 360f;
        


        // Convert the latitude and longitude to Cartesian coordinates
        float radLat = Mathf.Deg2Rad * latitude;
        float radLon = Mathf.Deg2Rad * longitude;

        // Use spherical to Cartesian conversion
        float x = radius * Mathf.Cos(radLat) * Mathf.Cos(radLon);
        float y = radius * Mathf.Sin(radLat);
        float z = radius * Mathf.Cos(radLat) * Mathf.Sin(radLon);

        // Check for NaNs in the output
        if (float.IsNaN(x) || float.IsNaN(y) || float.IsNaN(z))
        {
            Debug.LogError("Conversion resulted in NaN values");
            Debug.Log($"Angular Displacement: {angularDisplacement} = {linearDistance} / {radius}");

            Debug.Log($"Latitude: {latitude}, Longitude: {longitude}");
            latitude = 0;
            longitude = 0;
            return;
        }

        // Set the position relative to the sphere's center
        _nextPosition = _target + new Vector3(x, y, z);
    }

    public void StartMovingAroundPigtom(Vector3 center, float radius)
    {
        _movingAroundPigtom = true;
        _isMoving = true;
        _target = center;
        _pigtomRadius = radius;

        // Compute initial latitude and longitude based on the current position
        Vector3 relativePosition = transform.position - center;
        latitude = Mathf.Asin(relativePosition.y / radius) * Mathf.Rad2Deg;
        longitude = Mathf.Atan2(relativePosition.z, relativePosition.x) * Mathf.Rad2Deg;

        if (float.IsNaN(latitude) || float.IsNaN(longitude))
        {
            Debug.Log(center);
            Debug.LogError("Initial latitude or longitude is NaN");
            Debug.Log($"Relative Position: {relativePosition} | Radius: {radius}");
            Debug.Log($"Latitude: {latitude}, Longitude: {longitude}");
        }
    }

    public void StopMovingAroundPigtom()
    {
        _movingAroundPigtom = false;
        _isMoving = false;
    }

    private void OnDrawGizmos()
    {
        if (_isMoving)
        {
            Gizmos.color = Color.blue;
            if (_straightMovement)
                Gizmos.DrawLine(transform.position, _target);
            else
            {
                Vector3 prev = _bezierStart;
                for (int i = 0; i < 100; i++)
                {
                    float t = i / 100f;
                    Vector3 next = MathExtensions.Bezier(_bezierStart, _bezierControlPoint, _target, t);
                    Gizmos.DrawLine(prev, next);
                    prev = next;
                }
            }

            if (_movingAroundPigtom)
            {
                for (int i = 0; i < 1; i++)
                {

                    float radius = _pigtomRadius + 1;

                    float angularSpeed = _speed /radius;
                    // Update longitude using the angular speed
                    float testLongitude = longitude;
                    testLongitude += angularSpeed * Mathf.Rad2Deg * Time.deltaTime * i;
                    testLongitude = testLongitude % 360;

                    // Convert latitude and longitude (spherical coordinates) to Cartesian (x, y, z)
                    float radLat = Mathf.Deg2Rad * latitude;
                    float radLon = Mathf.Deg2Rad * testLongitude;

                    // Use spherical to Cartesian conversion
                    float x = radius * Mathf.Cos(radLat) * Mathf.Cos(radLon);
                    float y = radius * Mathf.Sin(radLat);
                    float z = radius * Mathf.Cos(radLat) * Mathf.Sin(radLon);

                    // Set the position relative to the sphere's center
                    var nextPosition = _target + new Vector3(x, y, z);

                    if (i == 0)
                    {
                        Gizmos.color = Color.green;
                        Gizmos.DrawSphere(nextPosition, 0.5f);

                        // Vector3 direction = (nextPosition - transform.position).normalized;
                        // Quaternion lookRotation = Quaternion.LookRotation(direction);
                        // Gizmos.DrawRay(transform.position, direction* 3);


                    }

                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(nextPosition, 0.05f);
                }
                Gizmos.DrawSphere(transform.position, 1);

                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(transform.position, transform.forward * 5);
            }
        }
    }

}
