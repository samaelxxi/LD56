using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;


[SelectionBase]
public class QuatumPig : MonoBehaviour
{
    [SerializeField] ElectronsTrigger _electronsTrigger;
    [SerializeField] QuatumPigController _controller;
    [SerializeField] Transform _piggoMesh;
    [SerializeField] Transform _piggoFan;
    
    ElectronOrbit _electronOrbit;

    Electron _preparedForShootElectron;
    float _shootPreparationTime = 0;
    float _fanSpeed = 0;


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

        _piggoMesh.DOLocalMoveZ(0.1f, 1).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);

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
        if (CanCollectElectron())
            ServiceLocator.Get<PigUI>().OnElectronInSight();
        else
            ServiceLocator.Get<PigUI>().OnElectronOutOfSight();

        // // Get the local rotation as a Quaternion
        // Quaternion localRotation = _piggoFan.localRotation;

        // // Step 2: Create a rotation only for the x-axis increment (based on speed and deltaTime)
        float targetSpeed = 60 * Time.deltaTime * _controller.TotalForwardSpeed;
        _fanSpeed = Mathf.MoveTowards(_fanSpeed, targetSpeed, 0.2f);

        _piggoFan.Rotate(Vector3.up, _fanSpeed, Space.Self);


        // // Step 3: Apply this increment to the current local rotation
        // _piggoFan.localRotation = localRotation * xRotationIncrement;

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
