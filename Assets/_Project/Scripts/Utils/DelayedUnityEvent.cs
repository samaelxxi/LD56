using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DelayedUnityEvent : MonoBehaviour
{
    public float delay = 1;
    public bool repeat = false;
    public bool startOnAwake = false;

    public UnityEngine.Events.UnityEvent onDelayedEvent;

    private void Awake()
    {
        if (startOnAwake)
            StartDelayedEvent();
    }

    public void StartDelayedEvent()
    {
        StartCoroutine(DelayedEvent());
    }

    private IEnumerator DelayedEvent()
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            onDelayedEvent.Invoke();
            if (!repeat)
                yield break;
        }
    }
}