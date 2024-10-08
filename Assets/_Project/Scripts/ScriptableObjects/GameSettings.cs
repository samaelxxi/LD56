using System.Collections;
using System.Collections.Generic;
using CarterGames.Common.Serializiation;
using UnityEngine;


public static class GameSettings
{
    private static GameSettingsSO _gameSettings;

    public static void Init(GameSettingsSO gameSettings)
    {
        _gameSettings = gameSettings;
    }

    public static int MaxCollectedElectrons => _gameSettings.MaxCollectedElectrons;
    public static SerializableDictionary<ElectronType, int> OatCombination => _gameSettings.OatCombination;
    public static Oatium OatiumPrefab => _gameSettings.OatiumPrefab;
    public static Tapke TapkePrefab => _gameSettings.TapkePrefab;
    public static List<GameObject> RandomStuffPrefabs => _gameSettings.RandomStuffPrefabs;
    public static float GameSessionTimeSeconds => _gameSettings.GameSessionTimeSeconds;


    public static WorldCreatorSettings WorldCreatorSettings => HardMode ? _gameSettings.HardWorldCreatorSettings : _gameSettings.EasyWorldCreatorSettings;

    public static float FailSpawnChance => _gameSettings.FailSpawnChance;
    public static float TapkeSpawnChance => _gameSettings.TapkeSpawnChance;


    public static float MoveSensetivity { get; set; } = 1;
    public static bool HardMode { get; set; } = false;
}