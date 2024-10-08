using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using CarterGames.Assets.AudioManager;


[SelectionBase]
public class QuatumPig : MonoBehaviour
{
    [SerializeField] ElectronsTrigger _electronsTrigger;
    [SerializeField] QuatumPigController _controller;
    [SerializeField] Transform _piggoMesh;
    [SerializeField] Transform _piggoFan;
    [SerializeField] AudioSource _assFanAudio;
    [SerializeField] float _assFanVolume = 1;
    
    ElectronOrbit _pigOrbit;

    Electron _preparedForShootElectron;
    float _shootPreparationTime = 0;
    float _fanSpeed = 0;
    PigUI _ui;
    RaycastHit[] _pigtoms = new RaycastHit[1];
    Pigtom pigtomToTransform;


    public void Awake()
    {
        _electronsTrigger.OnElectronEnter += OnElectronActivity;
        _electronsTrigger.OnElectronExit += OnElectronActivity;

        _controller.OnEatElectron += TransformAtomClicked;
        _controller.OnCollectElectron += CollectElectronClicked;
        _controller.OnShootElectron += ShootElectron;
        _controller.ScrollWheelUp += () => OnChangeElectron(true);
        _controller.ScrollWheelDown += () => OnChangeElectron(false);
        _controller.OnEscapePressed += EscapePressed;

        var electronsObj = new GameObject("PigOrbit");
        electronsObj.transform.parent = transform;
        var orbit = electronsObj.AddComponent<ElectronOrbit>();
        orbit.Setup(transform, 2, 5, 0, 10, ElectronType.Red, electronsObj.transform, 0.1f);
        _pigOrbit = orbit;

        _piggoMesh.DOLocalMoveZ(0.1f, 1).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);

        _electronsTrigger.SetIgnoredOrbit(_pigOrbit);

        Debug.Log("Pig created");
        Game.Instance.SetPig(this);
    }

    void Start()
    {
        _ui = Game.Instance.PigUI;
        Game.Instance.StartNewGame();
    }


    private void ShootElectron()
    {
        if (_pigOrbit.ElectronsNum == 0 && _preparedForShootElectron == null)
            return;

        if (_preparedForShootElectron == null)
        {
            PrepareForShooting();
        }
        else
        {
            _preparedForShootElectron.TargetPosition = transform.position + transform.forward * 4;
            _preparedForShootElectron.Launch(transform.forward * 0.1f);
            _preparedForShootElectron.SetEmitting(true);
            Debug.Log("Electron launched");
            _preparedForShootElectron.transform.parent = null;
            _preparedForShootElectron = null;
            AudioManager.Play("electronShot", pitch: 1f.WithVariation(0.07f));

            _controller.AddSomeForce(-transform.forward * 20);
        }
    }

    private void EscapePressed()
    {
        if (Game.Instance.IsPaused)
            Game.Instance.ResumeGame();
        else
            Game.Instance.PauseGame();
    }

    private void OnChangeElectron(bool next)
    {
        if (_preparedForShootElectron == null || _pigOrbit.ElectronsNum == 0)
            return;

        Electron nextElectron;

        if (next)
        {
            nextElectron = _pigOrbit.GetElectron(0);
            AudioManager.Play("electronUp");
        }
        else
        {
            nextElectron = _pigOrbit.GetElectron(_pigOrbit.ElectronsNum - 1);
            AudioManager.Play("electronDown");
        }
        
        _pigOrbit.RemoveElectron(nextElectron);
        _pigOrbit.AddNewElectron(_preparedForShootElectron);
        _preparedForShootElectron = nextElectron;
        _shootPreparationTime = Time.time;
    }

    private void FixedUpdate()
    {
        if (_preparedForShootElectron && Time.time - _shootPreparationTime > 3)
        {
            _pigOrbit.AddNewElectron(_preparedForShootElectron);
            _preparedForShootElectron = null;
        }
        else if (_preparedForShootElectron)
        {
            _preparedForShootElectron.TargetPosition = transform.position + transform.forward * 4;
        }


        // no time to catch all possible events


        ManageUI();
        RotateAssFan();
    }

    private void PrepareForShooting()
    {
        _preparedForShootElectron = _pigOrbit.GetElectron(0);
        _preparedForShootElectron.Orbit.RemoveElectron(_preparedForShootElectron);

        _shootPreparationTime = Time.time;
    }

    private void RotateAssFan()
    {
        float targetSpeed = 40 * Time.deltaTime * _controller.TotalForwardSpeed;
        _fanSpeed = Mathf.MoveTowards(_fanSpeed, targetSpeed, 0.3f);

        float maxFanSpeed = 40 * Time.deltaTime * _controller.MaxSpeed;
        float volume = Mathf.Abs(_fanSpeed) / maxFanSpeed * _assFanVolume;
        _assFanAudio.volume = volume;
        _piggoFan.Rotate(Vector3.up, _fanSpeed, Space.Self);
    }

    private void ManageUI()
    {
        if (CanCollectElectron())
            _ui.OnElectronInSight();
        else
            _ui.OnElectronOutOfSight();

        var pigtom = Physics.RaycastNonAlloc(transform.position, transform.forward, _pigtoms, 6, Globals.PigtomMask);
        pigtomToTransform = null;

        if (pigtom > 0)
        {
            var hit = _pigtoms[0];

            if (hit.collider != null && hit.collider.TryGetComponent(out Pigtom p) &&
                !p.StartedTransformation)
            {
                _ui.SetMakeOatiumActive(true);
                pigtomToTransform = p;
            }
            else
                _ui.SetMakeOatiumActive(false);
        }
        else
            _ui.SetMakeOatiumActive(false);
    }

    private void OnElectronActivity(Electron electron)
    {
        // deprecated
    }

    private void TransformAtomClicked()
    {
        if (pigtomToTransform != null)
        {
            pigtomToTransform.StartTransformation();
            pigtomToTransform = null;
            Debug.Log("Pigtom transformed");
        }
        // else if (_electronsTrigger.NearestElectron != null)
        // {
        //     _electronsTrigger.NearestElectron.gameObject.SetActive(false);
        //     Debug.Log("Electron eaten");
        // }
    }

    float _lastTimeOinked = 0;
    void OnCollisionEnter(Collision other)
    {
        Debug.Log(other.impulse.magnitude);
        if (Time.time - _lastTimeOinked >1 && other.impulse.magnitude > 7)
            Oink();
        
    }

    void Oink()
    {
        AudioManager.PlayGroup("oink", 1, 1f.WithVariation(0.1f));
        _lastTimeOinked = Time.time;
    }

    private void CollectElectronClicked()
    {
        if (CanCollectElectron())
        {
            var electron = _electronsTrigger.NearestElectron;
            if (electron.Orbit != null)
                electron.Orbit.RemoveElectron(electron);
            _pigOrbit.AddNewElectron(electron);
            electron.SetEmitting(false);
            OnElectronActivity(electron);
            AudioManager.Play("electronPickUp", pitch: 1f.WithVariation(0.1f));
            Debug.Log("Electron collected");
        }
    }

    private bool CanCollectElectron()
    {
        var nearestElectron = _electronsTrigger.NearestElectron;

        return _electronsTrigger.NearestElectron != null 
            && _pigOrbit.ElectronsNum < GameSettings.MaxCollectedElectrons
            && _preparedForShootElectron != nearestElectron
                && !nearestElectron.IsLaunched;
    }

    void OnDrawGizmos()
    {
    }
}
