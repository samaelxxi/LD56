using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class ColliderExtensions
{
    public static Vector3 GetRandomPosition(this BoxCollider boxCollider)
    {
        var center = boxCollider.transform.TransformPoint(boxCollider.center);
        var size = boxCollider.transform.TransformVector(boxCollider.size);
        var x = Random.Range(center.x - size.x / 2, center.x + size.x / 2);
        var y = Random.Range(center.y - size.y / 2, center.y + size.y / 2);
        var z = Random.Range(center.z - size.z / 2, center.z + size.z / 2);
        return new Vector3(x, y, z);
    }
}
