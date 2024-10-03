using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ListenedField<T>
{
    private T _value;

    public T Value
    {
        get => _value;
        set
        {
            _value = value;
            OnValueChanged?.Invoke(_value);
        }
    }

    public event System.Action<T> OnValueChanged;

    public ListenedField(T value = default(T))
    {
        _value = value;
    }
}
