using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OatsSpawner : MonoBehaviour
{
    [SerializeField] Oatium _oatiumPrefab;
    [SerializeField] BoxCollider _spawnArea;

    // Start is called before the first frame update
    void Start()
    {
        int totalOats = Game.Instance.TotalOatiumCollected;
        // totalOats = 10;
        for (int i = 0; i < totalOats; i++)
        {
            SpawnOat();
        }
    }

    private void SpawnOat()
    {
        int i = 0;
        Vector3 position = Vector3.zero;
        while (i < 100)
        {
            position = _spawnArea.GetRandomPosition();
            if (Physics.OverlapSphere(position, 1, LayerMask.GetMask("Default")).Length == 0)
            {
                break;
            }
            i++;
        }

        Oatium oatium = Instantiate(_oatiumPrefab, position, Quaternion.identity);
                oatium.transform.parent = transform;
    }
}
