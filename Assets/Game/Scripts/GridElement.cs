using System;

namespace Game.Scripts
{
    public class GridElement
    {
        Grid<GridElement> grid;

        public Match3Element AssignedElement { get; private set; }
        public int X { get; }

        public int Y { get; }

        public GridElement(Grid<GridElement> targetGrid, int posX, int posY, Match3Element element)
        {
            X = posX;
            Y = posY;
            AssignedElement = element;
            grid = targetGrid;
        }

        public bool IsNeighbour(GridElement testElement)
        {
            var absX = Math.Abs(X - testElement.X);
            var absY = Math.Abs(Y - testElement.Y);
            return absX <= 1 && absY <= 1 && absX != absY;
        }

        public void SetValue(Match3Element newValue)
        {
            AssignedElement = newValue;
            grid.TriggerGridObjectValueUpdated(X, Y);
        }
    }
}