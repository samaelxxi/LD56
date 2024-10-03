using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerZone : MonoBehaviour
{
    [SerializeField] protected UnityEvent _onTriggerEnter;
    [SerializeField] protected bool _onlyOnce;

    [SerializeField] protected LayerMask _layerMask;

    protected bool _triggered;

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (_triggered && _onlyOnce)
            return;

        if ((_layerMask.value & 1 << other.gameObject.layer) != 0)
        {
            _onTriggerEnter.Invoke();
            _triggered = true;
        }
    }
}
