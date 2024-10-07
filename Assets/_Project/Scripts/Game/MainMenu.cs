using System.Collections;
using System.Collections.Generic;
using CarterGames.Assets.AudioManager;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] DialogWindow _dialogWindow;
    // Start is called before the first frame update
    void Start()
    {
        _dialogWindow.GoGoGo();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        AudioManager.Play("click");
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Game");
    }
}
