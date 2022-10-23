using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Game.Scripts
{
    public static class Utils
    {
        public static T RandomElement<T>(this List<T> list)
        {
            var randomIndex = Random.Range(0, list.Count);
            return list[randomIndex];
        }

        public static T RandomWeightedElement<T>(this List<T> list) where T : IWeightedElement
        {
            var totalWeight = list.Sum(w => w.Weight);
            var randomWeight = Random.Range(0, totalWeight);
            foreach (var element in list)
            {
                if (randomWeight < element.Weight)
                    return element;
                randomWeight -= element.Weight;
            }

            return default;
        }

        public static void DestroyChildren(this Transform objectTransform)
        {
            for (var i = objectTransform.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(objectTransform.GetChild(i).gameObject);
            }
        }
    }
}