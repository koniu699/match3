using System;

namespace Game.Scripts
{
    public class GridElement
    {
        int x;
        int y;
        Grid<GridElement> grid;

        public Match3Element AssignedElement { get; private set; }

        public GridElement(Grid<GridElement> targetGrid, int posX, int posY, Match3Element element)
        {
            x = posX;
            y = posY;
            AssignedElement = element;
            grid = targetGrid;
        }

        public bool IsNeighbour(GridElement testElement)
        {
            var absX = Math.Abs(x - testElement.x);
            var absY = Math.Abs(y - testElement.y);
            return absX <= 1 && absY <= 1 && absX != absY;
        }

        public void SetValue(Match3Element newValue)
        {
            AssignedElement = newValue;
            grid.TriggerGridObjectValueUpdated(x, y);
        }
    }
}