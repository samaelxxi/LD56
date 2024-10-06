using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ElectronOrbit : MonoBehaviour
{
    public Transform _center;
    [SerializeField] float _orbitRadius = 5f;
    [SerializeField] float _orbitSpeed = 10f;
    [SerializeField] int _electronsNum = 1;
    [SerializeField] float _polarAngle; // Angle from the Y-axis
    [SerializeField] float _polarSpeed = 0.1f;

    public event Action OnElectronsNumChanged;

    public float OrbitRadius => _orbitRadius;
    public float Speed => _orbitSpeed;
    public float PolarAngle => _polarAngle;
    public int ElectronsNum => _electronsNum;
    public ElectronType ElectronType => _electronType;
    public IEnumerable<Electron> Electrons => electronAngles.Keys;

    private Dictionary<Electron, float> electronAngles = new();
    ElectronType _electronType;

    public void Setup(Transform center, float orbitRadius, float orbitTime, 
        int maxElectrons, float polarAngle, ElectronType electronType, Transform parent,
        float polarSpeed = 0)
    {
        _center = center;
        _orbitRadius = orbitRadius;
        _orbitSpeed = 2 * Mathf.PI / orbitTime;
        _electronsNum = 0;
        _polarAngle = polarAngle;
        _polarSpeed = polarSpeed;
        _electronType = electronType;

        var factory = ServiceLocator.Get<ElectronsFactory>();
        for (int i = 0; i < maxElectrons; i++)
        {
            var electron = factory.GetElectron(electronType);
            electron.gameObject.name = $"{_electronType.ToString()} Electron {i}";

            AddElectron(electron, i, maxElectrons);
            electron.transform.position = electron.TargetPosition; // Set the initial position
        }
    }


    public void AddElectron(Electron electron, int index, int maxElectrons)
    {
        // Calculate the angle based on index for initial position
        float angle = 2 * Mathf.PI * index / maxElectrons;
        electronAngles.Add(electron, angle);
        UpdateElectronPosition(electron); // Set the initial position
        electron.Orbit = this;
        electron.transform.parent = transform;
        _electronsNum++;

        OnElectronsNumChanged?.Invoke();
    }

    public void AddNewElectron(Electron electron)
    {
        AddElectron(electron, _electronsNum, _electronsNum+1);

        if (_electronsNum == 1)
            return;

        List<float> angles = new();
        foreach (var e in electronAngles.Keys)
        {
            if (e == electron)
                continue;
            angles.Add(electronAngles[e]);
        }
        float farthestAngle = MathExtensions.FindFarthestPoint(angles) * Mathf.Deg2Rad;

        electronAngles[electron] = farthestAngle;
    }

    public Electron GetElectron(int index)
    {
        return electronAngles.Keys.ToList()[index];
    }

    public void RemoveElectron(Electron electron)
    {
        electronAngles.Remove(electron);
        _electronsNum--;
        electron.SetOrbit(null);

        OnElectronsNumChanged?.Invoke();
    }

    public void FixedUpdate()
    {
        foreach (var electron in electronAngles.Keys.ToList())
        {
            var angle = electronAngles[electron];
            angle += _orbitSpeed * Time.deltaTime; // Update the angle for the next frame
            electronAngles[electron] = angle;
        }
        // Update angles for circular motion
        _polarAngle += _orbitSpeed * _polarSpeed * Time.deltaTime; // Vertical rotation (slower for a better effect)

        foreach (var electron in electronAngles.Keys)
        {
            UpdateElectronPosition(electron);
        }
    }

    private void UpdateElectronPosition(Electron electron)
    {
        // Calculate the new position for the electron
        Vector3 newPosition = GetElectronPosition(electron);
        
        // Set the position of the electron
        Vector3 targetPos = _center.position + newPosition;
        electron.TargetPosition = targetPos;
    }

    private Vector3 GetElectronPosition(Electron electron)
    {
        // Find the angle of the electron in the orbit
        var angle = electronAngles[electron];

        // Calculate the x, y, and z positions using spherical coordinates
        float x = _orbitRadius * Mathf.Cos(angle) * Mathf.Cos(_polarAngle);
        float y = _orbitRadius * Mathf.Sin(angle);
        float z = _orbitRadius * Mathf.Cos(angle) * Mathf.Sin(_polarAngle);

        // Debug.Log($"{x}, {y}, {z}");
        // Debug.LogWarning($"New angle: {angle}");

        return new Vector3(x, y, z);
    }

}