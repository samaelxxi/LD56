using System;
using System.Collections;
using System.Collections.Generic;
using CarterGames.Assets.AudioManager;
using DesignPatterns.Singleton;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : Singleton<Game>
{
    public QuatumPig QuatumPig { get; private set; }
    public PigUI PigUI { get => _pigUI; set => SetPigUI(value); }
    private int _oatiumCollected = 0;
    private float _startTime;

    private int _totalOatiumCollected;

    private bool _gameIsRunning;
    private bool _isGameOver;
    private bool _isPaused;

    PigUI _pigUI { get; set; }

    public bool IsPaused => _isPaused;
    public int TotalOatiumCollected => _totalOatiumCollected;


    public override void Awake()
    {
        base.Awake();
        SceneManager.activeSceneChanged += ChangedActiveScene;
    }

    private void ChangedActiveScene(Scene arg0, Scene arg1)
    {
    }

    private void SetPigUI(PigUI pigUI)
    {
        _pigUI = pigUI;
        #if UNITY_STANDALONE
        PigUI.HideEscHint();
        #endif
    }

    void Start()
    {
        MusicManager.Play("Music", volume: 0.5f);

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
        HideCursor();
        _startTime = Time.time;
        _oatiumCollected = 0;
        _isGameOver = false;
        _gameIsRunning = true;
        Game.Instance.PigUI.SetOatiumNumber(0);
    }

    private void GameOver()
    {
        ShowCursor();
        Game.Instance.PigUI.ShowGameOver();

        _isGameOver = true;
        _gameIsRunning = false;
        Debug.Log("Game over");
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadSceneAsync("MainMenu");
        Time.timeScale = 1;
        ShowCursor();
        _isGameOver = false;
        _isPaused = false;
        _gameIsRunning = false;
    }

    void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        _isPaused = true;
        _gameIsRunning = false;
        ShowCursor();
        Game.Instance.PigUI.ShowPauseMenu();
        Game.Instance.PigUI.HideEscHint();
    }

    public void ResumeGame()
    {
        if (_isGameOver)
            return;

        HideCursor();
        Time.timeScale = 1;
        _isPaused = false;
        _gameIsRunning = true;
        Game.Instance.PigUI.HidePauseMenu();
    }

    public void CollectOatium()
    {
        _oatiumCollected++;
        _totalOatiumCollected++;
        AudioManager.Play("ateOat", volume: 0.5f);
        Game.Instance.PigUI.SetOatiumNumber(_oatiumCollected);
    }
}
