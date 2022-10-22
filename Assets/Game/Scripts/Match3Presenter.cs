using System.Collections.Generic;
using System.Threading.Tasks;
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
            Debug.Log("Draw grid triggered");
            boardOrigin.DestroyChildren();
            match3ElementControllers = new Match3ElementController[gridModel.Grid.GridReference.GetLength(0),
                gridModel.Grid.GridReference.GetLength(1)];
            for (var i = 0; i < gridModel.Grid.GridReference.GetLength(0); i++)
            {
                for (var j = 0; j < gridModel.Grid.GridReference.GetLength(1); j++)
                {
                    if (gridModel.Grid.GetGridObject(i, j) != null)
                    {
                        var position = new Vector2(i * gridElementSize, j * gridElementSize);
                        var instance = Instantiate(prefab, boardOrigin);
                        instance.transform.localPosition = position;
                        var match3Controller = instance.GetComponent<Match3ElementController>();
                        if (match3Controller == null)
                            continue;
                        match3Controller.Setup(i, j, gridModel);
                        match3ElementControllers[i, j] = match3Controller;
                    }
                }
            }
        }

        public async Task SwapElements(GridElement firstElement, GridElement secondElement)
        {
            var firstController = match3ElementControllers[firstElement.X, firstElement.Y];
            var secondController = match3ElementControllers[secondElement.X, secondElement.Y];
            if (firstController == null || secondController == null)
                return;
            var tasks = new Task[2];
            tasks[0] =firstController.TweenTo(secondController.transform.position);
            tasks[1] =  secondController.TweenTo(firstController.transform.position);
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
    }
}