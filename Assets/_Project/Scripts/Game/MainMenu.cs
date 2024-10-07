using System.Collections;
using System.Collections.Generic;
using CarterGames.Assets.AudioManager;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] DialogWindow _dialogWindow;
    [SerializeField] GameObject _exitButton;
    // Start is called before the first frame update
    void Start()
    {
        _dialogWindow.GoGoGo();

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
}
