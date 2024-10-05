using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Random = UnityEngine.Random;

public class Pigtom : MonoBehaviour
{
    [SerializeField] private int minElectronsNum = 1;
    [SerializeField] private int maxElectronsNum = 10;
    [SerializeField] private List<Electron> orbitElectrons;
    [SerializeField] private float minOrbitRadius = 5f;
    [SerializeField] private float maxOrbitRadius = 10f;
    [SerializeField] private float minDistanceBetweenOrbits = 1f;
    [SerializeField] private float minOrbitTime = 5f;
    [SerializeField] private float maxOrbitTime = 15f;



    private List<ElectronOrbit> _orbits = new();

    void Start()
    {
        var orbitObj = new GameObject("Orbits");
        orbitObj.transform.parent = transform;

        int orbits = orbitElectrons.Count;

        for (int i = 0; i < orbits; i++)
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
            float minPolarAngleDiff = 360.0f / orbits * 0.8f;
            k = 0;
            while (_orbits.Any(e => Mathf.Abs(Mathf.DeltaAngle(e.PolarAngle * Mathf.Rad2Deg, polarAngle)) < minPolarAngleDiff) && k < 1000)
            {
                polarAngle = Random.Range(0, 360);
                k++;
            }

            Debug.Log($"{polarAngle}, {k} {minPolarAngleDiff}");
            polarAngle *= Mathf.Deg2Rad;

            var electronsObj = new GameObject("Orbit " + i);
            electronsObj.transform.parent = orbitObj.transform;
            var orbit = electronsObj.AddComponent<ElectronOrbit>();
            var prefab = orbitElectrons[i];
            orbit.Setup(this, orbitRadius, orbitSpeed, electronsNum, polarAngle, prefab, electronsObj.transform);

            _orbits.Add(orbit);
        }
    }

    void FixedUpdate()
    {
        foreach (var orbit in _orbits)
        {
            orbit.Orbit();
        }
    }
}
