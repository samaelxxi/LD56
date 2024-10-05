using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Attributes.AutoRegisteredService]
public class PigtomsManager : Services.MonoRegistrable
{
    private List<Pigtom> _pigtoms = new List<Pigtom>();


    public IEnumerable<Pigtom> Pigtoms => _pigtoms;


    public void AddPigtom(Pigtom pigtom)
    {
        _pigtoms.Add(pigtom);
    }
}
