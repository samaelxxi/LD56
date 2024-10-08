using System.Collections;
using System.Collections.Generic;
using UnavinarML.General;
using UnityEngine;

public class WorldCreator : MonoBehaviour
{
    [SerializeField] private WorldCreatorSettings _worldCreatorSettings;
    [SerializeField] private float _maxIteration = 1000;
    [SerializeField] private float smallStepSize = 0.1f;
    [SerializeField] private bool _realGame = true;


    List<Pigtom> _objects = new();
    List<Tapke> _tapkes = new();

    GameObject _pigtoms;
    Vector3 spaceMin, spaceMax;


    void Start()
    {
        if (_realGame)
            _worldCreatorSettings = GameSettings.WorldCreatorSettings;

        GenerateWorld();
    }


    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            // RebuildWorld();
        }
    }

    void GenerateWorld()
    {
        InitPigtoms();
        AdjustPositions();
        MakeSomeHoles();

        SprinkleSomeTapkes();

        // Voila
    }

    public void RebuildWorld()
    {
        foreach (var obj in _objects)
            Destroy(obj.gameObject);
        _objects.Clear();
        Destroy(_pigtoms);

        foreach (var tapke in _tapkes)
            Destroy(tapke.gameObject);
        _tapkes.Clear();

        GenerateWorld();
    }

    void InitPigtoms()
    {
        _pigtoms = new GameObject("Pigtoms");

        spaceMin = _worldCreatorSettings.WorldCenter - _worldCreatorSettings.WorldSize * 0.5f;
        spaceMax = _worldCreatorSettings.WorldCenter + _worldCreatorSettings.WorldSize * 0.5f;

        int pigtomToMake = _worldCreatorSettings.PigtomsQuantity + _worldCreatorSettings.PigtomHoles;

        for (int i = 0; i < pigtomToMake; i++)
        {
            Vector3 pos = RandomWorldPosition();

            // Assign random radii if objects have different sizes
            float radius = Random.Range(0.5f, 2f); // Example: radii between 0.5 and 2 units

            var pigtom = CreatePigtom();
            pigtom.transform.position = pos;
            _objects.Add(pigtom);
        }
    }

    // Step 2 & 3: Define Repulsion Forces and Iterative Adjustment
    void AdjustPositions()
    {
        for (int iteration = 0; iteration < _maxIteration; iteration++)
        {
            bool hasMovement = false;
            float distanceVariation = _worldCreatorSettings.DesiredAverageDistanceDeviation;
            float desiredAverageDistance = _worldCreatorSettings.DesiredAverageDistance;

            // Create a copy of positions to calculate new positions without interference
            List<Vector3> newPositions = new();
            foreach (var obj in _objects)
            {
                newPositions.Add(obj.transform.position);
            }

            for (int i = 0; i < _objects.Count; i++)
            {
                Vector3 totalForce = Vector3.zero;
                Pigtom objA = _objects[i];

                for (int j = 0; j < _objects.Count; j++)
                {
                    if (i == j) continue;

                    Pigtom objB = _objects[j];
                    Vector3 direction = objA.transform.position - objB.transform.position;
                    float distance = direction.magnitude;

                    // Calculate desired minimum distance considering radii and variation
                    float variationFactor = 1 + Random.Range(-distanceVariation, distanceVariation);
                    float desiredDistance = desiredAverageDistance * variationFactor + objA.NucleusRadius + objB.NucleusRadius;

                    if (distance < desiredDistance && distance > 0)
                    {
                        // Normalize the direction vector
                        Vector3 forceDirection = direction.normalized;
                        // Calculate the magnitude of the repulsion force
                        float forceMagnitude = (desiredDistance - distance) / desiredDistance;

                        // Accumulate the force
                        totalForce += forceDirection * forceMagnitude;
                    }

                    // New: Pull objects closer if they go too far from average distance
                    else if (distance > desiredDistance * 1.5f)
                    {
                        Vector3 pullBackForce = -direction.normalized * (distance - desiredDistance) * 0.001f;
                        // totalForce += pullBackForce;
                    }
                }

                // Apply the accumulated force to the position
                Vector3 displacement = totalForce * smallStepSize;
                if (displacement != Vector3.zero)
                {
                    hasMovement = true;
                    newPositions[i] += displacement;
                }
            }

            // Update positions
            for (int i = 0; i < _objects.Count; i++)
            {
                _objects[i].transform.position = newPositions[i];
                // Optionally, enforce boundary conditions
                _objects[i].transform.position = new Vector3(
                    Mathf.Clamp(_objects[i].transform.position.x, spaceMin.x, spaceMax.x),
                    Mathf.Clamp(_objects[i].transform.position.y, spaceMin.y, spaceMax.y),
                    Mathf.Clamp(_objects[i].transform.position.z, spaceMin.z, spaceMax.z)
                );
            }

            // Terminate early if no movement occurs
            if (!hasMovement)
            {
                Debug.Log($"Converged after {iteration} iterations.");
                break;
            }

            // Optionally, log progress
            if (iteration % 100 == 0)
            {
                Debug.Log($"Iteration {iteration}");
            }
        }
    }

    void MakeSomeHoles()
    {
        for (int i = 0; i < _worldCreatorSettings.PigtomHoles; i++)
        {
            var myPoorPigtomToBeDestroyed = _objects.RandomElement();
            _objects.Remove(myPoorPigtomToBeDestroyed);
            Destroy(myPoorPigtomToBeDestroyed.gameObject);
        }
    }

    Pigtom CreatePigtom()
    {
        Pigtom pigtom = Instantiate(_worldCreatorSettings.PigtomPrefab, _pigtoms.transform);

        float radius = _worldCreatorSettings.PigtomRadius.WithVariation(_worldCreatorSettings.PigtomRadiusDeviation);
        pigtom.nucleusRadius = radius;
        pigtom.minElectronsNum = _worldCreatorSettings.MinElectronsPerPigtom;
        pigtom.maxElectronsNum = _worldCreatorSettings.MaxElectronsPerPigtom;
        pigtom.minOrbitTime = _worldCreatorSettings.MinElectronOrbitingTime;
        pigtom.maxOrbitTime = _worldCreatorSettings.MaxElectronOrbitingTime;
        pigtom.minOrbitRadius = radius + 1;
        pigtom.minOrbitRadius = radius + 1 + _worldCreatorSettings.MinOrbitRadius;
        pigtom.maxOrbitRadius = radius + 1 + _worldCreatorSettings.MaxOrbitRadius;

        return pigtom;
    }


    void SprinkleSomeTapkes()
    {
        int tapkesCount = Random.Range(_worldCreatorSettings.MinTapkes, _worldCreatorSettings.MaxTapkes);
        for (int i = 0; i < tapkesCount; i++)
        {
            Vector3 tapkePos;
            while (true)
            {
                tapkePos = RandomWorldPosition();
                if (Physics.OverlapSphere(tapkePos, 3, Globals.PigtomMask).Length == 0)
                    break;
            }

            var tapke = Instantiate(GameSettings.TapkePrefab, tapkePos, Quaternion.identity);
            _tapkes.Add(tapke);
        }
    }

    Vector3 RandomWorldPosition()
    {
        return new Vector3(
            Random.Range(spaceMin.x, spaceMax.x),
            Random.Range(spaceMin.y, spaceMax.y),
            Random.Range(spaceMin.z, spaceMax.z)
        );
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_worldCreatorSettings.WorldCenter, _worldCreatorSettings.WorldSize);
    }
}
