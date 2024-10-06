using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Random = UnityEngine.Random;
using DG.Tweening;


[SelectionBase]
public class Pigtom : MonoBehaviour
{
    [SerializeField] private int minElectronsNum = 1;
    [SerializeField] private int maxElectronsNum = 10;
    [SerializeField] private List<ElectronType> orbitElectrons;
    [SerializeField] private float minOrbitRadius = 5f;
    [SerializeField] private float maxOrbitRadius = 10f;
    [SerializeField] private float minDistanceBetweenOrbits = 1f;
    [SerializeField] private float minOrbitTime = 5f;
    [SerializeField] private float maxOrbitTime = 15f;
    [SerializeField] private Transform nucleus;
    [SerializeField] private float nucleusRadius = 10;


    public float NucleusRadius => nucleusRadius;
    public bool StartedTransformation { get; private set; }

    private List<ElectronOrbit> _orbits = new();
    GameObject _orbitsObj;

    void Start()
    {
        ServiceLocator.Get<PigtomsManager>().AddPigtom(this);

        GetComponent<SphereCollider>().radius = nucleusRadius;

        _orbitsObj = new GameObject("Orbits");
        _orbitsObj.transform.parent = transform;

        minOrbitRadius = Mathf.Max(minOrbitRadius, (nucleusRadius + 1));
        if (maxOrbitRadius < minOrbitRadius)
            maxOrbitRadius = minOrbitRadius + 3;
        nucleus.localScale = Vector3.one * nucleusRadius * 2;

        int orbits = orbitElectrons.Count;

        for (int i = 0; i < orbits; i++)
        {
            CreateNewOrbit(orbitElectrons[i]);
        }
    }

    private void OnElectronNumChanged()
    {
        if (IsReadyForOatium())
        {
            Debug.Log("Ready for Oatium");
            // BecomeOautium();
        }
    }

    private void CreateNewOrbit(ElectronType type)
    {
        int k = 0;  // prevent infinite loop
        float orbitRadius = Random.Range(minOrbitRadius, maxOrbitRadius);
        while (_orbits.Any(e => Mathf.Abs(e.OrbitRadius - orbitRadius) < minDistanceBetweenOrbits) && k < 1000)
        {
            orbitRadius = Random.Range(minOrbitRadius, maxOrbitRadius);
            k++;
        }
        float orbitSpeed = Random.Range(minOrbitTime, maxOrbitTime);
        int electronsNum = Random.Range(minElectronsNum, maxElectronsNum);

        float polarAngle = Random.Range(0, 360);
        float minPolarAngleDiff = 360.0f / (orbitElectrons.Count+1) * 0.8f;
        k = 0;
        while (_orbits.Any(e => Mathf.Abs(Mathf.DeltaAngle(e.PolarAngle * Mathf.Rad2Deg, polarAngle)) < minPolarAngleDiff) && k < 1000)
        {
            polarAngle = Random.Range(0, 360);
            k++;
        }
        polarAngle *= Mathf.Deg2Rad;

        var electronsObj = new GameObject(type.ToString() + " Orbit " + (_orbits.Count+1));
        electronsObj.transform.parent = _orbitsObj.transform;
        var orbit = electronsObj.AddComponent<ElectronOrbit>();
        Debug.Log($"Creating orbit {type} with {electronsNum} electrons");
        orbit.Setup(transform, orbitRadius, orbitSpeed, electronsNum, polarAngle, type, electronsObj.transform);

        orbit.OnElectronsNumChanged += OnElectronNumChanged;

        _orbits.Add(orbit);
    }

    public bool IsReadyForOatium()
    {
        Dictionary<ElectronType, int> electrons = new();

        foreach (var orbit in _orbits)
        {
            var electronsNum = orbit.ElectronsNum;
            if (electrons.ContainsKey(orbit.ElectronType))
            {
                electrons[orbit.ElectronType] += electronsNum;
            }
            else
            {
                electrons.Add(orbit.ElectronType, electronsNum);
            }
        }

        foreach (var electron in electrons)
        {
            if (electron.Value != GameSettings.OatCombination[electron.Key])
            {
                return false;
            }
        }

        return true;
    }

    public void AddLaunchedElectron(Electron electron)
    {
        var orbits = _orbits.Where(e => e.ElectronType == electron.Type).ToList();
        if (orbits.Count == 1)
        {
            orbits[0].AddNewElectron(electron);
        }
        else if (orbits.Count == 0)
        {
            CreateNewOrbit(electron.Type);
            _orbits[_orbits.Count-1].AddNewElectron(electron);
        }
        else
        {
            ElectronOrbit closestOrbit = null;
            float minDistance = float.MaxValue;
            foreach (var orbit in orbits)
            {
                foreach (var e in orbit.Electrons)
                {
                    float distance = Vector3.Distance(electron.transform.position, e.transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestOrbit = orbit;
                    }
                }
            }
            closestOrbit.AddNewElectron(electron);
        }
    }


    public void StartTransformation()
    {
        if (StartedTransformation)
            return;
        StartedTransformation = true;

        if (IsReadyForOatium())
        {
            BecomeOautium();
        }
        else
        {
            BecomeVerbatium();
        }
    }

    public void BecomeOautium()
    {
        SuckElectronsAndShrinkAfter();

        Vector3 position = transform.position;  // closure
        StaticCoroutine.StartInSec(() => Instantiate(GameSettings.OatiumPrefab, 
            position, Quaternion.identity), 6);
    }

    private void SuckElectrons()
    {
        foreach (var orbit in _orbits)
        {
            var electrons = orbit.Electrons.ToList();
            foreach (var electron in electrons)
            {
                orbit.RemoveElectron(electron);
                float moveTime = Random.Range(1f, 2f);
                float delay = Random.Range(0f, 1f);
                electron.transform.DOMove(transform.position, moveTime).SetEase(Ease.InOutSine)
                    .SetDelay(delay)
                    .OnComplete(() => electron.BeDestroyed());
            }
        }
    }

    private void SuckElectronsAndShrinkAfter()
    {
        SuckElectrons();

        transform.DOScale(Vector3.zero, 2).SetEase(Ease.InBack).SetDelay(4)
            .OnComplete(() => Destroy(gameObject));
    }

    public void BecomeVerbatium()
    {
         SuckElectronsAndShrinkAfter();
    }
}
