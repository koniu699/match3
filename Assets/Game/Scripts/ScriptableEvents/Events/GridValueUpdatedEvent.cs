using ScriptableEvents;
using UnityEngine;

namespace Game.Scripts.ScriptableEvents.Events
{
    [CreateAssetMenu(fileName = "GridValueUpdatedEvent", menuName = "Match3/New GridValueUpdatedEvent", order = 0)]
    public class GridValueUpdatedEvent : BaseScriptableEvent<GridValueUpdatedArgs>
    {
        
    }
}