using System.Collections.Generic;
using Game.Scripts.ScriptableEvents.Events;
using Sirenix.Utilities;
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
                var selectedValue = selectedGridElement.Match3Element;
                var clickedValue = clickedElement.Match3Element;
                selectedGridElement.SetValue(clickedValue);
                clickedElement.SetValue(selectedValue);
                var selectedElemMatches = TryFindMatches(selectedGridElement);
                var clickedElemMatches = TryFindMatches(clickedElement);
                
                ClearGridElements(selectedElemMatches);
                ClearGridElements(clickedElemMatches);
                gridObject.Grid.GridUpdated?.Invoke();
                selectedGridElement = null;
            }
            else
                selectedGridElement = clickedElement;
        }

        HashSet<GridElement> TryFindMatches(GridElement gridElement)
        {
            var horizontalMatches = FindMatchesFor(1, 0, gridElement);
            horizontalMatches.AddRange(FindMatchesFor(-1, 0 , gridElement));
            var verticalMatches = FindMatchesFor(0, 1, gridElement);
            verticalMatches.AddRange(FindMatchesFor(0,-1, gridElement));

            horizontalMatches.Add(gridElement);
            verticalMatches.Add(gridElement);
            var totalMatches = new HashSet<GridElement>();
            if (horizontalMatches.Count >= 3)
                totalMatches.AddRange(horizontalMatches);
            if (verticalMatches.Count >= 3)
                totalMatches.AddRange(verticalMatches);
            Debug.Log($"Matches found {horizontalMatches.Count} - {verticalMatches.Count}");
            return totalMatches;
        }

        void ClearGridElements(HashSet<GridElement> elements)
        {
            foreach (var element in elements)
            {
                gridObject.Grid.GetGridObject(element.X, element.Y).ElementDestroyed();
                gridObject.Grid.SetGridObject(element.X, element.Y, null);
            }
        }

        HashSet<GridElement> FindMatchesFor(int valueModX, int valueModY, GridElement gridElement)
        {
            var matches = new HashSet<GridElement>();
            var canTest = true;
            var i = 1;
            while (canTest)
            {
                if (gridObject.Grid.PositionInBounds(gridElement.X + valueModX * i, gridElement.Y + valueModY * i))
                {
                    var elementToCheck =
                        gridObject.Grid.GetGridObject(gridElement.X + valueModX * i, gridElement.Y + valueModY * i);
                    if (elementToCheck.Match3Element == gridElement.Match3Element)
                    {
                        matches.Add(elementToCheck);
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