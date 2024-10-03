using System;
using UnityEngine;

namespace Extensions
{
    public static class TypesExtensions
    {
        public static bool IsMonoBehaviour(this Type t)
        {
            return typeof(MonoBehaviour).IsAssignableFrom(t);
        }

        public static bool IsScriptableObject(this Type t)
        {
            return typeof(ScriptableObject).IsAssignableFrom(t);
        }

        public static string TrimLeaderboardName(this string name)
        {
            var idx = name.LastIndexOf("#");
            if (idx != -1)
                name = name[..idx];
            name = name[..Mathf.Min(23, name.Length)];
            return name;
        }
    }
}
