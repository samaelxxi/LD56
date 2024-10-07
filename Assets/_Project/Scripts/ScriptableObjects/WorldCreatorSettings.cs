using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldCreatorSettings", menuName = "ScriptableObjects/WorldCreatorSettings", order = 1)]
public class WorldCreatorSettings : ScriptableObject
{
    [field: SerializeField] public Vector3 WorldCenter { get; private set; }
    [field: SerializeField] public Vector3 WorldSize { get; private set; }

    [field: SerializeField] public Pigtom PigtomPrefab { get; private set; }
    [field: SerializeField] public int PigtomsQuantity { get; private set; }
    [field: SerializeField] public int PigtomHoles { get; private set; }
    [field: SerializeField] public float DesiredAverageDistance { get; private set; }
    [field: SerializeField] public float DesiredAverageDistanceDeviation { get; private set; }


    [field: SerializeField, Space] public float PigtomRadius { get; private set; }
    [field: SerializeField] public float PigtomRadiusDeviation { get; private set; }
    [field: SerializeField] public float MinOrbitRadius { get; private set; }
    [field: SerializeField] public float MaxOrbitRadius { get; private set; }
    [field: SerializeField] public int MinElectronsPerPigtom { get; private set; }
    [field: SerializeField] public int MaxElectronsPerPigtom { get; private set; }
    [field: SerializeField] public float MinElectronOrbitingTime { get; private set; }
    [field: SerializeField] public float MaxElectronOrbitingTime { get; private set; }


    [field: SerializeField, Space] public int MinTapkes { get; private set; }
    [field: SerializeField] public int MaxTapkes { get; private set; }


}
