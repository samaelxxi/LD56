using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ElectronOrbit : MonoBehaviour
{
    public Pigtom _pigtom;
    [SerializeField] float _orbitRadius = 5f;
    [SerializeField] float _orbitSpeed = 10f;
    [SerializeField] int _electronsNum = 1;
    [SerializeField] float _polarAngle; // Angle from the Y-axis

    public float OrbitRadius => _orbitRadius;
    public float PolarAngle => _polarAngle;

    private Dictionary<Electron, float> electronAngles = new();

    public void Setup(Pigtom pigtom, float orbitRadius, float orbitTime, 
        int electronsNum, float polarAngle, Electron electronPrefab, Transform parent)
    {
        _pigtom = pigtom;
        _orbitRadius = orbitRadius;
        _orbitSpeed = 2 * Mathf.PI / orbitTime;
        _electronsNum = electronsNum;
        _polarAngle = polarAngle;

        for (int i = 0; i < electronsNum; i++)
        {
            var electron = Instantiate(electronPrefab, parent);
            AddElectron(electron, i); // Pass the index to position correctly
        }
    }

    public void AddElectron(Electron electron, int index)
    {
        // Calculate the angle based on index for initial position
        float angle = 2 * Mathf.PI * index / _electronsNum;
        electronAngles.Add(electron, angle);
        UpdateElectronPosition(electron); // Set the initial position
    }

    public void RemoveElectron(Electron electron)
    {
        electronAngles.Remove(electron);
        _electronsNum--;
    }

    public void Orbit()
    {
        foreach (var electron in electronAngles.Keys.ToList())
        {
            var angle = electronAngles[electron];
            angle += _orbitSpeed * Time.deltaTime; // Update the angle for the next frame
            electronAngles[electron] = angle;
        }
        // Update angles for circular motion
        // _polarAngle += _orbitSpeed * 0.5f * Time.deltaTime; // Vertical rotation (slower for a better effect)

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
        electron.transform.localPosition = _pigtom.transform.position + newPosition;
    }

    private Vector3 GetElectronPosition(Electron electron)
    {
        // Find the angle of the electron in the orbit
        var angle = electronAngles[electron];

        // Calculate the x, y, and z positions using spherical coordinates
        float x = _orbitRadius * Mathf.Cos(angle) * Mathf.Cos(_polarAngle);
        float y = _orbitRadius * Mathf.Sin(angle);
        float z = _orbitRadius * Mathf.Cos(angle) * Mathf.Sin(_polarAngle);

        // float x = _orbitRadius * Mathf.Sin(Mathf.PI / 4) * Mathf.Cos(angle);
        // float y = _orbitRadius * Mathf.Sin(Mathf.PI / 4) * Mathf.Sin(angle);
        // float z = _orbitRadius * Mathf.Cos(Mathf.PI / 4) + 2 * Mathf.Sin(_orbitSpeed * Time.time); // Oscillation in z


        return new Vector3(x, y, z);
    }
}