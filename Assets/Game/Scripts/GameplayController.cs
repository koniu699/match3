using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        [SerializeField] Match3Presenter match3Presenter;

        GridElement selectedGridElement;

        bool inputBlocked;

        void Awake()
        {
            gridElementClicked.AddListener(OnGridElementClicked);
        }

        void Start()
        {
            if (gridObject == null)
            {
                Debug.LogError("Grid Object not assigned! Please fix.");
                return;
            }

            gridObject.CreateBoard(gridDimensions.x, gridDimensions.y);

#pragma warning disable CS4014
            CheckAndClearMatches(false);
#pragma warning restore CS4014

            match3Presenter.ShowElements();
        }

        void OnGridElementClicked(GridElement clickedElement)
        {
            HandleElementClicked(clickedElement);
        }

        async void HandleElementClicked(GridElement clickedElement)
        {
            if (inputBlocked)
                return;
            if (selectedGridElement == null || selectedGridElement == clickedElement)
            {
                selectedGridElement = clickedElement;
                return;
            }

            inputBlocked = true;
            if (selectedGridElement.IsNeighbour(clickedElement))
            {
                await SwapGridElements(selectedGridElement, clickedElement);
                var selectedElemMatches = TryFindMatches(selectedGridElement);
                var clickedElemMatches = TryFindMatches(clickedElement);

                selectedElemMatches.AddRange(clickedElemMatches);
                await AnimateClearGridElements(selectedElemMatches);

                DropElements();
                var spawnedElements = FillRandomElements();
                gridObject.Grid.GridUpdated?.Invoke();
                // await match3Presenter.ShowSpawnElements(spawnedElements);
                match3Presenter.HardRedraw();
                selectedGridElement = null;

                await CheckAndClearMatches();
            }
            else
                selectedGridElement = clickedElement;
        }

        async Task CheckAndClearMatches(bool useAnimations = true)
        {
            while (true)
            {
                var matches = new HashSet<GridElement>();
                for (var i = 0; i < gridObject.Grid.GridReference.GetLength(0); i++)
                {
                    for (var j = 0; j < gridObject.Grid.GridReference.GetLength(1); j++)
                    {
                        matches.AddRange(TryFindMatches(gridObject.Grid.GetGridObject(i, j)));
                    }
                }

                if (matches.Count <= 0)
                {
                    inputBlocked = false;
                    break;
                }

                if (useAnimations)
                    await AnimateClearGridElements(matches);
                else
                    ClearGridElements(matches);
                DropElements();
                FillRandomElements();
                gridObject.Grid.GridUpdated?.Invoke();
                match3Presenter.HardRedraw();
            }
        }

        List<Vector2Int> FillRandomElements()
        {
            var gridX = gridObject.Grid.GridReference.GetLength(0);
            var gridY = gridObject.Grid.GridReference.GetLength(1);
            var elementsToSpawn = new List<Vector2Int>(gridX * gridY);
            for (var i = 0; i < gridX; i++)
            {
                for (var j = 0; j < gridY; j++)
                {
                    if (gridObject.Grid.GetGridObject(i, j) != null)
                        continue;
                    gridObject.Grid.SetGridObject(i, j, gridObject.CreateNewElement(gridObject.Grid, i, j));
                    elementsToSpawn.Add(new Vector2Int(i, j));
                }
            }

            return elementsToSpawn;
        }

        async Task SwapGridElements(GridElement firstElement, GridElement secondElement)
        {
            await match3Presenter.SwapElements(firstElement, secondElement);
            var selectedValue = firstElement.Match3Element;
            var clickedValue = secondElement.Match3Element;
            firstElement.SetValue(clickedValue);
            secondElement.SetValue(selectedValue);
        }

        void DropElements()
        {
            for (var i = 0; i < gridObject.Grid.GridReference.GetLength(0); i++)
            {
                for (var j = 0; j < gridObject.Grid.GridReference.GetLength(1); j++)
                {
                    if (gridObject.Grid.GetGridObject(i, j) == null)
                    {
                        TryDropElementAbove(i, j);
                    }
                }
            }
        }

        void TryDropElementAbove(int i, int j)
        {
            for (var k = 1; k < gridObject.Grid.GridReference.GetLength(1); k++)
            {
                if (!gridObject.Grid.PositionInBounds(i, j + k))
                    break;
                if (gridObject.Grid.GetGridObject(i, j + k) != null)
                {
                    // match3Presenter.DropElement(i, j + k, j);
                    gridObject.Grid.SetGridObject(i, j,
                        new GridElement(gridObject.Grid, i, j,
                            gridObject.Grid.GetGridObject(i, j + k).Match3Element));
                    gridObject.Grid.SetGridObject(i, j + k, null);
                    break;
                }
            }
        }

        HashSet<GridElement> TryFindMatches(GridElement gridElement)
        {
            var horizontalMatches = FindMatchesFor(1, 0, gridElement);
            horizontalMatches.AddRange(FindMatchesFor(-1, 0, gridElement));
            var verticalMatches = FindMatchesFor(0, 1, gridElement);
            verticalMatches.AddRange(FindMatchesFor(0, -1, gridElement));

            horizontalMatches.Add(gridElement);
            verticalMatches.Add(gridElement);
            var totalMatches = new HashSet<GridElement>();
            if (horizontalMatches.Count >= 3)
                totalMatches.AddRange(horizontalMatches);
            if (verticalMatches.Count >= 3)
                totalMatches.AddRange(verticalMatches);
            return totalMatches;
        }

        async Task AnimateClearGridElements(HashSet<GridElement> elements)
        {
            await match3Presenter.ShowElementsClear(elements.ToList());
            ClearGridElements(elements);
        }

        void ClearGridElements(HashSet<GridElement> elements)
        {
            foreach (var element in elements)
            {
                DestroyGridElement(element);
            }
        }

        void DestroyGridElement(GridElement element)
        {
            gridObject.Grid.GetGridObject(element.X, element.Y)?.Destroy();
            gridObject.Grid.SetGridObject(element.X, element.Y, null);
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
                    var isSpecialMatch = gridObject.SpecialElements.Contains(elementToCheck.Match3Element);
                    if (elementToCheck.Match3Element == gridElement.Match3Element || isSpecialMatch)
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

        void OnDestroy()
        {
            gridElementClicked.RemoveListener(OnGridElementClicked);
        }
    }
}