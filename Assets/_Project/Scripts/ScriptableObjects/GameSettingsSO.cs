using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CarterGames.Common.Serializiation;


[CreateAssetMenu(fileName = "GameSettings", menuName = "GameSettings")]
public class GameSettingsSO : ScriptableObject, Services.IRegistrable, Services.IInitializable
{
    [field: SerializeField] public int MaxCollectedElectrons { get; private set; }
    [field: SerializeField] public SerializableDictionary<ElectronType, int> OatCombination { get; private set; }
    [field: SerializeField] public Oatium OatiumPrefab { get; private set; }
    [field: SerializeField] public float GameSessionTimeSeconds { get; private set; }

    public void Initialize()
    {
        GameSettings.Init(this);
        Debug.Log("GameSettingsSO.Initialize");
    }
}