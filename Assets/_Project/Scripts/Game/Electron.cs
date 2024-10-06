using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;


public enum ElectronType { Red, Blue, Green, Orange }


[SelectionBase]
public class Electron : MonoBehaviour
{
    [SerializeField] private float _speed = 0.1f;

    public bool IsLaunched => _isLaunched;
    public Vector3 TargetPosition { get; set; }
    [SerializeField] public ElectronOrbit Orbit { get => _orbit; set => _orbit = value; }
    public ElectronType Type => _type;


    float _startSpeed;
    Vector3 _moveDirection;
    bool _isLaunched;
    float _launchTime;
    [SerializeField] ElectronType _type;
    [SerializeField] ElectronOrbit _orbit;


    public void Awake()
    {
        _startSpeed = _speed;
    }

    public void FixedUpdate()
    {
        if (_isLaunched)
        {
            TargetPosition = transform.position + _moveDirection;

            var pigtoms = ServiceLocator.Get<PigtomsManager>();  // overlap sphere doesn't work ;(
            Debug.Log(pigtoms.Pigtoms.Count());
            foreach (var pigtom in pigtoms.Pigtoms)
            {
                if (Vector3.Distance(transform.position, pigtom.transform.position) < pigtom.NucleusRadius + 2)
                {
                    pigtom.AddLaunchedElectron(this);
                    _speed = _startSpeed;

                    _isLaunched = false;
                }
            }

            if (Time.time - _launchTime > 10)
                BeDestroyed();
        }

        transform.position = Vector3.MoveTowards(transform.position, TargetPosition, _speed);
    }

    public void SetType(ElectronType type)
    {
        _type = type;
    }

    public void Launch(Vector3 moveDirection)
    {
        _moveDirection = moveDirection.normalized;
        _isLaunched = true;
        _launchTime = Time.time;
        _speed *= 3;
    }

    public void BeDestroyed()
    {
        if (Orbit != null)
        {
            Debug.Log("Electron destroyed but not removed from orbit");
            Orbit.RemoveElectron(this);
        }

        transform.DOScale(Vector3.zero, 1).SetEase(Ease.InBack)
            .OnComplete(() => Destroy(gameObject));
    }
}
