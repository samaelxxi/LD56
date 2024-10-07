using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ElectronsTrigger : MonoBehaviour
{
    List<Electron> _electrons = new();
    Pigtom _pigtom;


    public Pigtom NearestPigtom => _pigtom;
    public Electron NearestElectron => GetAnyGoodElectron();
    public IEnumerable<Electron> Electrons => _electrons;

    public event Action<Pigtom> OnPigtomEnter;
    public event Action<Pigtom> OnPigtomExit;

    public event Action<Electron> OnElectronEnter;
    public event Action<Electron> OnElectronExit;

    ElectronOrbit _ignoredOrbit;


    public void SetIgnoredOrbit(ElectronOrbit orbit)
    {
        _ignoredOrbit = orbit;
    }

    private Electron GetAnyGoodElectron()
    {
        _electrons.RemoveAll(e => e == null);

        return _electrons.FirstOrDefault(e => e.Orbit != _ignoredOrbit);
    }


    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"{_electrons.Count} electrons in trigger");
        foreach (var e in _electrons)
            Debug.Log(e.name);



        if (other.TryGetComponent(out Electron electron) 
                && electron.Orbit != _ignoredOrbit &&
                !_electrons.Contains(electron) &&
                !electron.NotCatchable)
        {
            _electrons.Add(electron);
            OnElectronEnter?.Invoke(electron);
        }
        else if (other.TryGetComponent(out Pigtom pigtom))
        {
            _pigtom = pigtom;
            OnPigtomEnter?.Invoke(pigtom);
        }
    }

    void OnTriggerExit(Collider other)
    {

        if (other.TryGetComponent(out Electron electron)
                && electron.Orbit != _ignoredOrbit)
        {
            if (_electrons.Contains(electron))
            {
                _electrons.Remove(electron);

                OnElectronExit?.Invoke(electron);
            }
        }
        else if (other.TryGetComponent(out Pigtom pigtom) && _pigtom == pigtom)
        {
            _pigtom = null;
            OnPigtomExit?.Invoke(pigtom);
        }
    }
}
