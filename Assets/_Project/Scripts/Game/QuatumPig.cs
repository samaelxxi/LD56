using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[SelectionBase]
public class QuatumPig : MonoBehaviour
{
    [SerializeField] ElectronsTrigger _electronsTrigger;
    [SerializeField] QuatumPigController _controller;
    
    ElectronOrbit _electronOrbit;

    Electron _preparedForShootElectron;
    float _shootPreparationTime = 0;


    public void Awake()
    {
        _electronsTrigger.OnElectronEnter += OnElectronActivity;
        _electronsTrigger.OnElectronExit += OnElectronActivity;

        _controller.OnEatElectron += EatElectronClicked;
        _controller.OnCollectElectron += CollectElectronClicked;
        _controller.OnShootElectron += ShootElectron;
        _controller.ScrollWheelUp += () => OnChangeElectron(true);
        _controller.ScrollWheelDown += () => OnChangeElectron(false);

        var electronsObj = new GameObject("PigOrbit");
        electronsObj.transform.parent = transform;
        var orbit = electronsObj.AddComponent<ElectronOrbit>();
        orbit.Setup(transform, 1.5f, 5, 0, 10, ElectronType.Red, electronsObj.transform, 0.1f);
        _electronOrbit = orbit;

        _electronsTrigger.SetIgnoredOrbit(_electronOrbit);
    }


    private void ShootElectron()
    {
        if (_electronOrbit.ElectronsNum == 0 && _preparedForShootElectron == null)
            return;

        if (_preparedForShootElectron == null)
        {
            PrepareForShooting();
        }
        else
        {
            _preparedForShootElectron.TargetPosition = transform.position + transform.forward * 2;
            _preparedForShootElectron.Launch(transform.forward * 0.1f);
            Debug.Log("Electron launched");
            _preparedForShootElectron.transform.parent = null;
            _preparedForShootElectron = null;
        }
    }

    private void OnChangeElectron(bool next)
    {
        if (_preparedForShootElectron == null || _electronOrbit.ElectronsNum == 0)
            return;

        Electron nextElectron;

        if (next)
            nextElectron = _electronOrbit.GetElectron(0);
        else
            nextElectron = _electronOrbit.GetElectron(_electronOrbit.ElectronsNum - 1);
        
        _electronOrbit.RemoveElectron(nextElectron);
        _electronOrbit.AddNewElectron(_preparedForShootElectron);
        _preparedForShootElectron = nextElectron;
    }

    private void FixedUpdate()
    {
        if (_preparedForShootElectron && Time.time - _shootPreparationTime > 3)
        {
            _electronOrbit.AddNewElectron(_preparedForShootElectron);
            _preparedForShootElectron = null;
        }
        else if (_preparedForShootElectron)
        {
            _preparedForShootElectron.TargetPosition = transform.position + transform.forward * 2;
        }


        // no time to catch all possible events
        var nearestElectron = _electronsTrigger.NearestElectron;

        if (CanCollectElectron())
        {
            // Debug.Log(@$"{_electronsTrigger.NearestElectron} != null  :
            // {_electronsTrigger.NearestElectron != null}
            // && {_electronOrbit.ElectronsNum < GameSettings.MaxCollectedElectrons}
            // && {_preparedForShootElectron != nearestElectron}
            //     && {!nearestElectron.IsLaunched}");


            ServiceLocator.Get<PigUI>().OnElectronInSight();
        }
        else
            ServiceLocator.Get<PigUI>().OnElectronOutOfSight();
    }

    private void PrepareForShooting()
    {
        _preparedForShootElectron = _electronOrbit.GetElectron(0);
        _preparedForShootElectron.Orbit.RemoveElectron(_preparedForShootElectron);

        _shootPreparationTime = Time.time;
    }

    private void OnElectronActivity(Electron electron)
    {
        // if (_electronsTrigger.NearestElectron != null && 
        //         _electronOrbit.ElectronsNum < GameSettings.MaxCollectedElectrons)
        //     ServiceLocator.Get<PigUI>().OnElectronInSight();
        // else
        //     ServiceLocator.Get<PigUI>().OnElectronOutOfSight();
    }

    private void EatElectronClicked()
    {
        if (_electronsTrigger.NearestElectron != null)
        {
            _electronsTrigger.NearestElectron.gameObject.SetActive(false);
            Debug.Log("Electron eaten");
        }
    }

    private void CollectElectronClicked()
    {
        if (CanCollectElectron())
        {
            var electron = _electronsTrigger.NearestElectron;
            if (electron.Orbit != null)
                electron.Orbit.RemoveElectron(electron);
            _electronOrbit.AddNewElectron(electron);
            OnElectronActivity(electron);
            Debug.Log("Electron collected");
        }
    }

    private bool CanCollectElectron()
    {
        var nearestElectron = _electronsTrigger.NearestElectron;

        return _electronsTrigger.NearestElectron != null 
            && _electronOrbit.ElectronsNum < GameSettings.MaxCollectedElectrons
            && _preparedForShootElectron != nearestElectron
                && !nearestElectron.IsLaunched;
    }
}
