using System.Collections.Generic;
using Game.Scripts.ScriptableEvents;
using Game.Scripts.ScriptableEvents.Events;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts
{
    [CreateAssetMenu(fileName = "Grid", menuName = "Match3/New Grid", order = 0)]
    public class GridModel : ScriptableObject
    {
        [SerializeField] List<Match3Element> match3Elements;
        [SerializeField] GridValueUpdatedEvent gridValueUpdatedEvent;

        [SerializeField] List<Match3Element> specialElements;
        public List<Match3Element> SpecialElements => specialElements;
        public UnityAction GridUpdated;

        public Grid<GridElement> Grid { get; private set; }

        public void CreateBoard(int width, int height)
        {
            Grid = new Grid<GridElement>(width, height, (CreateNewElement));
            Grid.GridObjectValueUpdated += OnGridValueUpdated;
            Grid.GridUpdated += OnGridUpdated;
            GridUpdated?.Invoke();
        }

        void OnGridUpdated()
        {
            GridUpdated?.Invoke();
        }

        void OnGridValueUpdated(int x, int y, GridElement element)
        {
            gridValueUpdatedEvent.Raise(new GridValueUpdatedArgs(x, y, element));
        }


        public GridElement CreateNewElement(Grid<GridElement> targetGrid, int posX, int posY)
        {
            var instance = new GridElement(targetGrid, posX, posY, match3Elements.RandomWeightedElement());
            return instance;
        }
    }
}