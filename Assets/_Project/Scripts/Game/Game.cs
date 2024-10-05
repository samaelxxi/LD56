using System.Collections;
using System.Collections.Generic;
using DesignPatterns.Singleton;
using UnityEngine;

public class Game : Singleton<Game>
{
    // Start is called before the first frame update
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
