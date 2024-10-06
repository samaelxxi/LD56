using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PigUI : MonoBehaviour, Services.IRegistrable
{
    [SerializeField] private GameObject _electronInSigthWindow;
    [SerializeField] private GameObject _makeOatiumButton;


    public bool IsMakeOatiumButtonActive => _makeOatiumButton.activeSelf;

    void Awake()
    {
        _electronInSigthWindow.SetActive(false);

        ServiceLocator.Register(this);
    }

    public void OnElectronInSight()
    {
        if (!_electronInSigthWindow.activeSelf)
            _electronInSigthWindow.SetActive(true);
    }

    public void OnElectronOutOfSight()
    {
        if (_electronInSigthWindow.activeSelf)
            _electronInSigthWindow.SetActive(false);
    }

    public void SetMakeOatiumActive(bool active)
    {
        _makeOatiumButton.SetActive(active);
    }
}
