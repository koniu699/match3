using Game.Scripts.ScriptableEvents;
using Game.Scripts.ScriptableEvents.Events;
using UnityEngine;

namespace Game.Scripts
{
    public class GameplayController : MonoBehaviour
    {
        [SerializeField] GridModel gridObject;
        [SerializeField] GridElementClickedEvent gridElementClicked;
        [SerializeField] Vector2Int gridDimensions;
        [SerializeField] GridValueUpdatedEvent gridValueUpdatedEvent;

        GridElement selectedGridElement;

        void Awake()
        {
            gridElementClicked.AddListener(OnGridElementClicked);
        }

        void OnGridElementClicked(GridElement clickedElement)
        {
            if (selectedGridElement == null || selectedGridElement == clickedElement)
            {
                selectedGridElement = clickedElement;
                return;
            }

            if (selectedGridElement.IsNeighbour(clickedElement))
            {
                var selectedValue = selectedGridElement.AssignedElement;
                var clickedValue = clickedElement.AssignedElement;
                selectedGridElement.SetValue(clickedValue);
                clickedElement.SetValue(selectedValue);
                selectedGridElement = null;
            }
            else 
                selectedGridElement = clickedElement;
        }

        public void Start()
        {
            if (gridObject == null)
            {
                Debug.LogError("Grid Object not assigned! Please fix.");
                return;
            }

            gridObject.CreateBoard(gridDimensions.x, gridDimensions.y);
        }

        void OnDestroy()
        {
            gridElementClicked.RemoveListener(OnGridElementClicked);
        }
    }
}