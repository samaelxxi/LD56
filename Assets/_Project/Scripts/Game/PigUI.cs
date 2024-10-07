using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;
using CarterGames.Assets.AudioManager;


public class PigUI : MonoBehaviour
{
    [SerializeField] private GameObject _electronInSigthWindow;
    [SerializeField] private GameObject _makeOatiumButton;
    [SerializeField] private TMPro.TMP_Text _oatiumNumberText;
    [SerializeField] private TMPro.TMP_Text _remainingTimeText;
    [SerializeField] private GameObject _controls;
    [SerializeField] private Slider _mouseSlider;
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _endMenu;
    [SerializeField] private TMPro.TMP_Text _endOatiumNumberText;


    public bool IsMakeOatiumButtonActive => _makeOatiumButton.activeSelf;

    void Awake()
    {
        _electronInSigthWindow.SetActive(false);
        _makeOatiumButton.SetActive(false);

        Game.Instance.PigUI = this;

        _mouseSlider.value = GameSettings.MoveSensetivity;
        _mouseSlider.onValueChanged.AddListener(delegate { OnMouseSliderChanged(); });
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

    public void OnMouseSliderChanged()
    {
        GameSettings.MoveSensetivity = _mouseSlider.value;
    }

    public void ShowPauseMenu()
    {
        _pauseMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
    }

    public void HidePauseMenu()
    {
        _pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
        _controls.SetActive(false);
    }

    public void ShowGameOver()
    {
        Time.timeScale = 0;
        _endMenu.SetActive(true);
        _endOatiumNumberText.text = _oatiumNumberText.text;
    }

    public void OnContinueClicked()
    {
        AudioManager.Play("click");
        Game.Instance.ResumeGame();
    }

    public void OnGoToMenuClicked()
    {
        AudioManager.Play("click");
        Game.Instance.GoToMainMenu();
    }

    public void ShowControls()
    {
        AudioManager.Play("click");
        _controls.SetActive(true);
        _pauseMenu.SetActive(false);
    }

    public void HideControls()
    {
        AudioManager.Play("click");
        _controls.SetActive(false);
        _pauseMenu.SetActive(true);
    }
}
