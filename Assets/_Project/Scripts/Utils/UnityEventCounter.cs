using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class UnityEventCounter : MonoBehaviour
{
    [SerializeField] private int requiredCount;
    [SerializeField] private float delayForActivation;
    [SerializeField] private UnityEvent onCountReached;

    private int _counter = 0;
    private bool _isActivated = false;

    private Coroutine _delayedActivationCoroutine;

    public void IncrementCounter()
    {
        _counter++;

        if (_counter >= requiredCount)
        {
            if (_isActivated)
                return;

            _isActivated = true;
            _delayedActivationCoroutine = this.InSeconds(delayForActivation, () =>
            {
                onCountReached.Invoke();
            });
        }
    }

    public void ResetCounter()
    {
        _counter = 0;
        _isActivated = false;
        if (_delayedActivationCoroutine != null)
        {
            StopCoroutine(_delayedActivationCoroutine);
            _delayedActivationCoroutine = null;
        }
    }

    public void SetRequiredCount(int count)
    {
        requiredCount = count;
    }

    public void AddRequiredCount(int count)
    {
        requiredCount += count;
    }
}
