using System;
using System.Collections;
using System.Collections.Generic;
using DesignPatterns.Singleton;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : Singleton<Game>
{
    public QuatumPig QuatumPig { get; private set; }
    public PigUI PigUI { get; set; }
    private int _oatiumCollected = 0;
    private float _startTime;

    private bool _gameIsRunning;
    private bool _isGameOver;
    private bool _isPaused;

    public bool IsPaused => _isPaused;


    public override void Awake()
    {
        base.Awake();
        SceneManager.activeSceneChanged += ChangedActiveScene;
    }

    private void ChangedActiveScene(Scene arg0, Scene arg1)
    {
    }

    void Start()
    {

        Debug.Log("Game.Start");
    }

    void Update()
    {
        if (!_gameIsRunning)
        {
            return;
        }

        float remainingTime = GameSettings.GameSessionTimeSeconds - (Time.time - _startTime);
        Game.Instance.PigUI.SetRemainingTime(remainingTime);

        if (remainingTime <= 0 && !_isGameOver)
        {
            GameOver();
        }
    }

    public void SetPig(QuatumPig pig)
    {
        Debug.Log($"Game.SetPig |{pig}|");
        QuatumPig = pig;
    }

    public void StartNewGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _startTime = Time.time;
        _oatiumCollected = 0;
        _isGameOver = false;
        _gameIsRunning = true;
        Game.Instance.PigUI.SetOatiumNumber(0);
    }

    private void GameOver()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Game.Instance.PigUI.ShowGameOver();

        _isGameOver = true;
        _gameIsRunning = false;
        Debug.Log("Game over");
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
        _isGameOver = false;
        _isPaused = false;
        _gameIsRunning = false;
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        _isPaused = true;
        _gameIsRunning = false;
        Game.Instance.PigUI.ShowPauseMenu();
    }

    public void ResumeGame()
    {
        if (_isGameOver)
            return;

        Time.timeScale = 1;
        _isPaused = false;
        _gameIsRunning = true;
        Game.Instance.PigUI.HidePauseMenu();
    }

    public void CollectOatium()
    {
        _oatiumCollected++;
        Game.Instance.PigUI.SetOatiumNumber(_oatiumCollected);
    }
}
