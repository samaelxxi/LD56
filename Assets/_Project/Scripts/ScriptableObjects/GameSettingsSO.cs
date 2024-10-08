using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CarterGames.Common.Serializiation;


[CreateAssetMenu(fileName = "GameSettings", menuName = "GameSettings")]
public class GameSettingsSO : ScriptableObject, Services.IRegistrable, Services.IInitializable
{
    [Space]
    [field: SerializeField, Range(0, 1)]
    public float FailSpawnChance = 0.5f;
    [field: SerializeField, Range(0, 1)]
    public float TapkeSpawnChance = 0.5f;

    [field: SerializeField, Space] 
    public float GameSessionTimeSeconds { get; private set; }
    [field: SerializeField] public int MaxCollectedElectrons { get; private set; }
    [field: SerializeField] public SerializableDictionary<ElectronType, int> OatCombination { get; private set; }


    [field: SerializeField, Space(5)] public Oatium OatiumPrefab { get; private set; }
    [field: SerializeField] public Tapke TapkePrefab { get; private set; }
    [field: SerializeField] public List<GameObject> RandomStuffPrefabs { get; private set; }

    [field: SerializeField] public WorldCreatorSettings EasyWorldCreatorSettings { get; private set; }
    [field: SerializeField] public WorldCreatorSettings HardWorldCreatorSettings { get; private set; }




    public void Initialize()
    {
        GameSettings.Init(this);
        Debug.Log("GameSettingsSO.Initialize");
    }
}