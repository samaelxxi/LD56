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
    [SerializeField] public ElectronOrbit Orbit { get => _orbit; set => SetOrbit(value); }
    public ElectronType Type => _type;


    float _startSpeed;
    Vector3 _moveDirection;
    bool _isLaunched;
    float _launchTime;
    private bool _shouldEmit = true;
    [SerializeField] ElectronType _type;
    [SerializeField] ElectronOrbit _orbit;


    TrailRenderer _trailRenderer;

    public void Awake()
    {
        _startSpeed = _speed;
        _trailRenderer = GetComponent<TrailRenderer>();
        _trailRenderer.widthMultiplier = transform.localScale.x + 0.1f;
    }

    public void SetEmitting(bool shouldEmit)
    {
        if (_shouldEmit == shouldEmit)
            return;

        if (!shouldEmit)
        {
            trailTimeTween?.Kill();
            trailTimeTween = DOTween.To(() => _trailRenderer.time, x => _trailRenderer.time = x, 0, 1.5f)
                .OnComplete(() => _trailRenderer.emitting = false);
        }
        else
        {
            _trailRenderer.emitting = shouldEmit;
            trailTimeTween?.Kill();
            trailTimeTween = DOTween.To(() => _trailRenderer.time, x => _trailRenderer.time = x, 2, 1);
        }

        _shouldEmit = shouldEmit;
    }

    public void FixedUpdate()
    {
        if (_isLaunched)
        {
            TargetPosition = transform.position + _moveDirection;

            var pigtoms = ServiceLocator.Get<PigtomsManager>();  // overlap sphere doesn't work ;(
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

    public void SetOrbit(ElectronOrbit orbit)
    {
        _orbit = orbit;
        float trailTime = (_orbit == null || _orbit.Speed == 0) ? 3 : 1.5f / _orbit.Speed;

        if (_shouldEmit)
        {
            trailTimeTween?.Kill();
            trailTimeTween = DOTween.To(() => _trailRenderer.time, x => _trailRenderer.time = x, trailTime, 1);
        }
    }


    Tween trailTimeTween;

    public void Launch(Vector3 moveDirection)
    {
        _moveDirection = moveDirection.normalized;
        _isLaunched = true;
        _launchTime = Time.time;
        _trailRenderer.time = 2;
    }

    public void BeDestroyed()
    {
        if (Orbit != null)
        {
            Debug.Log("Electron destroyed but not removed from orbit");
            Orbit.RemoveElectron(this);
        }
        SetEmitting(false);

        transform.DOScale(Vector3.zero, 1).SetEase(Ease.InBack)
            .OnComplete(() => Destroy(gameObject));
    }
}
