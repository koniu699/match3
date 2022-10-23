using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts
{
    public class Match3Presenter : MonoBehaviour
    {
        [SerializeField] GridModel gridModel;
        [SerializeField] Transform boardOrigin;
        [SerializeField] float gridElementSize = 2f;
        [SerializeField] GameObject prefab;

        Match3ElementController[,] match3ElementControllers;

        public void Awake()
        {
            gridModel.GridUpdated += OnGridUpdated;
        }

        void OnGridUpdated()
        {
            // Debug.Log("Draw grid triggered");
            transform.position = new Vector3(gridElementSize * (-gridModel.Grid.GridReference.GetLength(0) + 1) / 2f,
                gridElementSize * (-gridModel.Grid.GridReference.GetLength(1) + 1) / 2f);
            if (match3ElementControllers == null || match3ElementControllers.Length == 0)
                match3ElementControllers = new Match3ElementController[gridModel.Grid.GridReference.GetLength(0),
                    gridModel.Grid.GridReference.GetLength(1)];
            DrawMissingElements();
        }

        [Button]
        public void HardRedraw()
        {
            foreach (var match3ElementController in match3ElementControllers)
            {
                match3ElementController.ElementDestroyed -= OnElementDestroyed;
            }

            boardOrigin.DestroyChildren();
            match3ElementControllers = new Match3ElementController[gridModel.Grid.GridReference.GetLength(0),
                gridModel.Grid.GridReference.GetLength(1)];
            DrawMissingElements();
            ShowElements();
        }

        void DrawMissingElements()
        {
            // Debug.Log("DRAw MISSING ELEMENTS");
            for (var i = 0; i < gridModel.Grid.GridReference.GetLength(0); i++)
            {
                for (var j = 0; j < gridModel.Grid.GridReference.GetLength(1); j++)
                {
                    if (gridModel.Grid.GetGridObject(i, j) != null && match3ElementControllers[i, j] == null)
                    {
                        // Debug.Log($"Creating element for {i}/{j}");
                        var instance = Instantiate(prefab, boardOrigin);
                        instance.name = $"Match3Element [{i},{j}]";
                        var match3Controller = instance.GetComponent<Match3ElementController>();
                        if (match3Controller == null)
                            continue;
                        match3Controller.Setup(i, j, gridModel, gridElementSize);
                        match3Controller.ElementDestroyed += OnElementDestroyed;
                        match3ElementControllers[i, j] = match3Controller;
                    }
                }
            }
        }

        void OnElementDestroyed(int x, int y)
        {
            match3ElementControllers[x, y].ElementDestroyed -= OnElementDestroyed;
            match3ElementControllers[x, y] = null;
        }

        public async Task SwapElements(GridElement firstElement, GridElement secondElement)
        {
            var firstController = match3ElementControllers[firstElement.X, firstElement.Y];
            var secondController = match3ElementControllers[secondElement.X, secondElement.Y];
            if (firstController == null || secondController == null)
                return;
            var tasks = new Task[2];
            tasks[0] = firstController.TweenTo(secondController.transform.position);
            tasks[1] = secondController.TweenTo(firstController.transform.position);
            await Task.WhenAll(tasks);
        }

        public async Task ShowElementsClear(List<GridElement> elements)
        {
            var tasks = new Task[elements.Count];
            for (int i = 0; i < elements.Count; i++)
            {
                var gridElement = elements[i];
                var elemController = match3ElementControllers[gridElement.X, gridElement.Y];
                tasks[i] = elemController.TweenHide();
            }

            await Task.WhenAll(tasks);
        }

        public void DropElement(int originX, int originY, int destinationY)
        {
            match3ElementControllers[originX, originY].Drop(destinationY);
        }

        public async Task ShowSpawnElements(List<Vector2Int> elementsToSpawn)
        {
            var tasks = new Task[elementsToSpawn.Count];
            for (var i = 0; i < elementsToSpawn.Count; i++)
            {
                tasks[i] = match3ElementControllers[elementsToSpawn[i].x, elementsToSpawn[i].y].ShowSpawn();
            }

            await Task.WhenAll(tasks);
        }

        public void ShowElements()
        {
            foreach (var match3ElementController in match3ElementControllers)
            {
                match3ElementController.Show();
            }
        }
    }
}