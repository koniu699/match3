using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts
{
    [CreateAssetMenu(fileName = "Match3Element", menuName = "Match3/New Match3 Element", order = 0)]
    public class Match3Element : ScriptableObject, IWeightedElement
    {
        [SerializeField] [PreviewField] Sprite sprite;
        [SerializeField] uint weight;

        public Sprite Sprite => sprite;
        public uint Weight => weight;
    }
}