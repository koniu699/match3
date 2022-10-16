using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    public static class Utils
    {
        public static T RandomElement<T>(this List<T> list)
        {
            var randomIndex = Random.Range(0, list.Count);
            return list[randomIndex];
        }

        public static void DestroyChildren(this Transform objectTransform)
        {
            for (var i = objectTransform.childCount -1; i >= 0; i--)
            {
                Object.Destroy(objectTransform.GetChild(i).gameObject);
            }
        }
    }
}