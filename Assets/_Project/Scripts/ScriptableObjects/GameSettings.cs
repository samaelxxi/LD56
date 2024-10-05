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
}