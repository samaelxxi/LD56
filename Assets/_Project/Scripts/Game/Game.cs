using System;
using System.Collections;
using System.Collections.Generic;
using DesignPatterns.Singleton;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : Singleton<Game>
{
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

    // Update is called once per frame
    void Update()
    {
    }
}
