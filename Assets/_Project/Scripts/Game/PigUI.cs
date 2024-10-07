using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;


public class PigUI : MonoBehaviour, Services.IRegistrable
{
    [SerializeField] private GameObject _electronInSigthWindow;
    [SerializeField] private GameObject _makeOatiumButton;
    [SerializeField] private TMPro.TMP_Text _oatiumNumberText;
    [SerializeField] private TMPro.TMP_Text _remainingTimeText;


    public bool IsMakeOatiumButtonActive => _makeOatiumButton.activeSelf;

    void Awake()
    {
        _electronInSigthWindow.SetActive(false);
        _makeOatiumButton.SetActive(false);

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

    public void SetOatiumNumber(int number)
    {
        _oatiumNumberText.text = number.ToString();
        _oatiumNumberText.transform.DOPunchScale(Vector3.one * 0.1f, 0.5f);
    }

    public void SetRemainingTime(float time)
    {
        time = Mathf.Max(0, time);

        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        _remainingTimeText.text = timeSpan.ToString(@"mm\:ss");
        // _remainingTimeText.text = time.ToString("F2");
    }
}
