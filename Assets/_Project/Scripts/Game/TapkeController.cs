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
    Vector3 _target;

    Vector3 _bezierStart;
    Vector3 _bezierControlPoint;
    float _bezierTime = 0;
    float _bezierDistance = 0;
    float _bezierPathTime = 0;
    float _currentDt = 0;

    public void SetStraightMovementTarget(Vector3 target)
    {
        _straightMovement = true;
        _target = target;
        _isMoving = true;
    }

    public void SetBezierMovementTarget(Vector3 target, Vector3 controlPoint)
    {
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
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * _rotationSpeed);
    }

    void FixedUpdate()
    {
        if (!_isMoving)
            return;

        Vector3 nextPosition;
        if (_straightMovement)
        {
            nextPosition = Vector3.MoveTowards(transform.position, _target, _speed * Time.deltaTime);
        }
        else
        {
            // idk some stupid formulas I found shoved up my Ass
            float targetDT = 1 / (_bezierDistance / _speed) * Time.fixedDeltaTime;
            float remainingRealTime = (1 - _bezierTime) * _bezierDistance / _speed;
            if (remainingRealTime < _bezierPathTime / 5.0f)
            {
                float delta = targetDT / ((_bezierPathTime / 5.0f) * 50f) / 2;
                targetDT /= 10;
                _currentDt = Mathf.MoveTowards(_currentDt, targetDT, delta);
            }
            else
            {
                _currentDt = Mathf.MoveTowards(_currentDt, targetDT, targetDT/50f);
            }


            _bezierTime += _currentDt;
            nextPosition = MathExtensions.Bezier(_bezierStart, _bezierControlPoint, _target, _bezierTime);
        }

        RotateForward(nextPosition);
        transform.position = nextPosition;

        if (Vector3.Distance(transform.position, _target) < 0.1f)
        {
            _isMoving = false;
            Debug.Log(Time.time);
        }
    }

    private void OnDrawGizmos()
    {
        if (_isMoving)
        {
            Gizmos.color = Color.green;
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
        }
    }

}
