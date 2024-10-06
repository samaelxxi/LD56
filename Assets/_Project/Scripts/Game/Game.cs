using System;
using System.Collections;
using System.Collections.Generic;
using DesignPatterns.Singleton;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : Singleton<Game>
{
    public QuatumPig QuatumPig { get; set; }
    private int _oatiumCollected = 0;
    private float _startTime;

    private bool _isGameOver;


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
        Cursor.lockState = CursorLockMode.Locked;

        Debug.Log("Game.Start");
    }

    void Update()
    {
        float remainingTime = GameSettings.GameSessionTimeSeconds - (Time.time - _startTime);
        remainingTime = Mathf.Max(0, GameSettings.GameSessionTimeSeconds - (Time.time - _startTime));
        ServiceLocator.Get<PigUI>().SetRemainingTime(remainingTime);

        if (remainingTime <= 0 && !_isGameOver)
        {
            GameOver();
        }
    }

    public void StartNewGame()
    {
        _startTime = Time.time;
        _oatiumCollected = 0;
        _isGameOver = false;
        ServiceLocator.Get<PigUI>().SetOatiumNumber(0);
    }

    private void GameOver()
    {
        _isGameOver = true;
        Debug.Log("Game over");
    }

    public void CollectOatium()
    {
        _oatiumCollected++;
        ServiceLocator.Get<PigUI>().SetOatiumNumber(_oatiumCollected);
    }
}
