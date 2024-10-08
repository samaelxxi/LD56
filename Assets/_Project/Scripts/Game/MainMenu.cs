using System.Collections;
using System.Collections.Generic;
using CarterGames.Assets.AudioManager;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] DialogWindow _dialogWindow;
    [SerializeField] GameObject _exitButton;
    [SerializeField] Toggle _hardModeToggle;


    void Start()
    {
        _dialogWindow.GoGoGo();

        _hardModeToggle.isOn = GameSettings.HardMode;

        #if UNITY_WEBGL
        _exitButton.SetActive(false);
        #endif
    }

    public void StartGame()
    {
        AudioManager.Play("click");
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Game");
    }

    public void ExitGame()
    {
        AudioManager.Play("click");
        Application.Quit();
    }

    public void SetHardMode(bool hardMode)
    {
        GameSettings.HardMode = hardMode;
    }
}
