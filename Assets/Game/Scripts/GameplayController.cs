using System.Collections.Generic;
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
                TryFindMatches(selectedGridElement);
                TryFindMatches(clickedElement);

                selectedGridElement = null;
            }
            else
                selectedGridElement = clickedElement;
        }

        void TryFindMatches(GridElement gridElement)
        {
            var horizontalMatches = FindMatchesFor(1, 0, gridElement);
            horizontalMatches += FindMatchesFor(-1, 0, gridElement);
            var verticalMatches = FindMatchesFor(0, 1, gridElement);
            verticalMatches += FindMatchesFor(0, -1, gridElement);


            Debug.Log($"Matches found {horizontalMatches} - {verticalMatches}");
        }

        int FindMatchesFor(int valueModX, int valueModY, GridElement gridElement)
        {
            var matches = 0;
            var canTest = true;
            var i = 1;
            while (canTest)
            {
                if (gridObject.Grid.PositionInBounds(gridElement.X + valueModX * i, gridElement.Y + valueModY * i))
                {
                    var elementToCheck =
                        gridObject.Grid.GetGridObject(gridElement.X + valueModX * i, gridElement.Y + valueModY * i);
                    if (elementToCheck.AssignedElement == gridElement.AssignedElement)
                    {
                        matches++;
                        i++;
                    }
                    else
                        canTest = false;
                }
                else
                    canTest = false;
            }

            return matches;
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