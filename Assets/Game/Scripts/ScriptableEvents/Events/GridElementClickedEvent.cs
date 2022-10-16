using ScriptableEvents;
using UnityEngine;

namespace Game.Scripts.ScriptableEvents.Events
{
    [CreateAssetMenu(fileName = "GridElementClickedScriptableEvent", menuName = "Match3/New GridClicked Event")]
    public class GridElementClickedEvent : BaseScriptableEvent<GridElement>
    {
    }
}