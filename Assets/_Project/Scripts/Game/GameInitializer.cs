using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public static class GameInitializer
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        Debug.Log("GameInitializer.Initialize");
        InitializeServices();

        // TODO rework?
        var objs = UnityEngine.Object.FindObjectsByType(typeof(Game), FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (objs.Length > 0) return;
        Game.Instantiate(Resources.Load<Game>("Game"));
        // Globals.IsGameInitialized = true;
    }

    static void InitializeServices()
    {
        Debug.Log("GameInitializer.InitializeServices");
        ServiceLocator.RegisterSO<GameSettingsSO>("Services/GameSettings");
        ServiceLocator.Get<GameSettingsSO>().Initialize(); // idk why not initialized in RegisterSO
        ServiceLocator.RegisterSO<ElectronsFactory>("Services/ElectronsFactory");
        // ServiceLocator.RegisterSO<PrefabsHolder>("Services/PrefabsHolder");
        // ServiceLocator.RegisterSO<VFXManager>("Services/VFXManager");
        // ServiceLocator.RegisterSO<GlobalSO>("Services/GlobalSO");
        // ServiceLocator.RegisterSO<EditorUserSettings>("Services/EditorUserSettings");
        // ServiceLocator.RegisterSO<EnemyManager>("Services/EnemyManager");
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void InitializeAfterSceneLoad()
    {
        // Game.Instantiate(Resources.Load<GameObject>("AudioManager"));
    }
}
