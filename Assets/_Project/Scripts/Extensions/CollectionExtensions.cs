using System.Collections;
using System.Collections.Generic;
using System;

namespace UnavinarML.General
{
    public static class CollectionExtensions
    {
        public static T RandomElement<T>(this List<T> list)
        {
            if (list.Count == 0)
                return default;
            else if (list.Count == 1)
                return list[0];

            int index = UnityEngine.Random.Range(0, list.Count);

            return list[index];
        }
    }
}
