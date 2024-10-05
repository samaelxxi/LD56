using System.Collections;
using System.Collections.Generic;
using CarterGames.Common.Serializiation;
using UnityEngine;



[CreateAssetMenu(fileName = "ElectronsFactory", menuName = "ElectronsFactory")]
public class ElectronsFactory : ScriptableObject, Services.IRegistrable
{
    [field: SerializeField] public SerializableDictionary<ElectronType, Electron> Electrons { get; private set; }


    public Electron GetElectronPrefab(ElectronType type)
    {
        return Electrons[type];
    }

    public Electron GetElectron(ElectronType type)
    {
        Electron electron = Instantiate(Electrons[type]);
        electron.SetType(type);

        return electron;
    }
}
