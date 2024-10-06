using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[Attributes.AutoRegisteredService]
public class PigtomsManager : Services.MonoRegistrable
{
    private List<Pigtom> _pigtoms = new List<Pigtom>();


    public IEnumerable<Pigtom> Pigtoms => _pigtoms;
    public IEnumerable<Pigtom> CoolPigtoms => _pigtoms.Where(p => !p.StartedTransformation);


    public void AddPigtom(Pigtom pigtom)
    {
        _pigtoms.Add(pigtom);
    }

    public void RemovePigtom(Pigtom pigtom)
    {
        _pigtoms.Remove(pigtom);
    }
}
