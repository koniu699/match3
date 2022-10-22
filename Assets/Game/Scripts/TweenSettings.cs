using DG.Tweening;
using UnityEngine;

namespace Game.Scripts
{
    [CreateAssetMenu(fileName = "TweenSettings", menuName = "Match3/Tween Settings", order = 0)]
    public class TweenSettings : ScriptableObject
    {
        [SerializeField] float tweenDuration;
        [SerializeField] Ease easeType;

        public float TweenDuration => tweenDuration;

        public Ease EaseType => easeType;
    }
}