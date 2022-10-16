using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts
{
    [CreateAssetMenu(fileName = "Match3Element", menuName = "Match3/New Match3 Element", order = 0)]
    public class Match3Element : ScriptableObject
    {
        [SerializeField] [PreviewField] Sprite sprite;

        public Sprite Sprite => sprite;
    }
}